using GAS.RuntimeWithECS.Static;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public class AbilitySystemCell
    {
        private readonly AbilityController _abilityController;
        private readonly AttrSetController _attrSetController;

        private readonly BasicDataController _basicDataController;
        private readonly GameplayEffectController _gameplayEffectController;
        private readonly GameplayTagController _gameplayTagController;

        public AbilitySystemCell()
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
            _abilityController = new AbilityController(Entity);
        }

        public Entity Entity { get; private set; }
        private EntityManager EntityManager => GASManager.EntityManager;

        public void Dispose()
        {
            EntityHelper.DestroyEntity(Entity);
            Entity = Entity.Null;
        }

        public void Init(int[] baseTags, int[] attrSets, AbilityConfig[] baseAbilities, int level = 1)
        {
            // 1.初始化基础标签
            _gameplayTagController.AddFixedTags(baseTags);
            // 2.创建属性集
            foreach (var attrSetCode in attrSets)
            {
                var attrSetConfig = XAttrSet.AttributeSetMap[attrSetCode];
                _attrSetController.AddAttrSet(attrSetConfig);
            }

            // 3.初始化基础技能
            foreach (var abilityConfig in baseAbilities)
            {
                var ability = AbilityHelper.CreateAbilityEntity(abilityConfig.ComponentConfigs);
                _abilityController.GrantAbility(ability);
            }

            // 4.初始化等级
            SetLevel(level);
        }

        public GameObject GameObject => EntityHelper.GetGameObjectFromEntity(Entity);
        
        #region GameplayEffect

        public void ApplyGameplayEffectTo(NewGameplayEffectSpec gameplayEffectSpec, AbilitySystemCell target)
        {
            _gameplayEffectController.ApplyGameplayEffectTo(gameplayEffectSpec, target);
        }
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

        #region BasicData

        public void SetLevel(int level)
        {
            _basicDataController.SetLevel(level);
        }

        public int GetLevel()
        {
            return _basicDataController.GetLevel();
        }

        #endregion

        #region GameplayTag

        public bool HasTag(int tag)
        {
            return _gameplayTagController.HasTag(tag);
        }

        public void KillFixedTag(int tag)
        {
            _gameplayTagController.KillFixedTag(tag);
        }

        #endregion

        #region Attribute

        public float GetAttrCurrentValue(int attrSetCode,int attributeCode)
        {
            return _attrSetController.GetCurrentValue(attrSetCode,attributeCode);
        }
        
        public float GetAttrBaseValue(int attrSetCode,int attributeCode)
        {
            return _attrSetController.GetBaseValue(attrSetCode,attributeCode);
        }
        
        public void SetAttrBaseValue(int attrSetCode,int attributeCode,float value)
        {
            _attrSetController.SetBaseValue(attrSetCode,attributeCode,value);
        }
        
        #endregion

        #region Ability

        public void TryActivateAbility(int abilityCode, IAbilityParam param = null)
        {
            _abilityController.TryActivateAbility(abilityCode, param);
        }

        public void TryEndAbility(int abilityCode)
        {
            _abilityController.EndAbility(abilityCode);
        }

        public void TryCancelAbility(int abilityCode)
        {
            _abilityController.CancelAbility(abilityCode);
        }

        public bool IsAbilityActive(int abilityCode)
        {
            return _abilityController.IsAbilityActive(abilityCode);
        }

        public void SetAbilityParam(int abilityCode, IAbilityParam param)
        {
            _abilityController.SetAbilityParam(abilityCode, param);
        }

        public MCAbilityLogic GetAbilityLogic(int abilityCode)
        {
            return _abilityController.GetAbilityLogic(abilityCode);
        }

        #endregion


#if UNITY_EDITOR
        public int[] FixedTags()
        {
            return _gameplayTagController.FixedTags();
        }

        public DynamicBuffer<BEAttributeSet> AttrSets()
        {
            var attrBuffer = EntityManager.GetBuffer<BEAttributeSet>(Entity);
            return attrBuffer;
        }

        public DynamicBuffer<BEGameplayEffect> GameplayEffects()
        {
            return _gameplayEffectController.CurrentGameplayEffects;
        }
#endif
    }
}