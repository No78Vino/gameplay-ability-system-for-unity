#if UNITY_EDITOR
namespace GAS.Editor.Ability
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using GAS.Editor.GameplayAbilitySystem;
    using GAS.Runtime.Ability;
    
    public class AbilityOverview
    {
        [VerticalGroup("Abilities",order:1)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false,IsReadOnly = true)]
        [DisplayAsString]
        public List<string> Abilities = new List<string>();
        
        
        public AbilityOverview()
        {
            var abilityAssets = EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
            
            abilityAssets.ForEach(ability =>
            {
                Abilities.Add(ability.UniqueName);
            });
        }
        
        [VerticalGroup("Buttons",order:0)]
        [Button("Generate Ability Collection",ButtonSizes.Large,ButtonStyle.Box,Expanded = true,Name = "Generate Ability Collection")]
        void GenerateAbilityCollection()
        {
            var loadMethodCodeString = "Framework.Utilities.AssetUtil.LoadAsset<AbilityAsset>";
            AbilityCollectionGenerator.Gen();
        }
    }
}
#endif