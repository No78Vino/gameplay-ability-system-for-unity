namespace GAS
{
    public static class GasDefine
    {
        public const string GAS_VERSION = "1.1.8";

        public const string GAS_TAG_LIB_CSHARP_SCRIPT_NAME = "GTagLib.gen.cs";

        public const string GAS_ATTRIBUTE_LIB_CSHARP_SCRIPT_NAME = "GAttrLib.gen.cs";

        public const string GAS_ATTRIBUTESET_LIB_CSHARP_SCRIPT_NAME = "GAttrSetLib.gen.cs";

        public const string GAS_ABILITY_LIB_CSHARP_SCRIPT_NAME = "GAbilityLib.gen.cs";

        public const string GAS_ASCUTIL_CSHARP_SCRIPT_NAME = "AbilitySystemComponentExtension.gen.cs";

        /// <summary>
        /// TODO
        /// I will try to make an Ability-Script-Generator in the future. 
        /// </summary>
        public const string GAS_GAMEPLAYABILITY_CLASS_CSHARP_SCRIPT_NAME = "GameplayAbilityClass.gen.cs";


        public const string GAS_ASC_LIBRARY_FOLDER = "AbilitySystemComponentLib";

        public const string GAS_EFFECT_LIBRARY_FOLDER = "GameplayEffectLib";

        public const string GAS_ABILITY_LIBRARY_FOLDER = "GameplayAbilityLib";

        public const string GAS_CUE_LIBRARY_FOLDER = "GameplayCueLib";

        public const string GAS_MMC_LIBRARY_FOLDER = "ModMagnitudeCalculationLib";

        public const string GAS_ABILITY_TASK_LIBRARY_FOLDER = "AbilityTaskLib";

        public const string GAS_ATTRIBUTESET_CLASS_TYPE_PREFIX = "GAS.Runtime.AttributeSet.AS_";

#if UNITY_EDITOR
        public const string GAS_BASE_SETTING_PATH = "ProjectSettings/GASSettingAsset.asset";
        public const string GAS_TAGS_MANAGER_ASSET_PATH = "ProjectSettings/GameplayTagsAsset.asset";
        public const string GAS_ATTRIBUTE_ASSET_PATH = "ProjectSettings/AttributeAsset.asset";
        public const string GAS_ATTRIBUTE_SET_ASSET_PATH = "ProjectSettings/AttributeSetAsset.asset";
#endif
    }
}