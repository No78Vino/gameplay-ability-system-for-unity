using UnityEditor;
using UnityEngine;

namespace GAS
{
    public static class GasDefine
    {
        public const string GAS_VERSION = "1.0.4";

        public const string GAS_ASSET_FOLDER_NAME = "GAS_Setting";

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
        
        
        public static string GAS_ASSET_PATH => $"Assets/{GAS_ASSET_FOLDER_NAME}";
        public static string GAS_SYSTEM_ASSET_PATH => $"{GAS_ASSET_PATH}/GASSettingAsset.asset";

        public const string GAS_ATTRIBUTESET_CLASS_TYPE_PREFIX = "GAS.Runtime.AttributeSet.AS_";

#if UNITY_EDITOR
        public static void CheckGasAssetFolder()
        {
            if (!AssetDatabase.IsValidFolder(GAS_ASSET_PATH))
            {
                AssetDatabase.CreateFolder("Assets", GAS_ASSET_FOLDER_NAME);
                Debug.Log($"[EX] {GAS_ASSET_FOLDER_NAME} folder created!");
            }
        }
#endif
    }
}