using System;
using GAS.Editor.Ability;
using GAS.Runtime.Ability;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Demo.Script.GAS.AbilityTask
{
    [Serializable]
    public class TeleportToBeamPoint:InstantAbilityTask
    {
        public Vector2 BeamPointLeft;
        public Vector2 BeamPointRight;
        public override void OnExecute()
        {
            var unit = _spec.Owner.GetComponent<FightUnit>();
            var pos = unit.transform.position;
            bool left = Random.value > 0.5f;
            var beamPoint = left ? BeamPointLeft : BeamPointRight;
            pos.x = beamPoint.x;
            pos.y = beamPoint.y;
            unit.transform.position = pos;
            
            unit.transform.localScale = new Vector3(left ? 1 : -1, 1, 1);
        }
    }
    
    
    public class TeleportToBeamPointInspector:InstantAbilityTaskInspector<TeleportToBeamPoint>
    {
        public TeleportToBeamPointInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateSonInspector(false);
            var left = TrackInspectorUtil.CreateVector2Field("Beam Point Left", _task.BeamPointLeft,
                (oldValue,newValue) =>
                {
                    _task.BeamPointLeft = newValue;
                    Save();
                });
            inspector.Add(left);
            var right = TrackInspectorUtil.CreateVector2Field("Beam Point Right", _task.BeamPointRight,
                (oldValue,newValue) =>
                {
                    _task.BeamPointRight = newValue;
                    Save();
                });
            inspector.Add(right);
            return inspector;
        }

        public override void OnEditorPreview()
        {
        }
    }
}