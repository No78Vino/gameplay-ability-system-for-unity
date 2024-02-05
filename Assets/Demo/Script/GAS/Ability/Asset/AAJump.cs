using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class AAJump:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Jump);
        }

        [BoxGroup]
        [LabelText("跳跃力")]
        [LabelWidth(100)]
        [Range(1,500)]
        public float JumpPower;
    }
}