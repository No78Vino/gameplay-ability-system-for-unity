using System;
using Cysharp.Threading.Tasks;
using GAS.Cue;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Ability
{
    public class Attack:AbstractAbility<AAAttack>
    {
        public readonly CuePlayAnimationOfFightUnit cueAttackAnim;
        public readonly GameplayEffectAsset DirectDamageEffect;
        public readonly GameplayEffectAsset DefendedDamageEffect;
        public readonly float waitTimeForDoDamage;
        public readonly float waitTimeForEnd;
        
        public Attack(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            cueAttackAnim = AbilityAsset.cueAttackAnim;
            DirectDamageEffect = AbilityAsset.DirectDamageEffect;
            DefendedDamageEffect = AbilityAsset.DefendedDamageEffect;
            waitTimeForDoDamage = AbilityAsset.waitTimeForDoDamage;
            waitTimeForEnd = AbilityAsset.waitTimeForEnd;
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new AttackSpec(this, owner);
        }
    }
    
    public class AttackSpec: AbilitySpec
    {
        private Attack _attack;
        public AttackSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _attack = ability as Attack;
        }

        public override void ActivateAbility(params object[] args)
        {
            DoAttack().Forget();
        }

        public override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
            CancelAbility();
        }

        async UniTask DoAttack()
        {
            var cueAttackAnimSpec =
                _attack.cueAttackAnim.CreateSpec(new GameplayCueParameters() { sourceAbilitySpec = this });
            cueAttackAnimSpec.Trigger();
            await UniTask.Delay(TimeSpan.FromSeconds(_attack.waitTimeForDoDamage));
            // TODO
            // DO DAMAGE
            await UniTask.Delay(TimeSpan.FromSeconds(_attack.waitTimeForEnd));
            TryEndAbility();
        }
    }
}