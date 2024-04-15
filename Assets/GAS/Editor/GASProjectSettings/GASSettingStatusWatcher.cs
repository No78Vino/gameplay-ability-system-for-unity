#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    
    [InitializeOnLoad]
    public static class EditorStatusWatcher
    {
        public static Action OnEditorFocused;
        static bool isFocused;
        static EditorStatusWatcher() => EditorApplication.update += Update;
        static void Update()
        {
            if (isFocused != InternalEditorUtility.isApplicationActive)
            {
                isFocused = InternalEditorUtility.isApplicationActive;
                if (isFocused)
                {
                    GameplayTagsAsset.LoadOrCreate();
                    OnEditorFocused?.Invoke();
                }
            }
        }
    }
}
#endif