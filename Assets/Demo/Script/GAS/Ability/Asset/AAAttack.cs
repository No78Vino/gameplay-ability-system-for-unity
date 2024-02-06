using System;

namespace GAS.Runtime.Ability
{
    public class AAAttack:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Attack);
        }
    }
}