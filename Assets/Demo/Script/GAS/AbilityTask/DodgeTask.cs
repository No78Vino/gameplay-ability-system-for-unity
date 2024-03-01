using System;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    public class DodgeTask:OngoingAbilityTask
    {
        [SerializeField] private float _dodgeDistance;
        
        private Vector3 _start;
        private FightUnit _unit;

        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _unit = _spec.Owner.GetComponent<FightUnit>();
        }

        public override void OnStart(int startFrame)
        {
            _start= _spec.Owner.transform.position;
        }

        public override void OnEnd(int endFrame)
        {
            
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            var endPos = _start;
            endPos.x+= Mathf.Sign(_unit.Renderer.localScale.x) * _dodgeDistance;
            
            float t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _unit.Rb.MovePosition(Vector3.Lerp(_start, endPos, t));
        }
    }
}