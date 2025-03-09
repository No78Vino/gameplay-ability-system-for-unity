namespace GAS.RuntimeWithECS.Ability.Component
{
    public abstract class AbilityLogicBase
    {
        public abstract void ActivateAbility();

        public abstract void CancelAbility();

        public abstract void EndAbility();
    }
}