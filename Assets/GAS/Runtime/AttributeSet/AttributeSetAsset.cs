using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.AttributeSet
{
    public struct AttributeSetConfig
    {
        public string Name;
        public string[] AttributeNames;
        
        public void SetName(string name)
        {
            Name = name;
        }
        
        public void SetAttributeNames(string[] attributeNames)
        {
            AttributeNames = attributeNames;
        }
    }
    
    public class AttributeSetAsset:ScriptableObject
    {
        [SerializeField] 
        [LabelText("AttributeSet Collection Gen Path")]
        [LabelWidth(200)]
        public string AttributeSetCollectionGenPath = "Script/Gen/";

        public List<AttributeSetConfig> AttributeSets = new();
    }
}