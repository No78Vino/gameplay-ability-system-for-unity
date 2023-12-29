using UnityEditor;
using UnityEngine;

namespace GAS.Core
{
    public class GASSettingAsset : ScriptableObject
    {
        public string CodeGeneratePath = "Assets/Scripts/Gen";
        public string GASConfigAssetPath = "Assets/GAS_Setting/Config";



        
        
        
        
        private static GASSettingAsset Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GASSettingAsset>(GasDefine.GAS_SYSTEM_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = CreateInstance<GASSettingAsset>();
                AssetDatabase.CreateAsset(a,GasDefine.GAS_SYSTEM_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = CreateInstance<GASSettingAsset>();
            }

            return asset;
        }
        
        static GASSettingAsset _setting;
        public static GASSettingAsset Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = Load();
                }

                return _setting;
            }
        }
        
        public static string ASCLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_ASC_LIBRARY_FOLDER}";
        public static string GameplayEffectLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_EFFECT_LIBRARY_FOLDER}";
        public static string GameplayAbilityLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_ABILITY_LIBRARY_FOLDER}";
        public static string GameplayCueLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_CUE_LIBRARY_FOLDER}";
        public static string MMCLibPath => $"{Setting.GASConfigAssetPath}/{GasDefine.GAS_MMC_LIBRARY_FOLDER}";
        
        public static string GAS_TAG_ASSET_PATH => $"{Setting.GASConfigAssetPath}/GameplayTagsAsset.asset";
        public static string GAS_ATTRIBUTE_ASSET_PATH => $"{Setting.GASConfigAssetPath}/AttributeAsset.asset";
        public static string GAS_ATTRIBUTESET_ASSET_PATH => $"{Setting.GASConfigAssetPath}/AttributeSetAsset.asset";
    }
}