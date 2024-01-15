#if UNITY_EDITOR
namespace GAS.Editor.GameplayAbilitySystem
{
    using GAS.Core;
    using UnityEditor;
    using UnityEngine;

    
    public class GASSettingAsset : ScriptableObject
    {
        public string CodeGeneratePath = "Assets/Scripts/Gen";
        public string GASConfigAssetPath = "Assets/GAS_Setting/Config";
        public string StringCodeOfLoadAbilityAsset = "UnityEngine.Resources.Load<AbilityAsset>(\"{0}\")";
        //"Framework.Utilities.AssetUtil.LoadAsset<AbilityAsset>({0})";



        
        
        
        
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
        
        public static string CodeGenPath => Setting.CodeGeneratePath;
        
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
#endif