using System;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class GeneralSequentialAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(GeneralSequentialAbility);
        }

        [HideInInspector]
        public int MaxFrameCount;
    }
}