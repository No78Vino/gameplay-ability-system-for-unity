using System;
using GAS.Editor;
#if UNITY_EDITOR
#endif
using GAS.Runtime.Ability;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    [Serializable]
    public class DodgeTask : OngoingAbilityTask
    {
        [SerializeField] private float _dodgeDistance;

        private Vector3 _start;
        private FightUnit _unit;
        public float DodgeDistance => _dodgeDistance;

        public void SetDodgeDistance(float distance)
        {
            _dodgeDistance = distance;
        }

        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _unit = _spec.Owner.GetComponent<FightUnit>();
        }

        public override void OnStart(int startFrame)
        {
            _start = _spec.Owner.transform.position;
        }

        public override void OnEnd(int endFrame)
        {
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            var endPos = _start;
            endPos.x += Mathf.Sign(_unit.Renderer.localScale.x) * DodgeDistance;

            var t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _unit.Rb.MovePosition(Vector3.Lerp(_start, endPos, t));

            if (_unit is Player) Debug.Log(" [EX-Test] Player is Timeline Ability Running");
        }
    }
#if UNITY_EDITOR
    
    public class DodgeTaskInspector : OngoingTaskInspector<DodgeTask>
    {
        [Delayed]
        [LabelText("瞬步距离")]
        [OnValueChanged("OnDodgeDistanceChanged")]
        public float DodgeDistance;

        public override void Init(OngoingAbilityTask task)
        {
            base.Init(task);
            DodgeDistance = _task.DodgeDistance;
        }
        
        void OnDodgeDistanceChanged()
        {
            _task.SetDodgeDistance(DodgeDistance);
            Save();
        }
    }
    
    // public class DodgeTaskInspector : OngoingAbilityTaskInspector<DodgeTask>
    // {
    //     private Vector3? _start;
    //
    //     public DodgeTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
    //     {
    //     }
    //
    //     public override VisualElement Inspector()
    //     {
    //         var inspector = TrackInspectorUtil.CreateSonInspector(false);
    //
    //         var label = TrackInspectorUtil.CreateLabel("Dodge Task");
    //         inspector.Add(label);
    //
    //         var dodgeDistance = TrackInspectorUtil.CreateFloatField("Dodge Distance", _task.DodgeDistance, evt =>
    //         {
    //             _task.SetDodgeDistance(evt.newValue);
    //             Save();
    //         });
    //
    //         inspector.Add(dodgeDistance);
    //
    //         return inspector;
    //     }
    //
    //     public override void OnEditorPreview(int frame, int startFrame, int endFrame)
    //     {
    //         // _start ??= AbilityTimelineEditorWindow.Instance.PreviewObject.transform.position;
    //         // var endPos = _start.Value;
    //         // endPos.x+=  _task.DodgeDistance;
    //         // float t = (float)(frame - startFrame) / (endFrame - startFrame);
    //         // AbilityTimelineEditorWindow.Instance.PreviewObject.transform.position = (Vector3.Lerp(_start.Value, endPos, t));
    //     }
    // }
#endif
}