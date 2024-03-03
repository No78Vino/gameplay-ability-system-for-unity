using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    public class DodgeTask:OngoingAbilityTask<DodgeTaskSpec>
    {
        [SerializeField] private float _dodgeDistance;
        public float DodgeDistance => _dodgeDistance;
        public override OngoingAbilityTaskSpec CreateBaseSpec(AbilitySpec abilitySpec)
        {
            return CreateSpec(abilitySpec);
        }
    }
    
    public class DodgeTaskSpec:OngoingAbilityTaskSpec
    {
        private Vector3 _start;
        private FightUnit _unit;
        private DodgeTask _dodgeTask;
        
        public override void Init(AbilityTaskBase taskAsset,AbilitySpec spec)
        {
            base.Init(taskAsset,spec);
            _dodgeTask = taskAsset as DodgeTask;
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
            endPos.x+= Mathf.Sign(_unit.Renderer.localScale.x) * _dodgeTask.DodgeDistance;
            
            float t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _unit.Rb.MovePosition(Vector3.Lerp(_start, endPos, t));
        }
    }
}