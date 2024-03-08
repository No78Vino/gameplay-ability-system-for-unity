using GAS.General;
using GAS.Runtime.Cue;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Cue
{
    public class CueCircleAreaVisualization : GameplayCueDurational
    {
        [BoxGroup]
        [LabelText("半径")]
        public float Radius;

        [BoxGroup]
        [LabelText("可视预制体")]
        public GameObject Visualization;
        
        [BoxGroup]
        [LabelText("持续时间(f)")]
        public int Duration = 10;
        
        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueCircleAreaVisualizationSpec(this, parameters);
        }
    }

    public class CueCircleAreaVisualizationSpec : GameplayCueDurationalSpec<CueCircleAreaVisualization>
    {
        private AreaVisualization _visualization;
        private int _startFrame;
        
        public CueCircleAreaVisualizationSpec(CueCircleAreaVisualization cue, GameplayCueParameters parameters) : base(
            cue, parameters)
        {
        }
        
        public override void OnAdd()
        {
            _startFrame = GASTimer.CurrentFrameCount;
            
            var vfx = Object.Instantiate(cue.Visualization);
            vfx.transform.position = Owner.transform.position;
            _visualization = vfx.GetComponent<AreaVisualization>();
            _visualization.SetAreaSize(cue.Radius * 2);
        }

        public override void OnRemove()
        {
            Object.Destroy(_visualization.gameObject);
            
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
            _visualization.SetProgress((float)(GASTimer.CurrentFrameCount - _startFrame) / cue.Duration);
        }
    }
}