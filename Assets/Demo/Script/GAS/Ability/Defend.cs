using GAS.Cue;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Ability
{
    public class Defend:AbstractAbility<AADefend>
    {
        public readonly CuePlayAnimationOfFightUnit cueDefendAnim;
        public Defend(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            cueDefendAnim = AbilityAsset.cueDefendAnim;
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new DefendSpec(this, owner);
        }
    }
    
    public class DefendSpec: AbilitySpec
    {
        private Defend _defend;
        FightUnit _unit;
        public DefendSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _defend = ability as Defend;
            _unit = owner.GetComponent<FightUnit>();
        }

        public override void ActivateAbility(params object[] args)
        {
            var cueDefendAnimSpec =
                _defend.cueDefendAnim.CreateSpec(new GameplayCueParameters() { sourceAbilitySpec = this });
            
            cueDefendAnimSpec.Trigger();
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