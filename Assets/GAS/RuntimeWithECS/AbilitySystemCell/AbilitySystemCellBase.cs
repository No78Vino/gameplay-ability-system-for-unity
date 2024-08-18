using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public class AbilitySystemCellBase : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        protected EntityManager EntityManager => GASManager.EntityManager;

        private BasicDataComponent BasicData => EntityManager.GetComponentData<BasicDataComponent>(Entity);

        protected void OnEnable()
        {
            if (GASManager.IsInitialized && !EntityManager.Exists(Entity))
            {
                Entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(Entity, new BasicDataComponent());
                EntityManager.AddComponentData(Entity, new GASTagContainer());
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

        public void Init(int[] baseTags, int[] attrSets, AbilityAsset[] baseAbilities, int level)
        {
            // 1.初始化基础标签
            var fixedTags = new DynamicBuffer<int>();
            foreach (var i in baseTags) fixedTags.Add(i);
            EntityManager.SetComponentData(Entity,
                new GASTagContainer { FixedTags = fixedTags, DynamicTags = new DynamicBuffer<int>() });
            
            // 2.创建属性集
            
            // 3.初始化基础技能
            
            // 4.初始化等级
            SetLevel(level);
        }

        public void SetLevel(int level)
        {
            var bdc = BasicData;
            bdc.Level = level;
            EntityManager.SetComponentData(Entity, bdc);
        }
    }
}