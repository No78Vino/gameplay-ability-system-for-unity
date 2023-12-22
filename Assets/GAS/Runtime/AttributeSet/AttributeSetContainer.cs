using System.Collections.Generic;
using GAS.Runtime.Attribute;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.AttributeSet
{
    public class AttributeSetContainer
    {
        List<AttributeSet> _attributeSets = new();
        
        List<GameplayEffectExecution> _effectExecutions = new();
        
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
        
        public AttributeBase GetAttribute(string name)
        {
            foreach (var set in _attributeSets)
            {
                if (set[name]!=null)
                {
                    return set[name];
                }
            }

            return null;
        }
        
        void ApplyModFromInstantEffect(GameplayEffectSpec spec)
        {
            // TODO
            
            // for (var i = 0; i < spec.GameplayEffect.Modifiers.Length; i++)
            // {
            //     var modifier = spec.GameplayEffect.Modifiers[i];
            //     var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
            //     var attribute = modifier.Attribute;
            //
            //     // If attribute doesn't exist on this character, continue to next attribute
            //     if (attribute == null) continue;
            //     TryGetAttributeSet<attribute>(out var attributeValue);
            //
            //     switch (modifier.Operation)
            //     {
            //         case GEOperation.Add:
            //             attributeValue.BaseValue += magnitude;
            //             break;
            //         case GEOperation.Multiply:
            //             attributeValue.BaseValue *= magnitude;
            //             break;
            //         case GEOperation.Override:
            //             attributeValue.BaseValue = magnitude;
            //             break;
            //     }
            //     _attributeSetContainer.(attribute, attributeValue.BaseValue);
            // }
        }
        
        void ApplyModFromDurationalEffect(GameplayEffectSpec spec)
        {
            // TODO
            
            // GameplayEffectSpec.ModifierContainer[] modifiersToApply = new GameplayEffectSpec.ModifierContainer[spec.GameplayEffect.gameplayEffect.Modifiers.Length];
            // for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            // {
            //     var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
            //     var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
            //     var attributeModifier = new AttributeModifier();
            //     switch (modifier.ModifierOperator)
            //     {
            //         case EAttributeModifier.Add:
            //             attributeModifier.Add = magnitude;
            //             break;
            //         case EAttributeModifier.Multiply:
            //             attributeModifier.Multiply = magnitude;
            //             break;
            //         case EAttributeModifier.Override:
            //             attributeModifier.Override = magnitude;
            //             break;
            //     }
            //     modifiersToApply[i] = new() { Attribute = modifier.Attribute, Modifier = attributeModifier };
            // }
            // spec.modifiers = modifiersToApply;
            // spec.RaiseOnApplyEvent();
            // HandleRemoveGameplayEffectsWithTag(spec);
            // AppliedGameplayEffects.Add(spec);
            
            //_effectExecutions.Add(new GameplayEffectExecution(spec));
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
    }
}