namespace GAS.Runtime.Effects.Modifier
{
    public class AttributeBasedModCalculation:ModifierMagnitudeCalculation
    {
        public enum AttributeFrom
        {
            Source,
            Target
        }
        
        public enum GEAttributeCaptureType
        {
            SnapShot,
            Track,
        }
        
        public readonly string AttributeName;
        public readonly string AttributeSetName;
        public readonly string AttributeShortName;
        public AttributeFrom AttributeFromType;
        public readonly GEAttributeCaptureType CaptureType;
        
        public AttributeBasedModCalculation(GameplayEffectSpec spec,string attributeName, AttributeFrom attributeFromType,GEAttributeCaptureType captureType) : base(spec)
        {
            AttributeName = attributeName;
            var split = attributeName.Split('.');
            AttributeSetName = split[0];
            AttributeShortName = split[1];
            AttributeFromType = attributeFromType;
            CaptureType = captureType;
        }


        public override float CalculateMagnitude(params float[] modifierValue)
        {
            if(AttributeFromType == AttributeFrom.Source)
            {
                if(CaptureType == GEAttributeCaptureType.SnapShot)
                {
                    var attribute = _spec.Source.DataSnapshot()[AttributeName];
                    return attribute;
                }
                else
                {
                    var attribute = _spec.Source.GetAttributeCurrentValue(AttributeSetName, AttributeShortName);
                    return attribute ?? 1;
                }
            }
            else
            {
                if(CaptureType == GEAttributeCaptureType.SnapShot)
                {
                    var attribute = _spec.Owner.DataSnapshot()[AttributeName];
                    return attribute;
                }
                else
                {
                    var attribute = _spec.Owner.GetAttributeCurrentValue(AttributeSetName, AttributeShortName);
                    return attribute ?? 1;
                }
            }
        }
    }
}