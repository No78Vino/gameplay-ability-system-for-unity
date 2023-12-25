using EXMaid;
using UnityEditor;

namespace GAS.Core
{
    public static class GasDefine
    {
        public const string GAS_VERSION = "1.0.0";
        
        public const string GAS_MENU_ROOT = "Gameplay Ability System/";
        
        public const int GAS_TAG_MAX_GENERATIONS = 5;
        
        public const string GAS_ASSET_FOLDER_NAME = "GAS_Setting";

        public const string GAS_ATTRIBUTE_COLLECTION_CSHARP_SCRIPT_NAME = "AttributeCollection.gen.cs";
        
        public const string GAS_ATTRIBUTESET_CLASS_CSHARP_SCRIPT_NAME = "AttributeSetClass.gen.cs";
        
        public static string GAS_ASSET_PATH => $"Assets/{GAS_ASSET_FOLDER_NAME}";
        public static string GAS_SYSTEM_ASSET_PATH => $"{GAS_ASSET_PATH}/GASSystemAsset.asset";
        public static string GAS_TAG_ASSET_PATH => $"{GAS_ASSET_PATH}/GameplayTagsAsset.asset";
        public static string GAS_ATTRIBUTE_ASSET_PATH => $"{GAS_ASSET_PATH}/AttributeAsset.asset";
        public static string GAS_ATTRIBUTESET_ASSET_PATH => $"{GAS_ASSET_PATH}/AttributeSetAsset.asset";

        public static void CheckGasAssetFolder()
        {
            if (!AssetDatabase.IsValidFolder(GAS_ASSET_PATH))
            {
                AssetDatabase.CreateFolder("Assets", GAS_ASSET_FOLDER_NAME);
                EXLog.Log($"{GAS_ASSET_FOLDER_NAME} folder created!");
            }
        }
    }
}