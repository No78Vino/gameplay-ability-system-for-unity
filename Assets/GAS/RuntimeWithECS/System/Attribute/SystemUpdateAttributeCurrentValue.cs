using GAS.RuntimeWithECS.AttributeSet.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Attribute
{
    public partial class SystemUpdateAttributeCurrentValue : SystemBase
    {
        private EntityQuery _query;
        protected override void OnCreate()
        {
            _query = GetEntityQuery(typeof(TagAttributeDirty));
            RequireForUpdate(_query);    
        }
        
        protected override void OnUpdate()
        {
            
            // float newValue = _processedAttribute.BaseValue;
            // foreach (var tuple in _modifierCache)
            // {
            //     var spec = tuple.Item1;
            //     var modifier = tuple.Item2;
            //     var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
            //
            //     if (_processedAttribute.IsSupportOperation(modifier.Operation) == false)
            //     {
            //         throw new InvalidOperationException("Unsupported operation.");
            //     }
            //
            //     switch (modifier.Operation)
            //     {
            //         case GEOperation.Add:
            //             newValue += magnitude;
            //             break;
            //         case GEOperation.Minus:
            //             newValue -= magnitude;
            //             break;
            //         case GEOperation.Multiply:
            //             newValue *= magnitude;
            //             break;
            //         case GEOperation.Divide:
            //             newValue /= magnitude;
            //             break;
            //         case GEOperation.Override:
            //             newValue = magnitude;
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // }

            
            
        }
    }
}