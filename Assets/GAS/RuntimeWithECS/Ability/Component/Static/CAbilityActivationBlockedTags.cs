using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Static
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
}