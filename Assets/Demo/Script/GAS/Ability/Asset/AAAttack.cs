using System;
using GAS.Cue;
using GAS.Runtime.Effects;
using Sirenix.OdinInspector;

namespace GAS.Runtime.Ability
{
    public class AAAttack:AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(Attack);
        }

        [BoxGroup]
        [LabelText("攻击动画")]
        [LabelWidth(100)]
        [AssetSelector]
        public CuePlayAnimationOfFightUnit cueAttackAnim;
        
        [BoxGroup]
        [LabelText("直接伤害效果")]
        [LabelWidth(100)]
        [AssetSelector]
        public GameplayEffectAsset DirectDamageEffect;
        
        [BoxGroup]
        [LabelText("被防御效果")]
        [LabelWidth(100)]
        [AssetSelector]
        public GameplayEffectAsset DefendedDamageEffect;
        
        [BoxGroup]
        [LabelText("等待攻击生效时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        public float waitTimeForDoDamage;

        [BoxGroup]
        [LabelText("等待攻击结束时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        public float waitTimeForEnd;
    }
}