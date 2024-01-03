using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime.Attribute
{
    public class AttributeAsset : ScriptableObject
    {
        [SerializeField]
        public List<string> AttributeNames;
    }
}