using GAS.Runtime.Attribute;

namespace GAS.Runtime.AttributeSet
{
    public abstract class AttributeSet
    {
        public abstract AttributeBase this[string key] { get; }
        public readonly string[] AttributeNames;

        public void ChangeAttributeBase(string attributeFullName, float value)
        {
            if (this[attributeFullName] != null)
            {
                this[attributeFullName].SetBaseValue(value);
            }
        }
    }
}