using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using GAS.Cue;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class Attack:AbstractAbility<AAAttack>
    {
        public readonly CuePlayAnimationOfFightUnit cueAttackAnim;
        public readonly GameplayEffectAsset DirectDamageEffect;
        public readonly GameplayEffectAsset DefendedDamageEffect;
        public readonly GameplayEffectAsset PerfectDefendEffect;
        public readonly float waitTimeForDoDamage;
        public readonly float waitTimeForEnd;
        
        public Attack(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            cueAttackAnim = AbilityAsset.cueAttackAnim;
            DirectDamageEffect = AbilityAsset.DirectDamageEffect;
            DefendedDamageEffect = AbilityAsset.DefendedDamageEffect;
            PerfectDefendEffect = AbilityAsset.PerfectDefendEffect;
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
        private FightUnit _unit;
        private Attack _attack;
        public AttackSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _attack = ability as Attack;
            _unit = owner.GetComponent<FightUnit>();
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

            var box = _unit.BoxAttack00;
            var targets =
                Physics2D.OverlapBoxAll(box.Center, box.Size, 0, LayerMask.GetMask("FightUnit"));

            var defendAreas =
                Physics2D.OverlapBoxAll(box.Center, box.Size, 0, LayerMask.GetMask("DefendArea"));
            
            foreach (var target in targets)
            {
                var targetUnit = target.GetComponent<FightUnit>();
                if (targetUnit)
                {
                    var defended = Defended(targetUnit, defendAreas);
                    if (defended)
                    {
                        bool isPerfectDefended = IsPerfectDefended(targetUnit);
                        if (isPerfectDefended)
                        {
                            var effect = new GameplayEffect(_attack.PerfectDefendEffect);
                            targetUnit.ASC.ApplyGameplayEffectTo(effect, Owner);
                        }
                        else
                        {
                            var effect = new GameplayEffect(_attack.DefendedDamageEffect);
                            Owner.ApplyGameplayEffectTo(effect, targetUnit.ASC);
                        }

                    }
                    else
                    {
                        var effect = new GameplayEffect(_attack.DirectDamageEffect);
                        Owner.ApplyGameplayEffectTo(effect, targetUnit.ASC);
                    }

                    
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_attack.waitTimeForEnd));
            TryEndAbility();
        }
        
        bool Defended(FightUnit target,Collider2D[] defendAreas)
        {
            if(!target.ASC.HasTag(GameplayTagSumCollection.Event_Defending)) return false;
            
            return defendAreas.Any(defendArea => target.DefendArea == defendArea);
        }

        bool IsPerfectDefended(FightUnit target)
        {
            return target.ASC.HasTag(GameplayTagSumCollection.Event_PerfectDefending);
        }
    }

}