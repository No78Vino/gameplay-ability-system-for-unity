using System.Collections.Generic;
using GAS.Runtime.Component;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectContainer
    {
        private AbilitySystemComponent _owner;
        
        List<GameplayEffectSpec> _activeGameplayEffects = new();
        List<GameplayEffectSpec> _deactiveGameplayEffects = new();
        
        public GameplayEffectContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        public void Tick()
        {
            foreach (var gameplayEffectSpec in _activeGameplayEffects)
            {
                gameplayEffectSpec.Tick();
            }
        }
        
        public void AddGameplayEffectSpec(GameplayEffectSpec spec)
        {
            if (spec.CanRunning())
            {
                spec.Activate();
                _activeGameplayEffects.Add(spec);
            }
            else
            {
                spec.Deactivate();
                _deactiveGameplayEffects.Add(spec);
            }
            spec.TriggerOnAdd();
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            spec.Deactivate();
            spec.TriggerOnRemove();
            
            _activeGameplayEffects.Remove(spec);
            _deactiveGameplayEffects.Remove(spec);
        }

        public void CheckGameplayEffectState()
        {
            // TODO
            if (_owner)
            {
                
            }
            
            // foreach (var VARIABLE in COLLECTION)
            // {
            //     
            // }
            // if (spec.IsExpired())
            // {
            //     _deactiveGameplayEffects.Add(spec);
            // }
        }
    }
}