using System;

namespace GAS.Runtime.Ability
{
    public class AADodge:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Dodge);
        }
    }
}