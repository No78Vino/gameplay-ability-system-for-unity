using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public abstract class TimelineAbilityT<T> : AbstractAbility<T> where T : TimelineAbilityAssetBase
    {
        protected TimelineAbilityT(T abilityAsset) : base(abilityAsset)
        {
        }
    }

    public abstract class TimelineAbilitySpecT<AbilityT, AssetT> : AbilitySpec<AbilityT> where AbilityT : TimelineAbilityT<AssetT> where AssetT : TimelineAbilityAssetBase
    {
        protected TimelineAbilityPlayer<AbilityT, AssetT> _player;

        public int FrameCount => _player.FrameCount;
        public int FrameRate => _player.FrameRate;

        /// <summary>
        /// 不受播放速率影响的总时间
        /// </summary>
        public float TotalTime => _player.TotalTime;

        /// <summary>
        /// 向性技能的作用目标
        /// </summary>
        public AbilitySystemComponent Target { get; private set; }

        protected TimelineAbilitySpecT(AbilityT ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _player = new(this);
        }

        public void SetAbilityTarget(AbilitySystemComponent mainTarget)
        {
            Target = mainTarget;
        }

        public override void ActivateAbility()
        {
            var playSpeed = GetPlaySpeed();
            _player.Play(playSpeed);
        }

        public virtual float GetPlaySpeed()
        {
            return Data.AbilityAsset.Speed;
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
    /// 这是一个最朴素的TimelineAbility实现, 如果要实现更复杂的TimelineAbility, 请用TimelineAbilityT和TimelineAbilitySpecT为基类
    /// </summary>
    public sealed class TimelineAbility : TimelineAbilityT<TimelineAbilityAsset>
    {
        public TimelineAbility(TimelineAbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new TimelineAbilitySpec(this, owner);
        }
    }

    /// <summary>
    /// 这是一个最朴素的TimelineAbilitySpec实现, 如果要实现更复杂的TimelineAbility, 请用TimelineAbilityT和TimelineAbilitySpecT为基类
    /// </summary>
    public sealed class TimelineAbilitySpec : TimelineAbilitySpecT<TimelineAbilityT<TimelineAbilityAsset>, TimelineAbilityAsset>
    {
        public TimelineAbilitySpec(TimelineAbilityT<TimelineAbilityAsset> ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
    }
}