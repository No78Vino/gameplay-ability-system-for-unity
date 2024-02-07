using System;
using GAS.Cue;
using GAS.Runtime.Cue;
using Sirenix.OdinInspector;

namespace GAS.Runtime.Ability
{
    public class AADodge:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Dodge);
        }
        
        [BoxGroup]
        [LabelText("闪避动画")]
        [LabelWidth(100)]
        [AssetSelector]
        public CuePlayAnimationOfFightUnit cueDodge;

        [BoxGroup]
        [LabelText("拖尾轨迹特效")]
        [LabelWidth(100)]
        [AssetSelector]
        public CueVFX cueTrail;
        
        [BoxGroup]
        [LabelText("前摇时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        public float prepareTime;

        [BoxGroup]
        [LabelText("移动时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        public float motionTime;

        [BoxGroup]
        [LabelText("后摇时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        public float endTime;
        
        [BoxGroup]
        [LabelText("移动距离")]
        [LabelWidth(100)]
        public float motionDistance;
        
        [BoxGroup]
        [LabelText("闪避额外特效")]
        [LabelWidth(100)]
        [AssetSelector]
        public CueOneShotVFX cuePowerForceVFX;
    }
}