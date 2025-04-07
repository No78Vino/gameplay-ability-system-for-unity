using System.Collections.Generic;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Helper;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class EX_GAS_Watcher : OdinEditorWindow
    {
        private const string OpenWindow_MenuItemName = "EX-GAS/Watcher";
#if EX_GAS_ENABLE_HOT_KEYS
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName + " %F11";
#else
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName;
#endif
        [MenuItem(OpenWindow_MenuItemNameEnh, priority = 3)]
        private static void OpenWindow()
        {
            var window = GetWindow<EX_GAS_Watcher>();
            window.titleContent = new GUIContent("EX-GAS监测台");
            window.Show();
        }

        [BoxGroup("Tip")]
        [HideIf(nameof(IsEditorPlaying))]
        [DisplayAsString(false, 16, TextAlignment.Center, true)]
        [ShowInInspector]
        [HideLabel]
        public string Tip =
            "<b><color=#ff6988>EX-GAS检测器仅在游戏运行时生效。</color></b>";

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
        public string ascName => $"<b><color=yellow>{ExGasHelper.GetEntityName(entityWatching)}</color></b>";

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

        private static Dictionary<int, string> _cacheAbilityCode2Name;
        
        private static Dictionary<int, string> CacheAbilityCode2Name
        {
            get
            {
                if (_cacheAbilityCode2Name == null)
                {
                    _cacheAbilityCode2Name = new Dictionary<int, string>();
                    var allAbilityAssets = EXEditorHelper.FindAll<AbilityConfigAsset>();
                    foreach (var abilityAsset in allAbilityAssets)
                    {
                        var abilityBasicInfo = abilityAsset.ConfAssetAbilityBaseInfo;
                        var abilityCode = abilityBasicInfo.Code;
                        var abilityName = abilityBasicInfo.name;
                        _cacheAbilityCode2Name.TryAdd(abilityCode, abilityName);
                    }
                }

                return _cacheAbilityCode2Name;
            }
        }
        
        private static Dictionary<int, string> _cacheAttributeCode2Name;
        
        private static Dictionary<int, string> CacheAttributeCode2Name
        {
            get
            {
                if (_cacheAttributeCode2Name == null)
                {
                    _cacheAttributeCode2Name = new Dictionary<int, string>();
                    var attributeAsset = AttributeAsset.LoadOrCreate();
                    foreach (var attribute in attributeAsset.attributes)
                    {
                        var attributeCode = attribute.GetCode();
                        var attributeName = attribute.Name;
                        _cacheAttributeCode2Name.TryAdd(attributeCode, attributeName);
                    }
                }

                return _cacheAttributeCode2Name;
            }
        }
        
        private static Dictionary<int, string> _cacheAttrSetCode2Name;
        
        private static Dictionary<int, string> CacheAttrSetCode2Name
        {
            get
            {
                if (_cacheAttrSetCode2Name == null)
                {
                    _cacheAttrSetCode2Name = new Dictionary<int, string>();
                    var attributeSetAsset = AttributeSetAsset.LoadOrCreate();
                    foreach (var attributeSet in attributeSetAsset.AttributeSetConfigs)
                    {
                        var attrSetCode = attributeSet.GetCode();
                        var attrSetName = attributeSet.Name;
                        _cacheAttrSetCode2Name.TryAdd(attrSetCode, attrSetName);
                    }
                }

                return _cacheAttrSetCode2Name;
            }
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
            var attrSetBuffer = GASManager.EntityManager.GetBuffer<BEAttributeSet>(ascEntity);
            foreach (var attrSet in attrSetBuffer)
            {
                var attrSetCode = attrSet.Code;
                _ascAttributes.Add($"属性集:{CacheAttrSetCode2Name[attrSetCode]} - [{attrSetCode}]");
                var attributes = attrSet.Attributes;
                foreach (var attribute in attributes)
                    _ascAttributes.Add(
                        $"--- {CacheAttributeCode2Name[attribute.Code]} : {attribute.CurrentValue} (BaseValue:{attribute.BaseValue})"
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
                var tagName = GTagUtil.GetTagFullName(tag.tag);
                if (tagName != null) _ascFixedTags.Add(tagName);
            }

            // 动态标签
            _ascTempTags.Clear();
            var dynamicTagBuffer = GASManager.EntityManager.GetBuffer<BTemporaryTag>(ascEntity);
            foreach (var tag in dynamicTagBuffer)
            {
                var tagName = GTagUtil.GetTagFullName(tag.tag);
                var sourceName = ExGasHelper.GetEntityName(tag.source);
                if (tagName != null) _ascTempTags.Add($"{tagName} ->来源: {sourceName}");
            }
        }

        private void RefreshGameplayEffects()
        {
            _ascGameplayEffects.Clear();
            // var ascEntity = entityWatching;
            // var gameplayEffectBuffer = GASManager.EntityManager.GetBuffer<BEGameplayEffect>(ascEntity);
            // foreach (var gameplayEffect in gameplayEffectBuffer)
            // {
            //     var gameplayEffectName = GASManager.EntityManager.GetName(gameplayEffect.GameplayEffect);
            //     if (gameplayEffectName != null)
            //     {
            //         _ascGameplayEffects.Add(gameplayEffectName);
            //     }
            // }
        }

        private void RefreshAbilities()
        {
            _ascAbilities.Clear();
            var ascEntity = entityWatching;
            var abilityBuffer = GASManager.EntityManager.GetBuffer<BEAbility>(ascEntity);
            foreach (var ability in abilityBuffer)
            {
                var abilityBasicInfo = GASManager.EntityManager.GetComponentData<CAbilityBaseInfo>(ability.Ability);
                var abilityEntityName = ExGasHelper.GetEntityName(ability.Ability);
                var text = $"Lv.{abilityBasicInfo.Level} " +
                           $"- {CacheAbilityCode2Name[abilityBasicInfo.Code]} " +
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