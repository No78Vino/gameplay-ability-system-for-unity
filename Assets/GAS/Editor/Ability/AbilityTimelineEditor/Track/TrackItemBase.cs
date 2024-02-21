using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackItemBase
    {
        protected abstract string ItemAssetPath { get; }
        protected VisualElement Item;
        public virtual void Init()
        {
            Item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ItemAssetPath).Instantiate().Query().ToList()[1];
        }
    }
}
#endif