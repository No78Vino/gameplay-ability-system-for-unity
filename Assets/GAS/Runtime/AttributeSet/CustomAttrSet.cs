using System.Collections.Generic;

namespace GAS.Runtime
{
    public class CustomAttrSet:AttributeSet
    {
        Dictionary<string,AttributeBase> _attributes = new Dictionary<string,AttributeBase>();

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

        public override string[] AttributeNames { get; }
        public override void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
            foreach (var attribute in _attributes.Values)
                attribute.SetOwner(owner);
        }
    }
}