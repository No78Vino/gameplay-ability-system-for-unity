namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedUInt : SharedVariable<uint>
    {
        public static implicit operator SharedUInt(uint value) { return new SharedUInt { mValue = value }; }
    }
}