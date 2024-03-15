using System;
#if UNITY_EDITOR
using GAS.Editor;
#endif
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.Script.GAS.AbilityTask
{
    [Serializable]
    public class TeleportToBeamPoint : InstantAbilityTask
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

#if UNITY_EDITOR
    public class TeleportToBeamPointInspector : InstantTaskInspector<TeleportToBeamPoint>
    {
        [Delayed] [LabelText("左侧发射激光位点")] [OnValueChanged("OnBeamPointLeftChanged")]
        public Vector2 BeamPointLeft;

        [Delayed] [LabelText("右侧发射激光位点")] [OnValueChanged("OnBeamPointRightChanged")]
        public Vector2 BeamPointRight;

        public override void Init(InstantAbilityTask task)
        {
            base.Init(task);
            BeamPointLeft = _task.BeamPointLeft;
            BeamPointRight = _task.BeamPointRight;
        }

        void OnBeamPointLeftChanged()
        {
            _task.BeamPointLeft = BeamPointLeft;
            Save();
        }

        void OnBeamPointRightChanged()
        {
            _task.BeamPointRight = BeamPointRight;
            Save();
        }
    }
#endif
}