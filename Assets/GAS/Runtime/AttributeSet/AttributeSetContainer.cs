using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AttributeSetContainer
    {
        private readonly AbilitySystemComponent _owner;
        private readonly Dictionary<string,AttributeSet> _attributeSets = new Dictionary<string,AttributeSet>();
        private readonly Dictionary<AttributeBase,AttributeAggregator> _attributeAggregators = new Dictionary<AttributeBase, AttributeAggregator>();
        
        public Dictionary<string,AttributeSet> Sets => _attributeSets;
        
        public AttributeSetContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void AddAttributeSet<T>() where T : AttributeSet
        {
            if (TryGetAttributeSet<T>(out _)) return;
            
            var setName = AttributeSetUtil.AttributeSetName(typeof(T));
            _attributeSets.Add(setName,Activator.CreateInstance<T>());
            
            var attrSet = _attributeSets[setName];
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
            var setName = AttributeSetUtil.AttributeSetName(attrSetType);
            _attributeSets.Add(setName,Activator.CreateInstance(attrSetType) as AttributeSet);
            
            var attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    _attributeAggregators.Add(attrSet[attr],new AttributeAggregator(attrSet[attr],_owner));
                }
            }
            attrSet.SetOwner(_owner);
        }
        
        /// <summary>
        /// Be careful when using this method, it may cause unexpected errors(when using network sync).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAttributeSet<T>()where T : AttributeSet
        {
            var setName = AttributeSetUtil.AttributeSetName(typeof(T));
            var attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                _attributeAggregators.Remove(attrSet[attr]);
            }

            _attributeSets.Remove(setName);
        }
        
        public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            if(_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(typeof(T)), out var set))
            {
                attributeSet = (T)set;
                return true;
            }
            
            attributeSet = null;
            return false;
        }        
        
        bool TryGetAttributeSet(Type attrSetType,out AttributeSet attributeSet)
        {
            if(_attributeSets.TryGetValue(AttributeSetUtil.AttributeSetName(attrSetType), out var set))
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
                    var attr = attributeSet.Value[name];
                    snapshot.Add(attr.Name, attr.CurrentValue);
                }
            }
            return snapshot;
        }
        
        public void Dispose()
        {
            foreach (var aggregator in _attributeAggregators)
                aggregator.Value.OnDispose();
            
        }
    }
}