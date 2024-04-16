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
        static void Update()
        {
            if (isFocused != InternalEditorUtility.isApplicationActive)
            {
                isFocused = InternalEditorUtility.isApplicationActive;
                if (isFocused)
                {
                    GASSettingAsset.LoadOrCreate();
                    GameplayTagsAsset.LoadOrCreate();
                    AttributeAsset.LoadOrCreate();
                    AttributeSetAsset.LoadOrCreate();
                    OnEditorFocused?.Invoke();
                }
            }
        }
    }
}
#endif