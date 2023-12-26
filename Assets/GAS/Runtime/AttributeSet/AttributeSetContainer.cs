using System;
using System.Collections.Generic;
using GAS.Runtime.Attribute;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.AttributeSet
{
    public class AttributeSetContainer
    {
        Dictionary<string,AttributeSet> _attributeSets = new Dictionary<string,AttributeSet>();
        
        List<GameplayEffectSpec> _appliedGameplayEffectSpecs = new List<GameplayEffectSpec>();
        
        public void AddAttributeSet<T>() where T : AttributeSet
        {
            if (TryGetAttributeSet<T>(out _)) return;
            
            _attributeSets.Add(nameof(T),Activator.CreateInstance<T>());
        }
        
        /// <summary>
        /// Be careful when using this method, it may cause unexpected errors(when using network sync).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAttributeSet<T>()where T : AttributeSet
        {
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
        
        public AttributeBase GetAttribute(string attrSetName,string attrShortName)
        {
            return _attributeSets.TryGetValue(attrSetName, out var set) ? set[attrShortName] : null;
        }
        
        void ApplyModFromInstantEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attribute = GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attribute == null) continue;
                var magnitude = modifier.ModifierMagnitude.CalculateMagnitude(modifier.Coefficient);
                var baseValue = attribute.BaseValue;
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        baseValue += magnitude;
                        break;
                    case GEOperation.Multiply:
                        baseValue *= magnitude;
                        break;
                    case GEOperation.Override:
                        baseValue = magnitude;
                        break;
                }

                _attributeSets[modifier.AttributeSetName].ChangeAttributeBase(attribute, baseValue);
            }
        }
        
        void ApplyModFromDurationalEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attribute = GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attribute == null) continue;
                var magnitude = modifier.ModifierMagnitude.CalculateMagnitude(modifier.Coefficient);
                var currentValue = attribute.CurrentValue;
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        currentValue += magnitude;
                        break;
                    case GEOperation.Multiply:
                        currentValue *= magnitude;
                        break;
                    case GEOperation.Override:
                        currentValue = magnitude;
                        break;
                }
                _attributeSets[modifier.AttributeSetName].ChangeAttribute(attribute, currentValue);
            }

            _appliedGameplayEffectSpecs.Add(spec);
        }
        
        public void ApplyModFromGameplayEffectSpec(GameplayEffectSpec spec)
        {
            if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                ApplyModFromInstantEffect(spec);
            }
            else
            {
                ApplyModFromDurationalEffect(spec);
            }
        }
        
        public void RemoveModFromGameplayEffectSpec(GameplayEffectSpec spec)
        {
            // TODO
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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