namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedInt : SharedVariable<int>
    {
        public static implicit operator SharedInt(int value) { return new SharedInt { mValue = value }; }
    }
}