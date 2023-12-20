using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.AttributeSet
{
    public struct AttributeSetConfig
    {
        public string Name;
        public string[] AttributeNames;
    }
    
    public class AttributeSetAsset:ScriptableObject
    {
        [SerializeField] 
        [LabelText("AttributeSet Collection Gen Path")]
        [LabelWidth(200)]
        public string AttributeSetCollectionGenPath = "Script/Gen/";

        public List<AttributeSetConfig> AttributeSet = new();
    }
}