using System;
using GAS.Cue;
using GAS.Runtime.Effects;
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
        
        [BoxGroup]
        [LabelText("完美防御效果")]
        [LabelWidth(100)]
        [AssetSelector]
        public GameplayEffectAsset PerfectDefendEffect;
    }
}