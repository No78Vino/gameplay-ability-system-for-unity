using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.AttributeSet
{
    public class AttributeSetConfig
    {
        public string Name;
        public List<string> AttributeNames;
    }
    
    public class AttributeSetAsset:ScriptableObject
    {
        public string AttributeSetClassGenPath = "Script/Gen/";

        [SerializeField] private List<AttributeSetConfig> _attributeSetData = new List<AttributeSetConfig>();
        
        public List<AttributeSetConfig> AttributeSetConfigs=new List<AttributeSetConfig>();

        public void OnEnable()
        {
            AttributeSetConfigs.Clear();
            AttributeSetConfigs.AddRange(_attributeSetData);
        }

        private void OnValidate()
        {
            Save();
        }

        public void Save()
        {
            _attributeSetData.Clear();
            _attributeSetData.AddRange(AttributeSetConfigs);
        }
    }
}