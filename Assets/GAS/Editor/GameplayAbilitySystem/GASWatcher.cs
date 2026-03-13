using System;  
using System.Collections.Generic;  
using GAS.Runtime;  
using Sirenix.OdinInspector;  
using Sirenix.OdinInspector.Editor;  
using Unity.Entities;  
using UnityEditor;  
using UnityEngine;  
  
#if UNITY_EDITOR  
namespace GAS.Editor  
{  
    public class GASWatcher : OdinEditorWindow  
    {  
        private const string OpenWindow_MenuItemName = "EXTool/EX-GAS/监测台";  
#if EX_GAS_ENABLE_HOT_KEYS  
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName + " %F11";  
#else  
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName;  
#endif  
  
        [MenuItem(OpenWindow_MenuItemNameEnh, priority = 3)]  
        private static void OpenWindow()  
        {  
            var window = GetWindow<GASWatcher>();  
            window.titleContent = new GUIContent("EX-GAS监测台");  
            window.Show();  
        }  
  
        // ======================== 提示 ========================  
        [BoxGroup("Tip")]  
        [HideIf(nameof(IsEditorPlaying))]  
        [DisplayAsString(false, 16, TextAlignment.Center, true)]  
        [ShowInInspector]  
        [HideLabel]  
        public string Tip = "<b><color=#ff6988>EX-GAS监测台仅在游戏运行时生效。</color></b>";  
  
        // ======================== ASC 选择器 ========================  
        [VerticalGroup("ASC")]  
        [HorizontalGroup("ASC/Top")]  
        [ValueDropdown(nameof(AscEntityChoices), IsUniqueList = true, HideChildProperties = true)]  
        [ShowIf(nameof(IsEditorPlaying))]  
        [ShowInInspector]  
        [HideLabel]  
        [OnValueChanged(nameof(OnWatchEntityChanged))]  
        public Entity entityWatching = Entity.Null;  
  
        [HorizontalGroup("ASC/Top", order: 0)]  
        [ShowIf(nameof(IsEditorPlaying))]  
        [DisplayAsString(EnableRichText = true)]  
        [ShowInInspector]  
        [HideLabel]  
        public string ascName =>  
            $"<b><color=yellow>{(entityWatching == Entity.Null ? "NULL" : EntityHelper.GetEntityName(entityWatching))}</color></b>";  
  
        // ======================== 全局信息栏 ========================  
        [VerticalGroup("ASC")]  
        [ShowIf(nameof(IsEntityValid))]  
        [ShowInInspector]  
        [DisplayAsString(EnableRichText = true)]  
        [HideLabel]  
        public string GlobalInfo => _globalInfoText;  
        private string _globalInfoText = "";  
  
        // ======================== 属性区块 ========================  
        [FoldoutGroup("ASC/属性", expanded: true)]  
        [ShowIf(nameof(IsEntityValid))]  
        [ShowInInspector]  
        [DisplayAsString(EnableRichText = true)]  
        [HideLabel]  
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]  
        private List<string> _ascAttributes = new();  
  
