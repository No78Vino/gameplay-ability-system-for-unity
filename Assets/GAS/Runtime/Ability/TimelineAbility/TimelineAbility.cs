using GAS.Runtime.Component;

namespace GAS.Runtime.Ability.TimelineAbility
{
    public class TimelineAbility:AbstractAbility
    {
        public TimelineAbility(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new TimelineAbilitySpec(this, owner);
        }
    }
    
    public class TimelineAbilitySpec: AbilitySpec
    {
        private TimelineAbility _ability;

        public TimelineAbilitySpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _ability = ability as TimelineAbility;
            // 必要数据初始化
        }

        public override void ActivateAbility(params object[] args)
        {
            // TODO 播放前准备工作
            
            // TODO 播放时间轴
        }

        public override void CancelAbility()
        {
            // TODO 取消时间轴播放
        }

        public override void EndAbility()
        {
            // TODO 时间轴播放结束
        }

        protected override void AbilityTick()
        {
            // TODO 时间轴播放中的逻辑
        }
    }
}