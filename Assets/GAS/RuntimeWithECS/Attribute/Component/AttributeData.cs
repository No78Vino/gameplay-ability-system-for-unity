using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct AttributeData:IComponentData
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
        public bool Dirty;
        public bool TriggerCueEvent;
        
        public static readonly AttributeData NULL = new()
        {
            Code = -1
        };
    }
}