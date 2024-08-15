using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public class AbilitySystemCellBase : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        protected EntityManager EntityManager => GASManager.EntityManager;

        protected void OnEnable()
        {
            if (GASManager.IsInitialized && !EntityManager.Exists(Entity))
            {
                Entity = EntityManager.CreateEntity();
            }
        }

        protected void OnDisable()
        {
            if (GASManager.IsInitialized && EntityManager.Exists(Entity))
            {
                EntityManager.DestroyEntity(Entity);
                Entity = Entity.Null;
            }
        }
        
        public void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities, int level)
        {

            // if (baseTags != null) GameplayTagAggregator.Init(baseTags);
            //
            // if (attrSetTypes != null)
            // {
            //     foreach (var attrSetType in attrSetTypes)
            //         AttributeSetContainer.AddAttributeSet(attrSetType);
            // }
            //
            // if (baseAbilities != null)
            // {
            //     foreach (var info in baseAbilities)
            //         GrantAbility(info);
            // }
            
            SetLevel(level);
        }
        
        public void SetLevel(int level)
        {
            var bdc = BasicData;
            bdc.Level = level;
            EntityManager.SetComponentData(Entity,bdc);
        }

        private BasicDataComponent BasicData => EntityManager.GetComponentData<BasicDataComponent>(Entity);
    }
}