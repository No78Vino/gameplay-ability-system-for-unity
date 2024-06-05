using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public abstract class TimelineAbilityT<T> : AbstractAbility<T> where T : TimelineAbilityAssetBase
    {
        protected TimelineAbilityT(T abilityAsset) : base(abilityAsset)
        {
        }
    }

    public abstract class TimelineAbilitySpecT<T> : AbilitySpec<T> where T : AbstractAbility
    {
        protected TimelineAbilityPlayer<T> _player;

        /// <summary>
        /// 向性技能的作用目标
        /// </summary>
        public AbilitySystemComponent Target { get; private set; }

        protected TimelineAbilitySpecT(T ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _player = new TimelineAbilityPlayer<T>(this);
        }

        public void SetAbilityTarget(AbilitySystemComponent mainTarget)
        {
            Target = mainTarget;
        }

        public override void ActivateAbility(params object[] args)
        {
            _player.Play();
        }

        public override void CancelAbility()
        {
            _player.Stop();
        }

        public override void EndAbility()
        {
            _player.Stop();
        }

        protected override void AbilityTick()
        {
            Profiler.BeginSample("TimelineAbilitySpecT<T>::AbilityTick()");
            _player.Tick();
            Profiler.EndSample();
        }
    }

    /// <summary>
    /// 这是一个最朴素的TimelineAbility实现, 如果要实现更复杂的TimelineAbility, 请用TimelineAbilityT<T>和TimelineAbilitySpecT<T>为基类
    /// </summary>
    public sealed class TimelineAbility : TimelineAbilityT<TimelineAbilityAssetBase>
    {
        public TimelineAbility(TimelineAbilityAssetBase abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new TimelineAbilitySpec(this, owner);
        }
    }

    /// <summary>
    /// 这是一个最朴素的TimelineAbilitySpec实现, 如果要实现更复杂的TimelineAbility, 请用TimelineAbilityT<T>和TimelineAbilitySpecT<T>为基类
    /// </summary>
    public sealed class TimelineAbilitySpec : TimelineAbilitySpecT<TimelineAbility>
    {
        public TimelineAbilitySpec(TimelineAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
    }
}