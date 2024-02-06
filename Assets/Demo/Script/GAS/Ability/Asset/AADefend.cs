using System;

namespace GAS.Runtime.Ability
{
    public class AADefend:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Defend);
        }
    }
}