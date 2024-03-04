using GAS.Runtime.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
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
            
            var textTest = TrackInspectorUtil.CreateFloatField("test",_task.test,(evt =>
            {
                _task.test=(evt.newValue);
                Save();
            }));
            inspector.Add(textTest);
            
            return inspector;
        }

        public override void OnTargetCatcherPreview(GameObject obj)
        {
            // TODO
        }
    }
}