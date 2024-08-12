using System;
using System.Collections.Generic;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public enum EffectsDurationPolicy
    {
        [LabelText("瞬时(Instant)", SdfIconType.LightningCharge)]
        Instant = 1,

        [LabelText("永久(Infinite)", SdfIconType.Infinity)]
        Infinite,

        [LabelText("限时(Duration)", SdfIconType.HourglassSplit)]
        Duration
    }

    [Flags]
    public enum GameplayEffectSnapshotPolicy
    {
        [LabelText("禁用", SdfIconType.XCircleFill)]
        None = 0,

        [LabelText("来源", SdfIconType.Magic)]
        Source = 1 << 0,

        [LabelText("目标", SdfIconType.Person)]
        Target = 1 << 1,

        [LabelText("全部", SdfIconType.People)]
        All = Source | Target
    }

    public class GameplayEffect
    {
        public readonly string GameplayEffectName;
        public readonly EffectsDurationPolicy DurationPolicy;
        public readonly float Duration; // -1 represents infinite duration
        public readonly float Period;
        public readonly GameplayEffect PeriodExecution;
        public readonly GameplayEffectSnapshotPolicy SnapshotPolicy;
        public readonly GameplayEffectTagContainer TagContainer;

        // Cues
        public readonly GameplayCueInstant[] CueOnExecute;
        public readonly GameplayCueInstant[] CueOnRemove;
        public readonly GameplayCueInstant[] CueOnAdd;
        public readonly GameplayCueInstant[] CueOnActivate;
        public readonly GameplayCueInstant[] CueOnDeactivate;
        public readonly GameplayCueDurational[] CueDurational;

        // Modifiers
        public readonly GameplayEffectModifier[] Modifiers;
        public readonly ExecutionCalculation[] Executions; // TODO: this should be a list of execution calculations

        // Granted Ability
        public readonly GrantedAbilityFromEffect[] GrantedAbilities;

        //Stacking
        public readonly GameplayEffectStacking Stacking;

        // TODO: Expiration Effects 
        public readonly GameplayEffect[] PrematureExpirationEffect;
        public readonly GameplayEffect[] RoutineExpirationEffectClasses;

        public GameplayEffectSpec CreateSpec(
            AbilitySystemComponent creator,
            AbilitySystemComponent owner,
            float level = 1,
            object userData = null)
        {
            var spec = ObjectPool.Instance.Fetch<GameplayEffectSpec>();
            spec.Awake(this, userData);
            spec.Init(creator, owner, level);
            return spec;
        }

        /// <summary>
        /// 分离GameplayEffectSpec的实例化过程为：实例 + 数据初始化
        /// </summary>
        /// <returns></returns>
        public GameplayEffectSpec CreateSpec(object userData = null)
        {
            var spec = ObjectPool.Instance.Fetch<GameplayEffectSpec>();
            spec.Awake(this, userData);
            return spec;
        }

        public GameplayEffect(IGameplayEffectData data)
        {
            if (data is null)
            {
                throw new Exception($"GE data can't be null!");
            }

            GameplayEffectName = data.GetDisplayName();
            DurationPolicy = data.GetDurationPolicy();
            Duration = data.GetDuration();
            Period = data.GetPeriod();
            SnapshotPolicy = data.GetSnapshotPolicy();
            TagContainer = new GameplayEffectTagContainer(data);
            var periodExecutionGe = data.GetPeriodExecution();
#if UNITY_EDITOR
            if (periodExecutionGe != null && periodExecutionGe.GetDurationPolicy() != EffectsDurationPolicy.Instant)
            {
                Debug.LogError($"PeriodExecution of {GameplayEffectName} should be Instant type.");
            }
#endif
            PeriodExecution = periodExecutionGe != null ? new GameplayEffect(periodExecutionGe) : null;
            CueOnExecute = data.GetCueOnExecute();
            CueOnRemove = data.GetCueOnRemove();
            CueOnAdd = data.GetCueOnAdd();
            CueOnActivate = data.GetCueOnActivate();
            CueOnDeactivate = data.GetCueOnDeactivate();
            CueDurational = data.GetCueDurational();
            Modifiers = data.GetModifiers();
            Executions = data.GetExecutions();
            GrantedAbilities = GetGrantedAbilities(data.GetGrantedAbilities());
            Stacking = data.GetStacking();
        }

        public void Release()
        {
            PeriodExecution?.Release();
            GrantedAbilityFromEffectArrayPool.Recycle(GrantedAbilities);
        }

        private static readonly ArrayPool<GrantedAbilityFromEffect> GrantedAbilityFromEffectArrayPool = new();

        private static GrantedAbilityFromEffect[] GetGrantedAbilities(IReadOnlyCollection<GrantedAbilityConfig> grantedAbilities)
        {
            if (grantedAbilities.Count == 0)
            {
                return Array.Empty<GrantedAbilityFromEffect>();
            }

            var grantedAbilityFromEffects = ObjectPool.Instance.Fetch<List<GrantedAbilityFromEffect>>();
            foreach (var grantedAbilityConfig in grantedAbilities)
            {
                if (grantedAbilityConfig.AbilityAsset != null)
                    grantedAbilityFromEffects.Add(new GrantedAbilityFromEffect(grantedAbilityConfig));
            }

            var ret = GrantedAbilityFromEffectArrayPool.Fetch(grantedAbilityFromEffects.Count);
            grantedAbilityFromEffects.CopyTo(ret);
            grantedAbilityFromEffects.Clear();
            ObjectPool.Instance.Recycle(grantedAbilityFromEffects);
            return ret;
        }

        public bool CanApplyTo(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }

        public bool CanRunning(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.OngoingRequiredTags);
        }

        public bool IsImmune(IAbilitySystemComponent target)
        {
            return target.HasAnyTags(TagContainer.ApplicationImmunityTags);
        }

        public bool StackEqual(GameplayEffect effect)
        {
            if (Stacking.stackingType == StackingType.None) return false;
            if (effect.Stacking.stackingType == StackingType.None) return false;
            if (string.IsNullOrEmpty(Stacking.stackingCodeName)) return false;
            if (string.IsNullOrEmpty(effect.Stacking.stackingCodeName)) return false;

            return Stacking.stackingHashCode == effect.Stacking.stackingHashCode;
        }
    }
}