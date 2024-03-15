using GAS.Runtime;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    public class FreezePosition:OngoingAbilityTask
    {
        private FightUnit _unit;
        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _unit = _spec.Owner.GetComponent<FightUnit>();
        }
        
        public override void OnStart(int startFrame)
        {
            _unit.Rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public override void OnEnd(int endFrame)
        {
            _unit.Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            
        }
    }
}