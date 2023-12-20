using System.Collections.Generic;
using GAS.Runtime.Effects;

namespace GAS.Runtime.AttributeSet
{
    public class AttributeSetContainer
    {
        List<AttributeSet> _attributeSets = new();
        
        public void AddAttributeSet<T>(T attributeSet) where T : AttributeSet
        {
            if (TryGetAttributeSet<T>(out _)) return;
            
            _attributeSets.Add(attributeSet);
        }
        
        public void RemoveAttributeSet<T>()where T : AttributeSet
        {
            _attributeSets.Remove(_attributeSets.Find(set => set is T));
        }
        
        public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            foreach (var set in _attributeSets)
            {
                if (set is T s)
                {
                    attributeSet = s;
                    return true;
                }
            }

            attributeSet = null;
            return false;
        }
        
        public void ApplyModFromGameplayEffectSpec(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.Modifiers)
            {
                if (TryGetAttributeSet<AttributeSet>(out var attributeSet))
                {
                    attributeSet.ApplyMod(modifier);
                }
            }
        }
    }
}