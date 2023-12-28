using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using UnityEngine;

namespace GAS.Runtime.Effects
{
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "GAS/GameplayEffect")]
    public class GameplayEffectAsset:ScriptableObject
    {
        public string Name;
        public string Description;
        public EffectsDurationPolicy DurationPolicy;
        public float Duration;
        public float Period;
        public GameplayEffectAsset PeriodExecution;
        
        // Tag Container
        public string[] AssetTags;
        public string[] GrantedTags;
        public string[] ApplicationRequiredTags;
        public string[] OngoingRequiredTags;
        public string[] RemoveGameplayEffectsWithTags;
        
        // Cues
        public List<GameplayCue> CueOnExecute;
        public List<GameplayCue> CueOnRemove;
        public List<GameplayCue> CueOnAdd;

        public  List<GameplayEffectModifier> Modifiers;
        public  List<ExecutionCalculation> _executions;
    }
}