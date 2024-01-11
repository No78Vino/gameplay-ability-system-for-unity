#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using UnityEditor;
    using UnityEngine;
    public class GeneralAssetFloatInspector : EditorWindow
    {
        private Object _asset;
        private UnityEditor.Editor _floatInspector;
        
        public static void Open(Object asset)
        {
            GeneralAssetFloatInspector window = GetWindow<GeneralAssetFloatInspector>("Custom Editor");
            window.Initialize(asset);
            window.Show();
        }

        void Initialize(Object asset)
        {
            _asset = asset;
            if (_asset != null)
            {
                // 创建一个 Editor 并指定要编辑的资源或对象
                _floatInspector = UnityEditor.Editor.CreateEditor(_asset);
            }
        }

        private void OnGUI()
        {
            if (_floatInspector != null)
            {
                _floatInspector.OnInspectorGUI();
            }
        }

        private void OnDestroy()
        {
            if (_floatInspector != null)
            {
                DestroyImmediate(_floatInspector);
            }
        }
    }
}
#endif