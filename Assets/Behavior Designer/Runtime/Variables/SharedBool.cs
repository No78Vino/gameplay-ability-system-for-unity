namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public static implicit operator SharedBool(bool value) { return new SharedBool { mValue = value }; }
    }
}