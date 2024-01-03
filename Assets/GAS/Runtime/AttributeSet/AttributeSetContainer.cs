using System;
using System.Collections.Generic;
using GAS.Runtime.Attribute;
using GAS.Runtime.Component;

namespace GAS.Runtime.AttributeSet
{
    public class AttributeSetContainer
    {
        AbilitySystemComponent _owner;
        Dictionary<string,AttributeSet> _attributeSets = new Dictionary<string,AttributeSet>();
        Dictionary<AttributeBase,AttributeAggregator> _attributeAggregators = new Dictionary<AttributeBase, AttributeAggregator>();
        
        public Dictionary<string,AttributeSet> Sets => _attributeSets;
        
        public AttributeSetContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void AddAttributeSet<T>() where T : AttributeSet
        {
            if (TryGetAttributeSet<T>(out _)) return;
            
            _attributeSets.Add(nameof(T),Activator.CreateInstance<T>());
            
            var attrSet = _attributeSets[nameof(T)];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    _attributeAggregators.Add(attrSet[attr],new AttributeAggregator(attrSet[attr],_owner));
                }
            }
        }
        
        public void AddAttributeSet(Type attrSetType)
        {
            if (TryGetAttributeSet(attrSetType,out _)) return;
            
            _attributeSets.Add(attrSetType.Name,Activator.CreateInstance(attrSetType) as AttributeSet);
            
            var attrSet = _attributeSets[attrSetType.Name];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    _attributeAggregators.Add(attrSet[attr],new AttributeAggregator(attrSet[attr],_owner));
                }
            }
        }
        
        /// <summary>
        /// Be careful when using this method, it may cause unexpected errors(when using network sync).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAttributeSet<T>()where T : AttributeSet
        {
            var attrSet = _attributeSets[nameof(T)];
            foreach (var attr in attrSet.AttributeNames)
            {
                _attributeAggregators.Remove(attrSet[attr]);
            }

            _attributeSets.Remove(nameof(T));
        }
        
        bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            if(_attributeSets.TryGetValue(nameof(T), out var set))
            {
                attributeSet = (T)set;
                return true;
            }
            
            attributeSet = null;
            return false;
        }        
        
        bool TryGetAttributeSet(Type attrSetType,out AttributeSet attributeSet)
        {
            if(_attributeSets.TryGetValue(attrSetType.Name, out var set))
            {
                attributeSet = set;
                return true;
            }
            
            attributeSet = null;
            return false;
        }
        
        public float? GetAttributeBaseValue(string attrSetName,string attrShortName)
        {
            return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName].BaseValue : (float?)null;
        }
        
        public float? GetAttributeCurrentValue(string attrSetName,string attrShortName)
        {
            return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName].CurrentValue : (float?)null;
        }
        
        public Dictionary<string, float> Snapshot()
        {
            Dictionary<string, float> snapshot = new Dictionary<string, float>();
            foreach (var attributeSet in _attributeSets)
            {
                foreach (var name in attributeSet.Value.AttributeNames)
                {
                    snapshot.Add(name, attributeSet.Value[name].CurrentValue);
                }
            }
            return snapshot;
        }
    }
}