using System.Collections.Generic;
using GAS.Runtime;

namespace GAS.Runtime
{
    public abstract class AbstractAbility
    {
        public readonly string Name;

        // TODO : AbilityTask
        // public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        // public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        public GameplayEffect Cooldown { get; protected set; }

        public float CooldownTime { get; protected set; }

        public GameplayEffect Cost { get; protected set; }

        // public AbstractAbility(AbilityAsset abilityAsset)
        // {
        //     DataReference = abilityAsset;
        //
        //     Name = DataReference.UniqueName;
        //
        //
        //     CooldownTime = DataReference.CooldownTime;
        // }

        public abstract AbilitySpec CreateSpec(AbilitySystemCellMono owner);

        public void SetCooldown(GameplayEffect coolDown)
        {
            if (coolDown.DurationPolicy == EffectsDurationPolicy.Duration)
            {
                Cooldown = coolDown;
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.LogError("[EX] Cooldown must be duration policy!");
            }
#endif
        }

        public void SetCost(GameplayEffect cost)
        {
            if (cost.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                Cost = cost;
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.LogError("[EX] Cost must be instant policy!");
            }
#endif
        }
    }

    public abstract class AbstractAbility<T> : AbstractAbility// where T : AbilityAsset
    {
        //public T AbilityAsset => DataReference as T;

        // protected AbstractAbility(T abilityAsset) : base(abilityAsset)
        // {
        // }
    }
}