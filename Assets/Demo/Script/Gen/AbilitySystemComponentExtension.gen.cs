///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;

namespace GAS.Runtime
{
    public static class AbilitySystemComponentExtension
    {
        public static Type[] PresetAttributeSetTypes(this AbilitySystemComponent asc)
        {
            if (asc.Preset == null) return null;
            var attrSetTypes = new Type[asc.Preset.AttributeSets.Length];
            for (var i = 0; i < asc.Preset.AttributeSets.Length; i++)
                attrSetTypes[i] = GAttrSetLib.AttrSetTypeDict[asc.Preset.AttributeSets[i]];
            return attrSetTypes;
        }

        public static GameplayTag[] PresetBaseTags(this AbilitySystemComponent asc)
        {
            if (asc.Preset == null) return null;
            return asc.Preset.BaseTags;
        }

        public static void InitWithPreset(this AbilitySystemComponent asc,int level, AbilitySystemComponentPreset preset = null)
        {
            if (preset != null) asc.SetPreset(preset);
            if (asc.Preset == null) return;
            asc.Init(asc.PresetBaseTags(), asc.PresetAttributeSetTypes(), asc.Preset.BaseAbilities,level);
        }
    }
}