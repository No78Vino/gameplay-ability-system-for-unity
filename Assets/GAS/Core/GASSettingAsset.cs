using UnityEngine;

namespace GAS.Core
{
    public class GASSettingAsset : ScriptableObject
    {
        public string CodeGeneratePath = "Scripts/Gen";
        public string GASConfigAssetPath = "Assets/GAS_Setting/Config";
        public string GameplayEffectLibPath = "Assets/GAS_Setting/Config/GameplayEffectLib";
        public string GameplayAbilityLibPath = "Assets/GAS_Setting/Config/GameplayAbilityLib";
        public string GameplayCueLibPath = "Assets/GAS_Setting/Config/GameplayCueLib";
    }
}