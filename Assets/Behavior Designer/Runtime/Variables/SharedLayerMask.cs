namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedLayerMask : SharedVariable<UnityEngine.LayerMask>
    {
        public static implicit operator SharedLayerMask(UnityEngine.LayerMask value) { return new SharedLayerMask { Value = value }; }
    }
}