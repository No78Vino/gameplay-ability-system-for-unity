using System;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Modifier;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace TestUnit_ForGASECS
{
    public class GASECS_TestUnit : MonoBehaviour
    {
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
        private NewGameplayEffectSpec _geSpec;
        public Entity EntityASC;

        private EntityManager GasEntityManager => GASManager.EntityManager;

        private float _secondCount = 0;
        private const float UIRefreshDuration = 2;

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
            GASManager.Initialize();
            GTagList.InitTagList();
            MmcHub.Init();

            GASManager.Run();
        }

        [Button(ButtonSizes.Medium, Name = "创建ASC")]
        private void CreateASC()
        {
            _asc = new AbilitySystemCell();
            int[] baseTags = { GTagList.Magic_Fire, GTagList.Magic_Water };
            int[] attrSets = { EcsGAttrSetCode.Fight_Monster };
            _asc.Init(baseTags, attrSets, null);
            
            EntityASC = _asc.Entity;
            RefreshUI();
        }

        [Button(ButtonSizes.Medium, Name = "创建GE")]
        private void CreatGE()
        {
            _geSpec = GameplayEffectCreator.CreateGameplayEffectSpec(TestASCUnitUtils.GEConfig_ONEHIT);
        }

        [Button(ButtonSizes.Medium, Name = "施加普通攻击")]
        private void ApplyGEToASC()
        {
            _asc.ApplyGameplayEffectTo(_geSpec, _asc);
            RefreshUI();
        }

        [Button(ButtonSizes.Medium, Name = "施加要求Earth标签的攻击")]
        private void ApplyEarthHitToASC()
        {
            _asc.ApplyGameplayEffectTo(GameplayEffectCreator.CreateGameplayEffectSpec(TestASCUnitUtils.GEConfig_ONEHIT_REQUIRED_EARTH_TAG), _asc);
            RefreshUI();
            
        }
        [Button(ButtonSizes.Medium, Name = "从ASC移除GE")]
        private void RemoveGEFromASC()
        {
        }
    }
}