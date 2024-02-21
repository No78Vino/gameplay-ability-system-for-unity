using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackBase
    {
        protected VisualElement MenuParent;
        protected VisualElement TrackParent;
        protected VisualElement Track;
        protected VisualElement Menu;
        
        protected abstract string TrackAssetPath { get; }
        protected abstract string MenuAssetPath { get; }

        public TrackBase(VisualElement trackParent, VisualElement menuParent)
        {
            Init(trackParent, menuParent);
        }

        private void Init( VisualElement trackParent, VisualElement menuParent)
        {
            TrackParent = trackParent;
            MenuParent = menuParent;
            Track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];
            Menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
            TrackParent.Add(Track);
            MenuParent.Add(Menu);
        }
    }
}
#endif