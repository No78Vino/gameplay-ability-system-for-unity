using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime.Attribute
{
    public class AttributeAsset : ScriptableObject
    {
        [SerializeField] 
        public string AttributeCollectionGenPath = "Script/Gen/";
        
        [SerializeField]
        public List<string> AttributeNames;
    }
}