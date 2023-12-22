using Cysharp.Threading.Tasks;

namespace GAS.Runtime.Ability.AbilityTask
{
    public abstract class AsyncAbilityTask:AbstractAbilityTask
    {
        public override void Execute(params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public abstract UniTask ExecuteAsync(params object[] args);
    }
}