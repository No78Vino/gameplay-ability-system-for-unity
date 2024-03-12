#if UNITY_EDITOR
namespace GAS.Editor.General.VisualElement
{
    using UnityEngine.UIElements;

    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }
    }
}
#endif