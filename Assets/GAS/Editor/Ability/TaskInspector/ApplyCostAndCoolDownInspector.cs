#if UNITY_EDITOR

namespace GAS.Editor.Ability
{
    using GAS.Runtime.Ability;
    using UnityEngine.UIElements;
    public class ApplyCostAndCoolDownInspector:InstantAbilityTaskInspector<ApplyCostAndCoolDown>
    {
        public ApplyCostAndCoolDownInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateSonInspector(false);

            var label = TrackInspectorUtil.CreateLabel("Apply Cost And CD");
            inspector.Add(label);
            
            return inspector;
        }

        public override void OnEditorPreview()
        {
        }
    }
}

#endif