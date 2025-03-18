using System;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityActivationBlockedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAbilityActivationBlockedTags:GameplayAbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationBlockedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }

    [Serializable]
    public class ConfAssetAbilityActivationBlockedTags:IGameplayAbilityComponentConfigAsset
    {
        [SerializeField] private int[] tags;
        
        public GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityActivationBlockedTags
            {
                tags = tags
            };
        }
    }
}