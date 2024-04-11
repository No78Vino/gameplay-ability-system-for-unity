namespace GAS.Runtime
{
    public abstract class AttributeSet
    {
        protected AbilitySystemComponent _owner;
        
        public abstract AttributeBase this[string key] { get; }
        public abstract string[] AttributeNames { get; }
        public abstract void SetOwner(AbilitySystemComponent owner);
        public void ChangeAttributeBase(string attributeShortName, float value)
        {
            if (this[attributeShortName] != null)
            {
                this[attributeShortName].SetBaseValue(value);
            }
        }
    }
}