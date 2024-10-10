using System;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.AttributeSet;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public class AbilitySystemCellBase
    {
        public Entity Entity { get; private set; }
        private EntityManager EntityManager => GASManager.EntityManager;
        
        
        private readonly BasicDataController _basicDataController;
        private readonly AttrSetController _attrSetController;
        private readonly GameplayTagController _gameplayTagController;
        private readonly GameplayEffectController _gameplayEffectController;
        private readonly GameplayAbilityController _gameplayAbilityController;

        public AbilitySystemCellBase()
        {
            Entity = EntityManager.CreateEntity();
            EntityManager.SetName(Entity, $"ASC_V{Entity.Version}_{Entity.Index}");

            // 1.基础信息
            _basicDataController = new BasicDataController(Entity);
            // 2.AttrSet属性集控制器
            _attrSetController = new AttrSetController(Entity);
            // 3.Tag控制器
            _gameplayTagController = new GameplayTagController(Entity);
            // 4.GameplayEffect控制器
            _gameplayEffectController = new GameplayEffectController(Entity);
            // 5.Ability控制器
            _gameplayAbilityController = new GameplayAbilityController(Entity);


            // // 测试数据
            // _attrSetContainer.AddAttrSet(EcsGAttrSetLib.AS_FIGHT);
            // var v1 = _attrSetContainer.GetBaseValue(EcsGAttrSetLib.AS_FIGHT.Code, EcsGAttrLib.HP);
            // _attrSetContainer.InitBaseValue(EcsGAttrSetLib.AS_FIGHT.Code, EcsGAttrLib.HP,50);
            // var v2 = _attrSetContainer.GetBaseValue(EcsGAttrSetLib.AS_FIGHT.Code, EcsGAttrLib.HP);
        }

        protected void Dispose()
        {
            EntityManager.DestroyEntity(Entity);
            Entity = Entity.Null;
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

        public void SetLevel(int level) => _basicDataController.SetLevel(level);
        public int GetLevel() => _basicDataController.GetLevel();



        #region GameplayEffect 相关操作

        // private NewGameplayEffectSpec AddGameplayEffectEntityTo(Entity gameplayEffect, Entity target)
        // {
        // var attrBuffer = EntityManager.GetBuffer<AttributeSetBufferElement>(Entity);
        // var newAttrs = new AttributeData[config.Settings.Length];
        // for (var i = 0; i < config.Settings.Length; i++)
        // {
        //     var setting = config.Settings[i];
        //     newAttrs[i] = new AttributeData
        //     {
        //         Code = setting.Code,
        //         BaseValue = setting.InitValue,
        //         CurrentValue = setting.InitValue,
        //         MinValue = setting.Min,
        //         MaxValue = setting.Max
        //     };
        // }
        //
        // attrBuffer.Add(new AttributeSetBufferElement
        // {
        //     Code = attrSetCode,
        //     Attributes = new NativeArray<AttributeData>(newAttrs, Allocator.Persistent)
        // });
        // return true;

        //return target.AddGameplayEffect(this, gameplayEffectSpec);
        // }

        // public GameplayEffectSpec ApplyGameplayEffectTo(NewGameplayEffectSpec gameplayEffect, AbilitySystemCellBase target)
        // {
        //     return target.AddGameplayEffect(this, gameplayEffectSpec);
        // }

//         public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)
//         {
//             if (gameplayEffect == null)
//             {
// #if UNITY_EDITOR
//                 Debug.LogError($"[EX] Try To Apply a NULL GameplayEffect From {name} To {target.name}!");
// #endif
//                 return null;
//             }
//
//             var spec = gameplayEffect.CreateSpec();
//             return ApplyGameplayEffectTo(spec, target);
//         }
//
//         public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target,
//             int effectLevel)
//         {
//             if (gameplayEffect == null)
//             {
// #if UNITY_EDITOR
//                 Debug.LogError($"[EX] Try To Apply a NULL GameplayEffect From {name} To {target.name}!");
// #endif
//                 return null;
//             }
//
//             var spec = gameplayEffect.CreateSpec();
//             spec.SetLevel(effectLevel);
//             return ApplyGameplayEffectTo(spec, target);
//         }
//
//         public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffectSpec gameplayEffectSpec)
//         {
//             return ApplyGameplayEffectTo(gameplayEffectSpec, this);
//         }
//
//         public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
//         {
//             return ApplyGameplayEffectTo(gameplayEffect, this);
//         }
//
//         public void RemoveGameplayEffectSpec(GameplayEffectSpec gameplayEffectSpec)
//         {
//             GameplayEffectContainer.RemoveGameplayEffectSpec(gameplayEffectSpec);
//         }

        #endregion
    }
}