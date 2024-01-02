﻿using UnityEditor;
using UnityEngine;

namespace GAS.Core
{
    public static class GasDefine
    {
        public const string GAS_VERSION = "1.0.0";
        
        public const string GAS_ASSET_FOLDER_NAME = "GAS_Setting";

        public const string GAS_TAG_COLLECTION_CSHARP_SCRIPT_NAME = "GameplayTagDefineCollection.gen.cs";
        
        public const string GAS_ATTRIBUTE_COLLECTION_CSHARP_SCRIPT_NAME = "AttributeCollection.gen.cs";
        
        public const string GAS_ATTRIBUTESET_CLASS_CSHARP_SCRIPT_NAME = "AttributeSetClass.gen.cs";
        
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
        
        
        
        public static string GAS_ASSET_PATH => $"Assets/{GAS_ASSET_FOLDER_NAME}";
        public static string GAS_SYSTEM_ASSET_PATH => $"{GAS_ASSET_PATH}/GASSettingAsset.asset";
        public static string GAS_TAG_ASSET_PATH => GASSettingAsset.GAS_TAG_ASSET_PATH;
        public static string GAS_ATTRIBUTE_ASSET_PATH => GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH;
        public static string GAS_ATTRIBUTESET_ASSET_PATH => GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH;

        public static void CheckGasAssetFolder()
        {
            if (!AssetDatabase.IsValidFolder(GAS_ASSET_PATH))
            {
                AssetDatabase.CreateFolder("Assets", GAS_ASSET_FOLDER_NAME);
                Debug.Log($"[EX] {GAS_ASSET_FOLDER_NAME} folder created!");
            }
        }
    }
}