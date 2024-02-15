using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class BossAttack00:AbstractAbility<AABossAttack00>
    {
        public BossAttack00(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new BossAttack00Spec(this, owner);
        }
    }
    
    public class BossAttack00Spec: AbilitySpec
    {
        public BossAttack00Spec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
        }

        public override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
            CancelAbility();
        }
    }
}