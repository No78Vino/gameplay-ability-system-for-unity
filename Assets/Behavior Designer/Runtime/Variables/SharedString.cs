namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedString : SharedVariable<string>
    {
        public static implicit operator SharedString(string value) { return new SharedString { mValue = value }; }
    }
}