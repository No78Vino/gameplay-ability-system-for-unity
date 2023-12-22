using System.Collections.Generic;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using UnityEngine;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectAsset:ScriptableObject
    {
        public string Name;
        public EffectsDurationPolicy DurationPolicy;
        public float Duration;
        public float Period;
        public GameplayEffectTagContainer TagContainer;
        
        // Cues
        public List<GameplayCue> CueOnExecute;
        public List<GameplayCue> CueOnRemove;
        public List<GameplayCue> CueOnAdd;

        public  List<GameplayEffectModifier> Modifiers;
        public  List<GameplayEffectExecution> _executions;
        
        public GameplayEffectAsset PeriodExecution;
    }
}