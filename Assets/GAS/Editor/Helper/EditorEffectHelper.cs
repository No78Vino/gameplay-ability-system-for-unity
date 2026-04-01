using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public enum EffectEditComponent
    {
        [LabelText("描述标签")]
        AssetTags,
        
        [LabelText("获得标签")]
        GrantedTags,

        [LabelText("应用需求标签")]
        ApplicationRequiredTags,

        [LabelText("持续需求标签")]
        OngoingRequiredTags,
        
        [LabelText("移除持有标签的buff")]
        RemoveGameplayEffectsWithTags,
        
        [LabelText("被免疫的标签")]
        ImmunityTags,
        
        [LabelText("持续时间")]
        Duration,
        
        [LabelText("间隔执行")]
        Period,
        
        [LabelText("修改器")]
        Modifiers,
        
        [LabelText("应用时触发的Cue")]
        CueOnApply,
        
        [LabelText("帧更新的Cue")]
        CueOnTick,
        
        [LabelText("添加时触发的Cue")]
        CueOnAdd,
        
        [LabelText("移除时触发的Cue")]
        CueOnRemove,	
        
        [LabelText("激活时触发的Cue")]
        CueOnActivate,
        
        [LabelText("失活时触发的Cue")]
        CueOnDeactivate,
        
        [LabelText("获取技能")]
        GrantedAbility,
        
        [LabelText("buff堆叠")]
        Stacking
    }
    
    public static class EditorEffectHelper
    {
        public enum TagRequirementMode
        {
            All,
            Any,
            None
        }

        public readonly struct TagRequirementProtocolField
        {
            public TagRequirementProtocolField(EffectEditComponent component, string excelHeader, string jsonKey, TagRequirementMode mode)
            {
                Component = component;
                ExcelHeader = excelHeader;
                JsonKey = jsonKey;
                Mode = mode;
            }

            public EffectEditComponent Component { get; }
            public string ExcelHeader { get; }
            public string JsonKey { get; }
            public TagRequirementMode Mode { get; }
        }

        public static readonly TagRequirementProtocolField[] TagRequirementProtocolFields =
        {
            new(EffectEditComponent.ApplicationRequiredTags, "ApplicationRequiredTags", "applicationRequiredTags", TagRequirementMode.All),
            new(EffectEditComponent.OngoingRequiredTags, "OngoingRequiredTags", "ongoingRequiredTags", TagRequirementMode.All),
            new(EffectEditComponent.RemoveGameplayEffectsWithTags, "RemoveGameplayEffectsWithTags", "removeEffectsWithTags", TagRequirementMode.Any),
            new(EffectEditComponent.ImmunityTags, "ImmunityTags", "immunityTags", TagRequirementMode.Any)
        };

        public static IEnumerable<EffectEditComponent> ComponentTypes()
        {
            return new[]
            {
                EffectEditComponent.AssetTags,
                EffectEditComponent.GrantedTags,
                EffectEditComponent.ApplicationRequiredTags,
                EffectEditComponent.OngoingRequiredTags,
                EffectEditComponent.RemoveGameplayEffectsWithTags,
                EffectEditComponent.ImmunityTags,
                EffectEditComponent.Duration,
                EffectEditComponent.Period,
                EffectEditComponent.Modifiers,
                EffectEditComponent.CueOnApply,
                EffectEditComponent.CueOnTick,
                EffectEditComponent.CueOnAdd,
                EffectEditComponent.CueOnRemove,
                EffectEditComponent.CueOnActivate,
                EffectEditComponent.CueOnDeactivate,
                EffectEditComponent.GrantedAbility,
                EffectEditComponent.Stacking
            };
        }

        public static GEEditTagRequirement ParseTagRequirementCell(string raw, TagRequirementMode mode)
        {
            var requirement = new GEEditTagRequirement();
            if (string.IsNullOrWhiteSpace(raw)) return requirement;

            var text = raw.Trim();
            var parts = text.Split(';');
            var useRequirementFormat = parts.Length == 3 &&
                                       (text.Contains(",") || parts.Any(p => p == "0" || string.IsNullOrWhiteSpace(p)));

            if (useRequirementFormat)
            {
                requirement.All = ParseTagCsv(parts.Length > 0 ? parts[0] : string.Empty);
                requirement.Any = ParseTagCsv(parts.Length > 1 ? parts[1] : string.Empty);
                requirement.None = ParseTagCsv(parts.Length > 2 ? parts[2] : string.Empty);
                return requirement;
            }

            var tags = ParseLegacyTagList(text);
            switch (mode)
            {
                case TagRequirementMode.All:
                    requirement.All = tags;
                    break;
                case TagRequirementMode.Any:
                    requirement.Any = tags;
                    break;
                case TagRequirementMode.None:
                    requirement.None = tags;
                    break;
            }

            return requirement;
        }

        public static string EncodeTagRequirementCell(GEEditTagRequirement requirement, TagRequirementMode mode)
        {
            if (requirement == null) return string.Empty;
            var tags = GetTagsByMode(requirement, mode);
            if (tags == null || tags.Count == 0) return string.Empty;
            return string.Join(";", tags);
        }

        private static List<int> ParseTagCsv(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw) || raw.Trim() == "0") return new List<int>();
            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out var value) ? value : 0)
                .Where(x => x > 0)
                .ToList();
        }

        private static List<int> ParseLegacyTagList(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return new List<int>();
            return raw.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out var value) ? value : 0)
                .Where(x => x > 0)
                .ToList();
        }

        private static List<int> GetTagsByMode(GEEditTagRequirement requirement, TagRequirementMode mode)
        {
            return mode switch
            {
                TagRequirementMode.All => requirement.All,
                TagRequirementMode.Any => requirement.Any,
                _ => requirement.None
            };
        }
    }
}
