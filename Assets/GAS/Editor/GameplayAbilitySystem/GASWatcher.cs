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

        [BoxGroup("Tip")]
        [HideIf(nameof(IsEditorPlaying))]
        [DisplayAsString(false, 16, TextAlignment.Center, true)]
        [ShowInInspector]
        [HideLabel]
        public string Tip =
            "<b><color=#ff6988>EX-GAS监测台仅在游戏运行时生效。</color></b>";

        [VerticalGroup("ASC")]
        [HorizontalGroup("ASC/Top")]
        [ValueDropdown(nameof(AscEntityChoices), IsUniqueList = true, HideChildProperties = true)]
        [ShowIf(nameof(IsEditorPlaying))]
        [ShowInInspector]
        [HideLabel]
        [OnValueChanged(nameof(OnWatchEntityChanged))]
        public Entity entityWatching = Entity.Null;
        
        [HorizontalGroup("ASC/Top",order:0)]
        [ShowIf(nameof(IsEditorPlaying))]
        [DisplayAsString(EnableRichText = true)]
        [ShowInInspector]
        [HideLabel]
        public string ascName => $"<b><color=yellow>{(entityWatching==Entity.Null?"NULL":EntityHelper.GetEntityName(entityWatching))}</color></b>";

        [TabGroup("ASC/Content", "属性")]
        [ShowIf(nameof(IsEntityValid))]
        [ShowInInspector]
        [DisplayAsString]
        [LabelText(" ")]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        private List<string> _ascAttributes = new();

        [TabGroup("ASC/Content", "标签")]
        [HorizontalGroup("ASC/Content/标签/horizontal")]
        [ShowIf(nameof(IsEntityValid))]
        [ShowInInspector]
        [DisplayAsString]
        [LabelText("固有标签")]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        private List<string> _ascFixedTags = new();

        [HorizontalGroup("ASC/Content/标签/horizontal")]
        [ShowIf(nameof(IsEntityValid))]
        [ShowInInspector]
        [DisplayAsString]
        [LabelText("临时标签")]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        private List<string> _ascTempTags = new();

        [TabGroup("ASC/Content", "能力")]
        [ShowIf(nameof(IsEntityValid))]
        [ShowInInspector]
        [DisplayAsString]
        [LabelText(" ")]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        private List<string> _ascAbilities = new();

        [TabGroup("ASC/Content", "GE效果（Buff）")]
        [ShowIf(nameof(IsEntityValid))]
        [ShowInInspector]
        [DisplayAsString]
        [LabelText(" ")]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        private List<string> _ascGameplayEffects = new();

        private void Update()
        {
            if (IsEditorPlaying() && entityWatching != Entity.Null) RefreshASCContent();
        }

        public bool IsEditorPlaying()
        {
            return Application.isPlaying;
        }

        public bool IsEntityValid()
        {
            return IsEditorPlaying() && entityWatching != Entity.Null;
        }

        private void OnWatchEntityChanged()
        {
            RefreshASCContent();
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

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange != PlayModeStateChange.EnteredPlayMode &&
                playModeStateChange != PlayModeStateChange.ExitingPlayMode) return;
            _cachedAscEntities.Clear();
            entityWatching = Entity.Null;
        }
        #region cache

        private static readonly List<(string, Entity)> _cachedAscEntities = new();

        [HorizontalGroup("ASC/Top",width:200)]
        [Button("刷新当前ASC列表")]
        [ShowIf(nameof(IsEditorPlaying))]
        private static void RefreshAscEntitiesCache()
        {
            _cachedAscEntities.Clear();
            var ascEntities = EntityQueryHelper.GetAllEntitiesWithComponent<CAscBasicData>();
            foreach (var ascEntity in ascEntities)
                _cachedAscEntities.Add(
                    (GASManager.EntityManager.GetName(ascEntity), ascEntity)
                );
            _ascEntityChoices = new ValueDropdownItem[_cachedAscEntities.Count];
            for (var i = 0; i < _cachedAscEntities.Count; i++)
            {
                var (name, entity) = _cachedAscEntities[i];
                _ascEntityChoices[i] = new ValueDropdownItem(name, entity);
            }
        }

        private static ValueDropdownItem[] _ascEntityChoices;

        private static IEnumerable<ValueDropdownItem> AscEntityChoices
        {
            get { return _ascEntityChoices ?? new ValueDropdownItem[] { }; }
        }
        
        private static string GetAbilityNameByCode(int code)
        {
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAbilityNameByCode", code);
            return name!=null ? name as string : "未知技能(配置表内不存在)";
        }
        
        private static string GetAttributeNameByCode(int code)
        {
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAttributeNameByCode", code);
            return name!=null ? name as string : "未知属性(配置表内不存在)";
        }
        
        private static string GetAttrSetNameByCode(int code)
        {
            var name = EXEditorHelper.InvokeStaticXLubanMethod("GetAttrSetNameByCode", code);
            return name!=null ? name as string : "未知属性集(配置表内不存在)";
        }
        
        #endregion

        #region content update

        private void RefreshASCContent()
        {
            RefreshAttributes();
            RefreshTags();
            RefreshGameplayEffects();
            RefreshAbilities();
            
            Repaint();
        }

        private void RefreshAttributes()
        {
            _ascAttributes.Clear();
            var ascEntity = entityWatching;
            var attrSetBuffer = GASManager.EntityManager.GetBuffer<BEAttrSet>(ascEntity);
            foreach (var attrSet in attrSetBuffer)
            {
                var attrSetCode = attrSet.Code;
                _ascAttributes.Add($"属性集:{GetAttrSetNameByCode(attrSetCode)} - [{attrSetCode}]");
                var attributes = attrSet.Attributes;
                foreach (var attribute in attributes)
                    _ascAttributes.Add(
                        $"--- {GetAttributeNameByCode(attribute.Code)} : {attribute.CurrentValue} (BaseValue:{attribute.BaseValue})"
                    );
            }
        }

        private void RefreshTags()
        {
            _ascFixedTags.Clear();
            var ascEntity = entityWatching;
            // 固有标签
            var tagBuffer = GASManager.EntityManager.GetBuffer<BFixedTag>(ascEntity);
            foreach (var tag in tagBuffer)
            {
                var tagName = TagHelper.GetTagFullName(tag.tag);
                if (tagName != null) _ascFixedTags.Add(tagName);
            }

            // 动态标签
            _ascTempTags.Clear();
            var dynamicTagBuffer = GASManager.EntityManager.GetBuffer<BTemporaryTag>(ascEntity);
            foreach (var tag in dynamicTagBuffer)
            {
                var tagName = TagHelper.GetTagFullName(tag.tag);
                var sourceName = EntityHelper.GetEntityName(tag.source);
                if (tagName != null) _ascTempTags.Add($"{tagName} ->来源: {sourceName}");
            }
        }

        private void RefreshGameplayEffects()
        {
            _ascGameplayEffects.Clear();
            var ascEntity = entityWatching;
            var gameplayEffectBuffer = GASManager.EntityManager.GetBuffer<BGameplayEffect>(ascEntity);
            foreach (var gameplayEffect in gameplayEffectBuffer)
            {
                var gameplayEffectName = GASManager.EntityManager.GetName(gameplayEffect.GameplayEffect);
                if (gameplayEffectName != null  
                    && gameplayEffectName!="ENTITY_NOT_FOUND")
                {
                    var inUsage = GASManager.EntityManager.GetComponentData<CEffectInUsage>(gameplayEffect.GameplayEffect);
                    var source = EntityHelper.GetEntityName(inUsage.Source);
                    var text = $"[来源:{source}] Lv.{inUsage.Level} - {gameplayEffectName}";
                    _ascGameplayEffects.Add(text);
                }
                else
                {
                    _ascGameplayEffects.Add("ERROR: GE已被销毁，但未被移出容器！");
                }
            }
        }

        private void RefreshAbilities()
        {
            _ascAbilities.Clear();
            var ascEntity = entityWatching;
            var abilityBuffer = GASManager.EntityManager.GetBuffer<BAbility>(ascEntity);
            foreach (var ability in abilityBuffer)
            {
                var abilityBasicInfo = GASManager.EntityManager.GetComponentData<CAbilityBaseInfo>(ability.Ability);
                var abilityEntityName = EntityHelper.GetEntityName(ability.Ability);
                var text = $"Lv.{abilityBasicInfo.Level} " +
                           $"- {GetAbilityNameByCode(abilityBasicInfo.Code)} " +
                           $"[{abilityEntityName}]";
                if(GASManager.EntityManager.HasComponent<CAbilityActive>(ability.Ability))
                    text += " - 激活中";
                _ascAbilities.Add(text);
            }
        }

        #endregion
    }
}
#endif