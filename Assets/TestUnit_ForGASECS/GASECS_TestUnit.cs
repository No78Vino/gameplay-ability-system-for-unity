using System;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestUnit_ForGASECS
{
    public class GASECS_TestUnit : MonoBehaviour
    {
        public Entity EntityASC;

        private EntityManager GasEntityManager => GASManager.EntityManager;
        
        [DisplayAsString] 
        public string _ascName = "NULL";
        
        [TabGroup("FixedTags",GroupName = "Tags")]
        [Sirenix.OdinInspector.ReadOnly]
        public int[] fixedTags;
        
        [TabGroup("TempTags",GroupName = "Tags")]
        [Sirenix.OdinInspector.ReadOnly]
        public int[] tempTags;
        
        [TabGroup("AttrSets",GroupName = "Tags")]
        [Sirenix.OdinInspector.ReadOnly]
        public AttributeSetForShow[] AttrSets;

        [TabGroup("Effects", GroupName = "Tags")]
        [Sirenix.OdinInspector.ReadOnly]
        public EffectForShow[] effects;
        
        private AbilitySystemCell _asc;
        private NewGameplayEffectSpec _geSpec;

        void RefreshUI()
        {
            _ascName = EntityASC.ToString();

            fixedTags = _asc.FixedTags();
            //tempTags = _asc.TempTags();

            var aSet = _asc.AttrSets();
            AttrSets = new AttributeSetForShow[aSet.Length];
            for (int i = 0; i < aSet.Length; i++)
            {
                var attrs = new AttributeForShow[aSet[i].Attributes.Length];
                for (int j = 0; j < aSet[i].Attributes.Length; j++)
                    attrs[j] = new AttributeForShow
                    {
                        Code = aSet[i].Attributes[j].Code,
                        BaseValue = aSet[i].Attributes[j].BaseValue,
                        CurrentValue = aSet[i].Attributes[j].CurrentValue,
                        MinValue = aSet[i].Attributes[j].MinValue,
                        MaxValue = aSet[i].Attributes[j].MaxValue
                    };
                AttrSets[i] = new AttributeSetForShow()
                {
                    Code = aSet[i].Code,
                    Attrs = attrs
                };
            }

            var gameplayEffects = _asc.GameplayEffects();
            effects = new EffectForShow[gameplayEffects.Length];
            for (int i = 0; i < gameplayEffects.Length; i++)
            {
                var bf = gameplayEffects[i];
                var geEntity = bf.GameplayEffect;
                var basicData = GasEntityManager.GetComponentData<ComBasicInfo>(geEntity);
                var dur = GasEntityManager.GetComponentData<ComDuration>(geEntity);
                var mods = GasEntityManager.GetBuffer<BuffEleModifier>(geEntity);
                var modifiers = new ModifierSetting[mods.Length];
                for (int j = 0; j < mods.Length; j++)
                    modifiers[j] = new ModifierSetting()
                    {
                        AttrSetCode = mods[j].AttrSetCode, AttrCode = mods[j].AttrCode,
                        Operation = mods[j].Operation, Magnitude =  mods[j].Magnitude,
                        MMC = new MMCSettingConfig()
                        {
                            TypeCode =  mods[j].MMC.TypeCode,
                            floatParams =  mods[j].MMC.floatParams.ToArray(),
                            intParams =  mods[j].MMC.intParams.ToArray(),
                            stringParams =  StructForShow.FixedStringToStringArray(mods[j].MMC.stringParams)
                        }
                    };
                
                effects[i] = new EffectForShow()
                {
                    name = geEntity.ToString(), Target = basicData.Target.ToString(),
                    Source = basicData.Source.ToString(),
                    // Duration
                    duration = dur.duration, timeUnit = dur.timeUnit, active = dur.active,
                    // Period
                    // public int period;
                    // public string[] gameplayEffects;
                    // // Tags
                    // public int[] AssetTags;
                    // public int[] GrantedTags;
                    // public int[] ApplicationRequiredTags;
                    // public int[] OngoingRequiredTags;
                    // public int[] ImmunityTags;
                    // public int[] RemoveEffectWithTags;
                    // Modifiers
                    modifiers = modifiers,
                };
            }
        }


        [Button(ButtonSizes.Medium,Name = "初始化GAS")]
        void InitGAS()
        {
            GASManager.Initialize();
            GTagList.InitTagList();
            
            GASManager.Run();
        }
        
        [Button(ButtonSizes.Medium,Name = "创建ASC")]
        void CreateASC()
        {
            _asc = new AbilitySystemCell();
            EntityASC = _asc.Entity;
            
            int[] baseTags = { GTagList.Magic_Fire, GTagList.Magic_Water };
            int[] attrSets = { EcsGAttrSetCode.Fight_Monster };
            _asc.Init(baseTags,attrSets,null,1);

            RefreshUI();
        }

        [Button(ButtonSizes.Medium,Name = "创建GE")]
        void CreatGE()
        {
            GameplayEffectComponentConfig[] cfgBurning =
            {
                new ConfBasicInfo {Name = "Test_Burning"},
                new ConfAssetTags {tags = new []{GTagList.Magic_Fire}},
                new ConfModifiers {modifierSettings = new []
                {
                    new ModifierSetting()
                    {
                        AttrSetCode = EcsGAttrSetCode.Fight_Monster,
                        AttrCode = EcsGAttrLib.HP,
                        Operation = GEOperation.Minus,
                        Magnitude = 10,
                        MMC = new MMCSettingConfig()
                        {
                            TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
                            floatParams = new []{0.5f,0},
                        }
                    }
                }}
            };
            _geSpec = GameplayEffectCreator.CreateGameplayEffectSpec(cfgBurning);
        }
        
        [Button(ButtonSizes.Medium,Name = "施加GE到ASC")]
        void ApplyGEToASC()
        {
            _asc.ApplyGameplayEffectTo(_geSpec,_asc);
            RefreshUI();
        }

        [Button(ButtonSizes.Medium,Name = "从ASC移除GE")]
        void RemoveGEFromASC()
        {
            
        }
    }

    [Serializable]
    public struct AttributeSetForShow
    {
        public int Code;
        public AttributeForShow[] Attrs;
    }
    
    [Serializable]
    public struct AttributeForShow
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
    }

    [Serializable]
    public struct EffectForShow
    {
        // BasicData
        public string name;
        public string Target;
        public string Source;
        // Duration
        public int duration;
        public TimeUnit timeUnit;
        public bool active; 
        // Period
        public int period;
        public string[] gameplayEffects;
        // Tags
        public int[] AssetTags;
        public int[] GrantedTags;
        public int[] ApplicationRequiredTags;
        public int[] OngoingRequiredTags;
        public int[] ImmunityTags;
        public int[] RemoveEffectWithTags;
        // Modifiers
        public ModifierSetting[] modifiers;
    }
}