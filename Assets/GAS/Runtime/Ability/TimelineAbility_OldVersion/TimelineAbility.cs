using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public abstract class TimelineAbilityT<T> 
    {
        // protected TimelineAbilityT(T abilityAsset) : base(abilityAsset)
        // {
        // }
    }

    public abstract class TimelineAbilitySpecT<T> 
    {
        protected TimelineAbilityPlayer<T> _player;

        /// <summary>
        /// 向性技能的作用目标
        /// </summary>
        public AbilitySystemCellMono Target { get; private set; }

        protected TimelineAbilitySpecT(T ability, AbilitySystemCellMono owner)
        {
            //_player = new TimelineAbilityPlayer<T>(this);
        }

        public void SetAbilityTarget(AbilitySystemCellMono mainTarget)
        {
            Target = mainTarget;
        }

        public  void ActivateAbility(params object[] args)
        {
            _player.Play();
        }

        public  void CancelAbility()
        {
            _player.Stop();
        }

        public  void EndAbility()
        {
            _player.Stop();
        }

        protected  void AbilityTick()
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
        public TimelineAbility(TimelineAbilityAssetBase abilityAsset) //: base(abilityAsset)
        {
        }
    }

    /// <summary>
    /// 这是一个最朴素的TimelineAbilitySpec实现, 如果要实现更复杂的TimelineAbility, 请用TimelineAbilityT<T>和TimelineAbilitySpecT<T>为基类
    /// </summary>
    public sealed class EdtTimelineAbilitySpec : TimelineAbilitySpecT<TimelineAbility>
    {
        public EdtTimelineAbilitySpec(TimelineAbility ability, AbilitySystemCellMono owner) : base(ability, owner)
        {
        }
    }
}