using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Effects
{
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "GAS/GameplayEffect")]
    public class GameplayEffectAsset : ScriptableObject
    {
        private const string GRP_BASE = "Base Info";
        private const string GRP_BASE_H = "Base Info/H";
        private const string GRP_BASE_H_LEFT = "Base Info/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base Info/H/Right";
        
        
        [BoxGroup(GRP_BASE)]
        [HorizontalGroup(GRP_BASE_H,Width = 200)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Name;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Description;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        public EffectsDurationPolicy DurationPolicy;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ShowIf("DurationPolicy",EffectsDurationPolicy.Duration)]
        public float Duration;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ShowIf("DurationPolicy",EffectsDurationPolicy.Duration)]
        [ShowIf("DurationPolicy",EffectsDurationPolicy.Infinite)]
        public float Period;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ShowIf("IsPeriodic")]
        public GameplayEffectAsset PeriodExecution;

        // Tag Container
        public GameplayTag[] AssetTags;
        public GameplayTag[] GrantedTags;
        public GameplayTag[] ApplicationRequiredTags;
        public GameplayTag[] OngoingRequiredTags;
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        // Cues
        public GameplayCueInstant[] CueOnExecute;
        public GameplayCueInstant[] CueOnRemove;
        public GameplayCueInstant[] CueOnAdd;
        public GameplayCueInstant[] CueOnActivate;
        public GameplayCueInstant[] CueOnDeactivate;
        public GameplayCueDurational[] CueDurational;

        public GameplayEffectModifier[] Modifiers;


        // TODO
        [HideInInspector]
        public ExecutionCalculation[] Executions;
        
        
        
        
        
        
        
        
        bool IsPeriodic()
        {
            return Period > 0;
        }
    }
}