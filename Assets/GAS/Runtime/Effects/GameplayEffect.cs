using System.Collections.Generic;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects
{
    public enum EffectsDurationPolicy
    {
        Instant, 
        Infinite, 
        Duration
    }
    
    public struct GameplayEffect
    {
        GameplayEffectSpec _spec;
        
        EffectsDurationPolicy _durationPolicy;
        public EffectsDurationPolicy DurationPolicy => _durationPolicy;
        
        List<GameplayEffectModifier> _modifiers;
        
        List<GameplayEffectExecution> _executions;
        
        // -1 represents infinite duration
        float _duration;
        public float Duration => _duration;
        
        private float _period;
        public float Period => _period;

        public List<GameplayTag> NecessaryTags;
        public List<GameplayTag> RejectionTags;
        private List<GameplayCue> GameplayCues;
        
         public delegate void GameplayEffectEventHandler(GameplayEffectSpec sender);
         public event GameplayEffectEventHandler OnRemove;
         public event GameplayEffectEventHandler OnTick;
         public event GameplayEffectEventHandler OnApply;
         
         public void TriggerOnApply()
         {
             OnApply?.Invoke(_spec);
         }

         public void TriggerOnTick()
         {
             OnTick?.Invoke(_spec);
         }

         public void TriggerOnRemove()
         {
             OnRemove?.Invoke(_spec);
         }
    }
}