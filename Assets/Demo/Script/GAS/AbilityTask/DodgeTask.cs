using GAS.Editor.Ability;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.Script.GAS.AbilityTask
{
    public class DodgeTask:OngoingAbilityTask
    {
        [SerializeField] private float _dodgeDistance;
        public float DodgeDistance => _dodgeDistance;

        public void SetDodgeDistance(float distance) => _dodgeDistance = distance;
        
        private Vector3 _start;
        private FightUnit _unit;

        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _unit = _spec.Owner.GetComponent<FightUnit>();
        }
        
        public override void OnStart(int startFrame)
        {
            _start= _spec.Owner.transform.position;
        }

        public override void OnEnd(int endFrame)
        {
            
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            var endPos = _start;
            endPos.x+= Mathf.Sign(_unit.Renderer.localScale.x) * DodgeDistance;
            
            float t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _unit.Rb.MovePosition(Vector3.Lerp(_start, endPos, t));
        }
    }
    
    public class DodgeTaskInspector:OngoingAbilityTaskInspector<DodgeTask>
    {
        public DodgeTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTargetCatcherInspector();

            var label = TrackInspectorUtil.CreateLabel("Dodge Task");
            inspector.Add(label);

            var dodgeDistance = TrackInspectorUtil.CreateFloatField("Dodge Distance", _task.DodgeDistance, (evt) =>
            {
                _task.SetDodgeDistance(evt.newValue);
                Save();
            });
            
            inspector.Add(dodgeDistance);
            
            return inspector;
        }

        public override void OnTargetCatcherPreview(GameObject obj)
        {
            // TODO
        }
    }
}