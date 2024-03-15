using BehaviorDesigner.Runtime;
using GAS.Runtime;

namespace Demo.Script.GAS.AbilityTask
{
    public class DisableBehaviorTree:InstantAbilityTask
    {
        public override void OnExecute()
        {
            var bt = _spec.Owner.GetComponent<BehaviorTree>();
            if(bt) bt.DisableBehavior();
        }
    }
}