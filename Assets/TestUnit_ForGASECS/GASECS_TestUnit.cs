using System;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestUnit_ForGASECS
{
    public class GASECS_TestUnit : MonoBehaviour
    {
        public Entity EntityASC;

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
        
        private AbilitySystemCell _asc;
        private NewGameplayEffectSpec _geSpec;
        
        void RefreshUI()
        {
            _ascName = EntityASC.ToString();
            
            fixedTags = _asc.FixedTags();
            //tempTags = _asc.TempTags();
            
            var  aSet = _asc.AttrSets();
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
            GameplayEffectComponentConfig[] configs =
            {
            };
            _geSpec = GameplayEffectCreator.CreateGameplayEffectSpec(configs);
        }
        
        [Button(ButtonSizes.Medium,Name = "施加GE到ASC")]
        void ApplyGEToASC()
        {
            
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
}