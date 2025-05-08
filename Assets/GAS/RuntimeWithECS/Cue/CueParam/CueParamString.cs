namespace GAS.Runtime
{
    public class CueParamString:ICueParameter
    {
        public string Value { get; private set; }
        
        public CueParamString(string v)
        {
            Value = v;
        }

        public void SetValue(string v)
        {
            Value = v;
        }
    }
}