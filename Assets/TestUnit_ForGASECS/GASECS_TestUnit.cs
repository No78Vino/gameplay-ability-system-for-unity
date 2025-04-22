using DemoForESC._Script.Gen;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace TestUnit_ForGASECS
{
    public class GASECS_TestUnit : MonoBehaviour
    {
        private const float UIRefreshDuration = 0.5f;
        [DisplayAsString] public string _ascName = "NULL";

        [TabGroup("FixedTags", GroupName = "Tags")] [ReadOnly]
        public int[] fixedTags;

        [TabGroup("TempTags", GroupName = "Tags")] [ReadOnly]
        public int[] tempTags;

        [TabGroup("AttrSets", GroupName = "Tags")] [ReadOnly]
        public AttributeSetForShow[] AttrSets;

        [TabGroup("Effects", GroupName = "Tags")] [ReadOnly]
        public EffectForShow[] effects;


        private AbilitySystemCell _asc;

        private float _secondCount;
        public Entity EntityASC;

        private EntityManager GasEntityManager => GASManager.EntityManager;

        private void Update()
        {
            if (_secondCount <= 0)
            {
                _secondCount = UIRefreshDuration;
                RefreshUI();
            }

            _secondCount -= Time.deltaTime;
        }

        private void RefreshUI()
        {
            if (_asc == null) return;

            _ascName = EntityASC.ToString();

            fixedTags = _asc.FixedTags();
            //tempTags = _asc.TempTags();

            var aSet = _asc.AttrSets();
            AttrSets = new AttributeSetForShow[aSet.Length];
            for (var i = 0; i < aSet.Length; i++)
            {
                var attrs = new AttributeForShow[aSet[i].Attributes.Length];
                for (var j = 0; j < aSet[i].Attributes.Length; j++)
                    attrs[j] = new AttributeForShow
                    {
                        Code = aSet[i].Attributes[j].Code,
                        BaseValue = aSet[i].Attributes[j].BaseValue,
                        CurrentValue = aSet[i].Attributes[j].CurrentValue,
                        MinValue = aSet[i].Attributes[j].MinValue,
                        MaxValue = aSet[i].Attributes[j].MaxValue
                    };
                AttrSets[i] = new AttributeSetForShow
                {
                    Code = aSet[i].Code,
                    Attrs = attrs
                };
            }

            var gameplayEffects = _asc.GameplayEffects();
            effects = new EffectForShow[gameplayEffects.Length];
            for (var i = 0; i < gameplayEffects.Length; i++)
            {
                var bf = gameplayEffects[i];
                var geEntity = bf.GameplayEffect;
                var effectForShow = new EffectForShow();
                effectForShow.SetGameplayEffectEntity(geEntity);
                effects[i] = effectForShow;
            }
        }


        [Button(ButtonSizes.Medium, Name = "初始化GAS")]
        private void InitGAS()
        {
            GEN_GASLauncher.Launch();
            GASManager.Run();
        }

        [Button(ButtonSizes.Medium, Name = "创建ASC")]
        private void CreateASC()
        {
            _asc = new AbilitySystemCell();
            //int[] baseTags = { GEN_GameplayTagCode.Magic_Fire, GEN_GameplayTagCode.Magic_Water };
            // int[] attrSets = { GEN_AttrSetCode.Fight_Monster };
            AbilityConfig[] abilityConfigs =
            {
                TestASCUnitUtils.AbilityConfig_Debug
            };
            //_asc.Init(baseTags, attrSets, abilityConfigs);

            EntityASC = _asc.Entity;
            RefreshUI();
        }

        [Button(ButtonSizes.Medium, Name = "施加普通攻击")]
        private void ApplyGEToASC()
        {
            // var geSpec =
            //     GEUtil.CreateGameplayEffectSpec(TestASCUnitUtils.GEConfig_ONEHIT);
            // _asc.ApplyGameplayEffectTo(geSpec, _asc);
            var gameplayEffect = GEUtil.CreateGameplayEffectEntity(TestASCUnitUtils.GEConfig_ONEHIT);
            //GEUtil.ApplyGameplayEffectTo(gameplayEffect, _asc.Entity, _asc.Entity);
        }

        [Button(ButtonSizes.Medium, Name = "施加要求Earth标签的攻击")]
        private void ApplyEarthHitToASC()
        {
            var geSpec =
                GEUtil.CreateGameplayEffectSpec(TestASCUnitUtils.GEConfig_ONEHIT_REQUIRED_EARTH_TAG);
            _asc.ApplyGameplayEffectTo(geSpec, _asc);
        }

        [Button(ButtonSizes.Medium, Name = "燃烧buff")]
        private void ApplyBurningToASC()
        {
            var geSpec =
                GEUtil.CreateGameplayEffectSpec(TestASCUnitUtils.GEConfig_BURNING);
            _asc.ApplyGameplayEffectTo(geSpec, _asc);
        }

        [Button(ButtonSizes.Medium, Name = "从ASC移除GE")]
        private void RemoveGEFromASC()
        {
        }

        [Button(ButtonSizes.Medium, Name = "启用/关闭debug能力")]
        private void SwitchAbilityDebugLog()
        {
            // bool isActivated = _asc.IsAbilityActive(GEN_AbilityCode.DebugLog);
            // if (!isActivated)
            // {
            //     _asc.TryActivateAbility(GEN_AbilityCode.DebugLog);
            //     _asc.SetAbilityParam(GEN_AbilityCode.DebugLog, new AbilityParamString("Hello,World!"+Time.time));
            // }
            // else
            //     _asc.TryEndAbility(GEN_AbilityCode.DebugLog);
        }
    }
}