#if UNITY_EDITOR
namespace GAS.Editor
{
    using Sirenix.OdinInspector.Editor;
    public class OdinEditorWithoutHeader:OdinEditor
    {
        protected override void OnHeaderGUI()
        {
        }
    }
}
#endif