using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime.AttributeSet
{
    [Serializable]
    public class AttributeSetConfig
    {
        public string Name;
        public List<string> AttributeNames;
    }

    public class AttributeSetAsset : ScriptableObject
    {
        public string AttributeSetClassGenPath = "Script/Gen/";
        
        [SerializeField] private List<AttributeSetConfig> _attributeSetConfigs;

        public List<AttributeSetConfig> AttributeSetConfigs => _attributeSetConfigs;
    }
}