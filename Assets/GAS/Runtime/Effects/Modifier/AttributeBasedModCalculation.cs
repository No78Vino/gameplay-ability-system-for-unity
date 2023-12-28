using UnityEngine;
namespace GAS.Runtime.Effects.Modifier
{
    [CreateAssetMenu(fileName = "AttributeBasedModCalculation", menuName = "GAS/MMC/AttributeBasedModCalculation")]
    public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
    {
        public enum AttributeFrom
        {
            Source,
            Target
        }

        public enum GEAttributeCaptureType
        {
            SnapShot,
            Track
        }

        [SerializeField] public string attributeName;
        [SerializeField] public string attributeSetName;
        [SerializeField] public string attributeShortName;
        [SerializeField] public AttributeFrom attributeFromType;
        [SerializeField] public GEAttributeCaptureType captureType;

        public override float CalculateMagnitude(params float[] modifierValue)
        {
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == GEAttributeCaptureType.SnapShot)
                {
                    var attribute = _spec.Source.DataSnapshot()[attributeName];
                    return attribute;
                }
                else
                {
                    var attribute = _spec.Source.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                    return attribute ?? 1;
                }
            }

            if (captureType == GEAttributeCaptureType.SnapShot)
            {
                var attribute = _spec.Owner.DataSnapshot()[attributeName];
                return attribute;
            }
            else
            {
                var attribute = _spec.Owner.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                return attribute ?? 1;
            }
        }
    }
}