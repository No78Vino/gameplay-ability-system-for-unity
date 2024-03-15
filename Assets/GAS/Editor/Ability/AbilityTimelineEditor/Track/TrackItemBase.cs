#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEngine.UIElements;

    
    public abstract class TrackItemBase
    {
        protected VisualElement ve;
        public VisualElement Ve => ve;
    }
}
#endif