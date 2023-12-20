using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Attribute
{
    public class AttributeAsset : ScriptableObject
    {
        [SerializeField] 
        [LabelText("Attribute Collection Gen Path")]
        [LabelWidth(200)]
        public string AttributeCollectionGenPath = "Script/Gen/";
        
        [SerializeField]
        [LabelText("Attribute Names")]
        public List<string> AttributeNames;
    }
}