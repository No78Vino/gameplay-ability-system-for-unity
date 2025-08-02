using System;
using GAS.RuntimeWithECS.ComponentConfig;
using UnityEditor;

namespace GAS.RuntimeDataHelper.Ability
{
    [Serializable]
    public abstract class BaseGameplayAbilityComponentConfigAsset
    {
        protected GEN_AbilityConfigSO Asset;

        public void SetOwnAsset(GEN_AbilityConfigSO asset)
        {
            Asset = asset;
        }

        protected virtual void OnValueChanged()
        {
            if (Asset) EditorUtility.SetDirty(Asset);
        }

        public abstract GameplayAbilityComponentConfig GetConfig();
    }
}