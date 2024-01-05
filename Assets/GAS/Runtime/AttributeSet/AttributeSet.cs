using GAS.Runtime.Attribute;

namespace GAS.Runtime.AttributeSet
{
    public abstract class AttributeSet
    {
        public abstract AttributeBase this[string key] { get; }
        public abstract string[] AttributeNames { get; }

        public void ChangeAttributeBase(string attributeShortName, float value)
        {
            if (this[attributeShortName] != null)
            {
                this[attributeShortName].SetBaseValue(value);
            }
        }
    }
}