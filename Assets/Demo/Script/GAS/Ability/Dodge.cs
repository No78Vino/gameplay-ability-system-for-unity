﻿using Cysharp.Threading.Tasks;
using GAS.Cue;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class Dodge:AbstractAbility<AADodge>
    {
        public readonly CuePlayAnimationOfFightUnit CueDodge;
        public readonly CueVFX CueTrail;
        public readonly float PrepareTime;
        public readonly float MotionTime;
        public readonly float EndTime;
        public readonly float MotionDistance;
        public readonly CueOneShotVFX CuePowerForceVFX;
        
        public Dodge(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            CueDodge = AbilityAsset.cueDodge;
            PrepareTime = AbilityAsset.prepareTime;
            MotionTime = AbilityAsset.motionTime;
            EndTime = AbilityAsset.endTime;
            MotionDistance = AbilityAsset.motionDistance;
            CueTrail = AbilityAsset.cueTrail;
            CuePowerForceVFX = AbilityAsset.cuePowerForceVFX;
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new DodgeSpec(this, owner);
        }
    }
    
    public class DodgeSpec: AbilitySpec
    {
        private Dodge _dodge;
        private FightUnit _unit;
        public DodgeSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _dodge = ability as Dodge;
            _unit = owner.GetComponent<FightUnit>();
        }

        public override void ActivateAbility(params object[] args)
        {
            DoDodge().Forget();
        }

        public override void CancelAbility()
        {
            
        }

        public override void EndAbility()
        {
           
        }
        
        async UniTask DoDodge()
        {
            DoCost();
            var cueSpec = _dodge.CueDodge.CreateSpec(new GameplayCueParameters { sourceAbilitySpec = this });
            cueSpec.Trigger();
            
            var cuePowerForce = _dodge.CuePowerForceVFX.CreateSpec(new GameplayCueParameters { sourceAbilitySpec = this });
            cuePowerForce.Trigger();
            
            var cueTrialSpec = _dodge.CueTrail.CreateSpec(new GameplayCueParameters { sourceAbilitySpec = this });
            cueTrialSpec.OnAdd();
            await UniTask.Delay((int)(_dodge.PrepareTime * 1000));
            
            var dodgeSpeed = _dodge.MotionDistance/_dodge.MotionTime;
            var timer = _dodge.MotionTime;
            var start = _unit.transform.position;
            var endPos = start + new Vector3(_unit.Renderer.localScale.x, 0, 0) * _dodge.MotionDistance;
            while (timer > 0)
            {
                float t = 1 - timer / _dodge.MotionTime;
                _unit.Rb.MovePosition(Vector3.Lerp(start, endPos, t));
                timer -= Time.fixedDeltaTime;
                timer = Mathf.Max(0, timer);
                await UniTask.Yield();
            }
            await UniTask.Delay((int)(_dodge.MotionTime * 1000));
            cueTrialSpec.OnRemove();
            await UniTask.Delay((int)(_dodge.EndTime * 1000));
            TryEndAbility();
        }
    }
}