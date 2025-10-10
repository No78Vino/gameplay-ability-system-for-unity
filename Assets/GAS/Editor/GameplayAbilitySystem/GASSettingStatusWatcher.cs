#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    
    [InitializeOnLoad]
    public static class GASSettingStatusWatcher
    {
        public static Action OnEditorFocused;
        static bool isFocused;
        static GASSettingStatusWatcher() => EditorApplication.update += Update;

        private static void Update()
        {
            if (isFocused == InternalEditorUtility.isApplicationActive) return;
            isFocused = InternalEditorUtility.isApplicationActive;
            if (!isFocused) return;
            GASSettingAsset.LoadOrCreate();
            OnEditorFocused?.Invoke();
        }
    }
}
#endif