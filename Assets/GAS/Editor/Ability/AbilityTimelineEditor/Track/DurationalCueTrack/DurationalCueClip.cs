using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class DurationalCueClip: TrackClip<DurationalCueTrack>
    {
        private DurationalCueClipEvent DurationalCueClipEvent => clipData as DurationalCueClipEvent;
        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            // clip 文本
            ItemLabel.text = DurationalCueClipEvent.cue ? DurationalCueClipEvent.cue.name : "【NULL】";
            
            // clip位置，宽度
            var mainPos = ve.transform.position;
            mainPos.x = StartFrameIndex * FrameUnitWidth;
            ve.transform.position = mainPos;
            ve.style.width = DurationalCueClipEvent.durationFrame * FrameUnitWidth;

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            // 动画Clip
            var cue = new ObjectField("持续性提示");
            cue.style.display = DisplayStyle.Flex;
            cue.objectType = typeof(GameplayCueDurational);
            cue.value = DurationalCueClipEvent.cue;
            cue.RegisterValueChangedCallback(evt =>
            {
                // TODO 保存数据
                // DurationalCueClipEvent.cue = evt.newValue as DurationalCue;
                // TrackBase.RefreshShow(TrackBase.FrameWidth);
            });
            inspector.Add(cue);
            
            // 开始帧
            var startFrame = new Label("开始帧");
            inspector.Add(startFrame);
            var startFrameValue = new Label(DurationalCueClipEvent.startFrame.ToString());
            inspector.Add(startFrameValue);
            
            // 结束帧
            var endFrame = new Label("结束帧");
            inspector.Add(endFrame);
            var endFrameValue = new Label(DurationalCueClipEvent.EndFrame.ToString());
            inspector.Add(endFrameValue);
            
            // 持续帧
            var duration = new IntegerField("持续时长（f）");
            duration.value = DurationalCueClipEvent.durationFrame;
            duration.RegisterValueChangedCallback(evt =>
            {
                // TODO 保存数据
                // DurationalCueClipEvent.durationFrame = evt.newValue;
                // TrackBase.RefreshShow(TrackBase.FrameWidth);
            });
            inspector.Add(duration);
            
            // 删除按钮
            var deleteButton = new Button(() =>
            {
                // TODO 删除数据
                // TrackBase.RemoveTrackItem(this);
            });
            deleteButton.text = "删除";
            inspector.Add(deleteButton);
            
            return inspector;
        }
    }
}