using System.Collections.Generic;
using GAS.Runtime.Attribute;

namespace GAS.Runtime.AttributeSet
{
    public class CustomAttrSet:AttributeSet
    {
        Dictionary<string,AttributeBase> _attributes = new();

        public void AddAttribute(AttributeBase attribute)
        {
            if (_attributes.ContainsKey(attribute.Name))
                return;
            _attributes.Add(attribute.Name, attribute);
        }
        
        public void RemoveAttribute(AttributeBase attribute)
        {
            _attributes.Remove(attribute.Name);
        }

        public override AttributeBase this[string key] =>
            _attributes.TryGetValue(key, value: out var attribute) ? attribute : null;
            
    }
}