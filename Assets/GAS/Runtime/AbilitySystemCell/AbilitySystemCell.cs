using System.Collections.Generic;
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
            GASManager.BindAscToEntity(Entity, this);

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
        private static EntityManager EntityManager => GASManager.EntityManager;

        public void Dispose()  
        {  
            GASManager.UnbindAscToEntity(Entity);  
            if (GASManager.ExWorld != null && GASManager.ExWorld.IsCreated)  
                EntityHelper.DestroyEntity(Entity);  
            Entity = Entity.Null;  
        }

        public void Init(IEnumerable<int> baseTags, IEnumerable<AttrSetConfig> attrSets, IEnumerable<AbilityConfig> baseAbilities, int level = 1)
        {
            // 1.初始化基础标签
            _gameplayTagController.AddFixedTags(baseTags);
            // 2.创建属性集
            foreach (var attrSet in attrSets)
                _attrSetController.AddAttrSet(attrSet);
            
            // 3.初始化基础技能
            foreach (var abilityConfig in baseAbilities)
                GrantAbility(abilityConfig);

            // 4.初始化等级
            SetLevel(level);
        }

        public GameObject GameObject => EntityHelper.GetGameObjectFromEntity(Entity);
        
        #region GameplayEffect

        public void ApplyGameplayEffectTo(GameplayEffectSpec gameplayEffectSpec, AbilitySystemCell target)
        {
            _gameplayEffectController.ApplyGameplayEffectTo(gameplayEffectSpec, target);
        }
        
        public void ApplyGameplayEffectToSelf(GameplayEffectSpec gameplayEffectSpec)
        {
            _gameplayEffectController.ApplyGameplayEffectTo(gameplayEffectSpec, this);
        }

        public void RemoveGameplayEffect(GameplayEffectSpec gameplayEffectSpec)
        {
            _gameplayEffectController.RemoveGameplayEffect(gameplayEffectSpec.Entity);
        }

        public void ClearGameplayEffects() => _gameplayEffectController.ClearGameplayEffects();

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

        public bool HasTag(int tag) => _gameplayTagController.HasTag(tag);

        public bool HasAllTags(IEnumerable<int> tags) => _gameplayTagController.HasAllTags(tags);
        
        public bool HasAnyTags(IEnumerable<int> tags) => _gameplayTagController.HasAnyTags(tags);
        
        public void AddFixedTags(IEnumerable<int> tags) => _gameplayTagController.AddFixedTags(tags);
        
        public bool AddFixedTag(int tag) => _gameplayTagController.AddFixedTag(tag);
        
        public void KillFixedTag(int tag) => _gameplayTagController.KillFixedTag(tag);
        
        public void KillFixedTags(IEnumerable<int> tags) => _gameplayTagController.KillFixedTags(tags);

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

        public void TryActivateAbility(int abilityCode, XParam param = null)
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

        public void SetAbilityParam(int abilityCode, XParam param)
        {
            _abilityController.SetAbilityParam(abilityCode, param);
        }

        public MCAbilityLogic GetAbilityLogic(int abilityCode)
        {
            return _abilityController.GetAbilityLogic(abilityCode);
        }
        
        public AbilitySpec GetAbilitySpec(int abilityCode)
        {
            return _abilityController.GetAbilitySpec(abilityCode);
        }

        public void GrantAbility(AbilityConfig abilityCfg)
        {
            _abilityController.GrantAbility(abilityCfg);
        }

        public void RemoveAbility(int abilityCode)
        {
            _abilityController.RemoveAbility(abilityCode);
        }
        
        #endregion


#if UNITY_EDITOR
        public int[] FixedTags()
        {
            return _gameplayTagController.FixedTags();
        }

        public DynamicBuffer<BEAttrSet> AttrSets()
        {
            var attrBuffer = EntityManager.GetBuffer<BEAttrSet>(Entity);
            return attrBuffer;
        }

        public DynamicBuffer<BGameplayEffect> GameplayEffects()
        {
            return _gameplayEffectController.CurrentGameplayEffects;
        }
#endif
    }
}