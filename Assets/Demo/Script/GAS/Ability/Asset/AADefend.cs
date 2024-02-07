using System;
using GAS.Cue;
using Sirenix.OdinInspector;

namespace GAS.Runtime.Ability
{
    public class AADefend:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Defend);
        }
        
        [BoxGroup]
        [LabelText("防御动画")]
        [LabelWidth(100)]
        [AssetSelector]
        public CuePlayAnimationOfFightUnit cueDefendAnim;
    }
}