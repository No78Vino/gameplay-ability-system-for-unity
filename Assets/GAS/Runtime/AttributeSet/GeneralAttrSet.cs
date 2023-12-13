using GAS.Runtime.Attribute;
using GAS.Runtime.Effects;

namespace GAS.Runtime.AttributeSet
{
    public class GeneralAttrSet:AbstractAttributeSet
    {
        protected override void PreAttributeBaseChange(AttributeBase attribute, float newValue)
        {
        }

        protected override void PostAttributeBaseChange(AttributeBase attribute, float oldValue, float newValue)
        {
        }

        protected override void PreAttributeChange(AttributeBase attribute, float newValue)
        {
        }

        protected override void PostAttributeChange(AttributeBase attribute, float oldValue, float newValue)
        {
        }

        protected override void PreGameplayEffectExecute(GameplayEffect gameplayEffect, AttributeBase attribute, float newValue)
        {
        }

        protected override void PostGameplayEffectExecute(GameplayEffect gameplayEffect, AttributeBase attribute, float oldValue, float newValue)
        {
        }
    }
}