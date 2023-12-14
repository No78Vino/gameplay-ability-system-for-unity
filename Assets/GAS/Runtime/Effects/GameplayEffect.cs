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
        public event GameplayEffectEventHandler OnExecute;
        public event GameplayEffectEventHandler OnAdd;
        public event GameplayEffectEventHandler OnActivation;
        public event GameplayEffectEventHandler OnDeactivation;

        public void TriggerOnAdd()
        {
            OnAdd?.Invoke(_spec);
        }

        public void TriggerOnExecute()
        {
            OnExecute?.Invoke(_spec);
        }

        public void TriggerOnRemove()
        {
            OnRemove?.Invoke(_spec);
        }

        public void TriggerOnActivation()
        {
            OnActivation?.Invoke(_spec);
        }
        
        public void TriggerOnDeactivation()
        {
            OnDeactivation?.Invoke(_spec);
        }
    }
}