        // ======================== 标签区块 ========================  
        [FoldoutGroup("ASC/标签", expanded: true)]  
        [ShowIf(nameof(IsEntityValid))]  
        [ShowInInspector]  
        [DisplayAsString(EnableRichText = true)]  
        [HideLabel]  
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]  
        private List<string> _ascTags = new();  
  
        // ======================== 能力区块 ========================  
        [FoldoutGroup("ASC/能力", expanded: true)]  
        [ShowIf(nameof(IsEntityValid))]  
        [ShowInInspector]  
        [DisplayAsString(EnableRichText = true)]  
        [HideLabel]  
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]  
        private List<string> _ascAbilities = new();  
  
        // ======================== GE效果区块 ========================  
        [FoldoutGroup("ASC/GE效果(Buff)", expanded: true)]  
        [ShowIf(nameof(IsEntityValid))]  
        [ShowInInspector]  
        [DisplayAsString(EnableRichText = true)]  
        [HideLabel]  
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]  
        private List<string> _ascGameplayEffects = new();  
  
        // ======================== 刷新控制 ========================  
        private double _lastRepaintTime;  
        private const double RepaintInterval = 0.1; // 100ms  
  
        private void Update()  
        {  
            if (!IsEntityValid()) return;  
  
            var now = EditorApplication.timeSinceStartup;  
            if (now - _lastRepaintTime < RepaintInterval) return;  
            _lastRepaintTime = now;  
  
            RefreshASCContent();  
        }  
  
        public bool IsEditorPlaying() => Application.isPlaying;  
  
        public bool IsEntityValid()  
        {  
            return IsEditorPlaying()  
                   && entityWatching != Entity.Null  
                   && GASManager.EntityManager.Exists(entityWatching);  
        }  
  
        private void OnWatchEntityChanged()  
        {  
            ClearAllCaches();  
            if (IsEntityValid()) RefreshASCContent();  
        }  
  
        protected override void OnEnable()  
        {  
            base.OnEnable();  
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;  
        }  
  
        protected override void OnDisable()  
        {  
            base.OnDisable();  
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;  
        }  
  
        private void OnPlayModeStateChanged(PlayModeStateChange state)  
        {  
            if (state != PlayModeStateChange.EnteredPlayMode &&  
                state != PlayModeStateChange.ExitingPlayMode) return;  
            _cachedAscEntities.Clear();  
            entityWatching = Entity.Null;  
            ClearAllCaches();  
        }  
  
        private static void ClearAllCaches()  
        {  
            _abilityNameCache.Clear();  
            _attributeNameCache.Clear();  
            _attrSetNameCache.Clear();  
        }  
  
        #region ASC列表 & 名称缓存  
  
        private static readonly List<(string, Entity)> _cachedAscEntities = new();  
  
        [HorizontalGroup("ASC/Top", width: 200)]  
        [Button("刷新当前ASC列表")]  
        [ShowIf(nameof(IsEditorPlaying))]  
        private static void RefreshAscEntitiesCache()  
        {  
            _cachedAscEntities.Clear();  
            var ascEntities = EntityQueryHelper.GetAllEntitiesWithComponent<CAscBasicData>();  
            foreach (var ascEntity in ascEntities)  
                _cachedAscEntities.Add((GASManager.EntityManager.GetName(ascEntity), ascEntity));  
            _ascEntityChoices = new ValueDropdownItem[_cachedAscEntities.Count];  
            for (var i = 0; i < _cachedAscEntities.Count; i++)  
            {  
                var (name, entity) = _cachedAscEntities[i];  
                _ascEntityChoices[i] = new ValueDropdownItem(name, entity);  
            }  
        }  
  
        private static ValueDropdownItem[] _ascEntityChoices;  
        private static IEnumerable<ValueDropdownItem> AscEntityChoices =>  
            _ascEntityChoices ?? new ValueDropdownItem[] { };  
  
        // ---------- 名称缓存 ----------  
        private static readonly Dictionary<int, string> _abilityNameCache = new();  
        private static readonly Dictionary<int, string> _attributeNameCache = new();  
        private static readonly Dictionary<int, string> _attrSetNameCache = new();  
  
        private static string GetAbilityNameByCode(int code)  
        {  
            if (_abilityNameCache.TryGetValue(code, out var c)) return c;  
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAbilityNameByCode", code);  
            var r = name != null ? name as string : "未知技能";  
            _abilityNameCache[code] = r;  
            return r;  
        }  
  
        private static string GetAttributeNameByCode(int code)  
        {  
            if (_attributeNameCache.TryGetValue(code, out var c)) return c;  
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAttributeNameByCode", code);  
            var r = name != null ? name as string : "未知属性";  
            _attributeNameCache[code] = r;  
            return r;  
        }  
  
        private static string GetAttrSetNameByCode(int code)  
        {  
            if (_attrSetNameCache.TryGetValue(code, out var c)) return c;  
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAttrSetNameByCode", code);  
            var r = name != null ? name as string : "未知属性集";  
            _attrSetNameCache[code] = r;  
            return r;  
        }  
  
        private static string GetTagName(int tagCode) =>  
            TagHelper.GetTagFullName(tagCode) ?? $"Tag({tagCode})";  
  
        private static string OpName(GEOperation op) => op switch  
        {  
            GEOperation.Add => "+",  
            GEOperation.Minus => "-",  
            GEOperation.Multiply => "×",  
            GEOperation.Divide => "÷",  
            GEOperation.Override => "=",  
            _ => op.ToString()  
        };  
  
        #endregion  
  
        #region 数据刷新  
  
        private void RefreshASCContent()  
        {  
            if (!IsEntityValid()) return;  
            try  
            {  
                RefreshGlobalInfo();  
                RefreshAttributes();  
                RefreshTags();  
                RefreshAbilities();  
                RefreshGameplayEffects();  
            }  
            catch (Exception e)  
            {  
                Debug.LogWarning($"[GASWatcher] Refresh: {e.Message}");  
            }  
            Repaint();  
        }  
  
        // ---------- 全局信息 ----------  
        private void RefreshGlobalInfo()  
        {  
            var em = GASManager.EntityManager;  
            var gt = em.GetComponentData<GlobalTimer>(GASManager.EntityGlobalTimer);  
            var lvl = em.HasComponent<CAscBasicData>(entityWatching)  
                ? em.GetComponentData<CAscBasicData>(entityWatching).Level  
                : 0;  
            _globalInfoText =  
                $"<b>Frame</b>:{gt.Frame}  <b>Turn</b>:{gt.Turn}  <b>ASC Lv</b>:{lvl}  <b>Entity</b>:{EntityHelper.GetEntityName(entityWatching)}";  
        }  
  
        // ---------- 属性 ----------  
        private void RefreshAttributes()  
        {  
            _ascAttributes.Clear();  
            var em = GASManager.EntityManager;  
            var buf = em.GetBuffer<BEAttrSet>(entityWatching);  
            foreach (var attrSet in buf)  
            {  
                _ascAttributes.Add(  
                    $"<b><color=#88ccff>【{GetAttrSetNameByCode(attrSet.Code)}】</color></b>");  
                foreach (var a in attrSet.Attributes)  
                {  
                    var diff = Math.Abs(a.CurrentValue - a.BaseValue) > 0.001f;  
                    var cc = diff ? "<color=orange>" : "<color=white>";  
                    var clamp = "";  
                    if (a.IsClampMin || a.IsClampMax)  
                    {  
                        var parts = new List<string>();  
                        if (a.IsClampMin) parts.Add($"Min:{a.MinValue}");  
                        if (a.IsClampMax) parts.Add($"Max:{a.MaxValue}");  
                        clamp = $" <color=#666>[{string.Join(",", parts)}]</color>";  
                    }  
                    var dirty = a.Dirty ? " <color=red>●</color>" : "";  
                    _ascAttributes.Add(  
                        $"  {GetAttributeNameByCode(a.Code)}: {cc}{a.CurrentValue:F2}</color> (Base:{a.BaseValue:F2}){clamp}{dirty}");  
                }  
            }  
        }  
  
        // ---------- 标签 ----------  
        private void RefreshTags()  
        {  
            _ascTags.Clear();  
            var em = GASManager.EntityManager;  
            var fixBuf = em.GetBuffer<BFixedTag>(entityWatching);  
            var tmpBuf = em.GetBuffer<BTemporaryTag>(entityWatching);  
  
            _ascTags.Add($"<b>固有({fixBuf.Length})</b>");  
            foreach (var t in fixBuf)  
            {  
                var n = TagHelper.GetTagFullName(t.tag);  
                if (n != null) _ascTags.Add($"  {n}");  
            }  
            _ascTags.Add($"<b>临时({tmpBuf.Length})</b>");  
            foreach (var t in tmpBuf)  
            {  
                var n = TagHelper.GetTagFullName(t.tag);  
                var src = EntityHelper.GetEntityName(t.source);  
                if (n != null) _ascTags.Add($"  {n} <color=#888>← {src}</color>");  
            }  
        }  
  
        // ---------- 能力 ----------  
        private void RefreshAbilities()  
        {  
            _ascAbilities.Clear();  
            var em = GASManager.EntityManager;  
            var gt = em.GetComponentData<GlobalTimer>(GASManager.EntityGlobalTimer);  
            var buf = em.GetBuffer<BAbility>(entityWatching);  
            if (buf.Length == 0)  
            {  
                _ascAbilities.Add("<color=#888>无</color>");  
                return;  
            }  
  
            foreach (var ab in buf)  
            {  
                var ent = ab.Ability;  
                if (!em.Exists(ent)) continue;  
                var info = em.GetComponentData<CAbilityBaseInfo>(ent);  
                var aName = GetAbilityNameByCode(info.Code);  
                var eName = EntityHelper.GetEntityName(ent);  
                var active = em.HasComponent<CAbilityActive>(ent);  
                var actStr = active ? " <color=lime>[激活]</color>" : "";  
                _ascAbilities.Add(  
                    $"<b><color=#ffcc44>{aName}</color></b> Lv.{info.Level} [{eName}]{actStr}");  
  
                // --- CD ---  
                if (em.HasComponent<CAbilityCooldown>(ent))  
                {  
                    var cd = em.GetComponentData<CAbilityCooldown>(ent);  
                    
                    // === 方案B: CooldownGameplayEffectInstance已移除，改用CooldownTags ===  
                    // 取消注释下方代码块，并删除上方方案A代码块  
                     
                    if (cd.CooldownTags.IsCreated && cd.CooldownTags.Length > 0)  
                    {  
                        var tagNames = new List<string>();  
                        foreach (var tc in cd.CooldownTags) tagNames.Add(GetTagName(tc));  
                        _ascAbilities.Add(  
                            $"  <color=#cc8888>CDTag: {string.Join(", ", tagNames)}</color>");  
  
                        var owner = em.GetComponentData<CAbilityBaseInfo>(ent).Owner;  
                        bool inCd = ASCHelper.HasAnyTags(owner, cd.CooldownTags);  
                        if (inCd)  
                        {  
                            var cdRem = -1;  
                            var cdTotal = cd.Cooldown;  
                            var cdUnit = "帧";  
                            var geBuffer = em.GetBuffer<BGameplayEffect>(owner);  
                            for (var gi = 0; gi < geBuffer.Length; gi++)  
                            {  
                                var ge = geBuffer[gi].GameplayEffect;  
                                if (!em.Exists(ge) || !em.HasComponent<CEffectGrantedTags>(ge))  
                                    continue;  
                                var gTags = em.GetComponentData<CEffectGrantedTags>(ge);  
                                bool match = false;  
                                foreach (var ct in cd.CooldownTags)  
                                {  
                                    foreach (var gt2 in gTags.tags)  
                                    {  
                                        if (TagHelper.HasTag(gt2, ct))  
                                        {  
                                            match = true;  
                                            break;  
                                        }  
                                    }  
                                    if (match) break;  
                                }  
                                if (match && em.HasComponent<CDuration>(ge))  
                                {  
                                    var dur = em.GetComponentData<CDuration>(ge);  
                                    cdUnit = dur.timeUnit == TimeUnit.Frame ? "帧" : "回合";  
                                    var curTime = dur.timeUnit == TimeUnit.Frame ? gt.Frame : gt.Turn;  
                                    if (dur.StopTickWhenDeactivated && !dur.active)  
                                        cdRem = dur.remianTime;  
                                    else  
                                        cdRem = dur.duration > 0  
                                            ? Math.Max(0, dur.duration - (curTime - dur.activeTime))  
                                            : -1;  
                                    cdTotal = dur.duration;  
                                    break;  
                                }  
                            }  
                            if (cdRem >= 0)  
                                _ascAbilities.Add(  
                                    $"  <color=red>CD: {cdRem}/{cdTotal}{cdUnit}</color>");  
                            else  
                                _ascAbilities.Add(  
                                    $"  <color=red>冷却中 (配置:{cd.Cooldown})</color>");  
                        }  
                        else  
                        {  
                            _ascAbilities.Add(  
                                $"  <color=lime>CD就绪</color> (配置:{cd.Cooldown})");  
                        }  
                    }  
                    else  
                    {  
                        _ascAbilities.Add(  
                            $"  <color=#888>CD配置:{cd.Cooldown} (无冷却Tag)</color>");  
                    }  
                    // === 方案B 结束 ===  
                }  
  
                // --- ActivationOwnedTags ---  
                if (em.HasComponent<CAbilityActivationOwnedTags>(ent))  
                {  
                    var ownedTags = em.GetComponentData<CAbilityActivationOwnedTags>(ent);  
                    if (ownedTags.tags.Length > 0)  
                    {  
                        var names = new List<string>();  
                        foreach (var tc in ownedTags.tags) names.Add(GetTagName(tc));  
                        _ascAbilities.Add(  
                            $"  <color=#88dd88>激活授予Tag: {string.Join(", ", names)}</color>");  
                    }  
                }  
  
                // --- Logic 类型 ---  
                if (em.HasComponent<MCAbilityLogic>(ent))  
                {  
                    var logic = EntityHelper.GetManagedComponentData<MCAbilityLogic>(ent);  
                    if (logic?.Logic != null)  
                    {  
                        _ascAbilities.Add(  
                            $"  <color=#aaaaaa>逻辑: {logic.Logic.GetType().Name}</color>");  
                    }  
                }  
            }  
        }  
  
        // ---------- GE效果 ----------  
        private void RefreshGameplayEffects()  
        {  
            _ascGameplayEffects.Clear();  
            var em = GASManager.EntityManager;  
            var gt = em.GetComponentData<GlobalTimer>(GASManager.EntityGlobalTimer);  
            var geBuf = em.GetBuffer<BGameplayEffect>(entityWatching);  
  
            if (geBuf.Length == 0)  
            {  
                _ascGameplayEffects.Add("<color=#888>无</color>");  
                return;  
            }  
  
            for (var i = 0; i < geBuf.Length; i++)  
            {  
                var geEntity = geBuf[i].GameplayEffect;  
                var geName = em.GetName(geEntity);  
  
                if (geName == null || geName == "ENTITY_NOT_FOUND")  
                {  
                    _ascGameplayEffects.Add(  
                        "<color=red>ERROR: GE已被销毁，但未被移出容器！</color>");  
                    continue;  
                }  
  
                // 基本信息  
                var inUsage = em.GetComponentData<CEffectInUsage>(geEntity);  
                var source = EntityHelper.GetEntityName(inUsage.Source);  
                _ascGameplayEffects.Add(  
                    $"<b><color=#ffcc44>[{i}] {geName}</color></b>  Lv.{inUsage.Level}  <color=#888>来源:{source}</color>");  
  
                // Duration  
                if (em.HasComponent<CDuration>(geEntity))  
                {  
                    var dur = em.GetComponentData<CDuration>(geEntity);  
                    var actStr = dur.active  
                        ? "<color=lime>[激活]</color>"  
                        : "<color=red>[失活]</color>";  
                    var unit = dur.timeUnit == TimeUnit.Frame ? "帧" : "回合";  
                    string durStr;  
                    if (dur.duration <= 0)  
                    {  
                        durStr = $"无限({unit})";  
                    }  
                    else  
                    {  
                        var cur = dur.timeUnit == TimeUnit.Frame ? gt.Frame : gt.Turn;  
                        int rem;  
                        if (dur.StopTickWhenDeactivated && !dur.active)  
                            rem = dur.remianTime;  
                        else  
                            rem = Math.Max(0, dur.duration - (cur - dur.activeTime));  
                        durStr = $"剩余:{rem}/{dur.duration}{unit}";  
                    }  
                    _ascGameplayEffects.Add($"    {actStr} {durStr}");  
                }  
  
                // Stacking  
                if (em.HasComponent<CStacking>(geEntity))  
                {  
                    var st = em.GetComponentData<CStacking>(geEntity);  
                    var stType = st.StackType == EffectStackType.AggregateBySource  
                        ? "BySource"  
                        : "ByTarget";  
                    _ascGameplayEffects.Add(  
                        $"    <color=#cc99ff>层数:{st.StackCount}/{st.LimitCount} ({stType})</color>");  
                }  
  
                // Period  
                if (em.HasComponent<CPeriod>(geEntity))  
                {  
                    var per = em.GetComponentData<CPeriod>(geEntity);  
                    var unit = "帧";  
                    if (em.HasComponent<CDuration>(geEntity))  
                    {  
                        var dur = em.GetComponentData<CDuration>(geEntity);  
                        unit = dur.timeUnit == TimeUnit.Frame ? "帧" : "回合";  
                    }  
                    _ascGameplayEffects.Add(  
                        $"    <color=#99cccc>周期:{per.Period}{unit}</color>");  
                }  
  
                // GrantedTags  
                if (em.HasComponent<CEffectGrantedTags>(geEntity))  
                {  
                    var tags = em.GetComponentData<CEffectGrantedTags>(geEntity);  
                    if (tags.tags.Length > 0)  
                    {  
                        var names = new List<string>();  
                        foreach (var tc in tags.tags) names.Add(GetTagName(tc));  
                        _ascGameplayEffects.Add(  
                            $"    <color=#88dd88>GrantedTags: {string.Join(", ", names)}</color>");  
                    }  
                }  
  
                // AssetTags  
                if (em.HasComponent<CEffectAssetTags>(geEntity))  
                {  
                    var tags = em.GetComponentData<CEffectAssetTags>(geEntity);  
                    if (tags.tags.Length > 0)  
                    {  
                        var names = new List<string>();  
                        foreach (var tc in tags.tags) names.Add(GetTagName(tc));  
                        _ascGameplayEffects.Add(  
                            $"    <color=#aaaaaa>AssetTags: {string.Join(", ", names)}</color>");  
                    }  
                }  
  
                // Modifiers  
                if (em.HasComponent<MCModifiers>(geEntity))  
                {  
                    var mods = em.GetComponentData<MCModifiers>(geEntity);  
                    if (mods?.Modifiers != null && mods.Modifiers.Length > 0)  
                    {  
                        var parts = new List<string>();  
                        foreach (var m in mods.Modifiers)  
                        {  
                            parts.Add(  
                                $"{GetAttrSetNameByCode(m.AttrSetCode)}.{GetAttributeNameByCode(m.AttrCode)} [{OpName(m.Operation)}] {m.Magnitude:F2}");  
                        }  
                        _ascGameplayEffects.Add(  
                            $"    <color=#ddaa66>Modifiers: {string.Join(" | ", parts)}</color>");  
                    }  
                }  
  
                // 分隔线  
                if (i < geBuf.Length - 1)  
                    _ascGameplayEffects.Add(  
                        "<color=#444>────────────────────────────────</color>");  
            }  
        }  
  
        #endregion  
    }  
}  
#endif