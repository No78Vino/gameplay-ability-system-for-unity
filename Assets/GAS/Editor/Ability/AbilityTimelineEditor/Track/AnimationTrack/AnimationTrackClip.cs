using GAS.Runtime.Ability;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.AnimationTrack
{
    public class AnimationTrackClip : TrackClip<AnimationTrack>
    {
        private AnimationClipEvent AnimationClipEvent => clipData as AnimationClipEvent;
        private VisualElement AnimOverLine => ClipVe.OverLine;

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            // clip 文本
            ItemLabel.text = AnimationClipEvent.Clip.name;

            // 动画Clip结束线
            var clipFrameCount = (int)(AnimationClipEvent.Clip.length * AnimationClipEvent.Clip.frameRate);
            if (clipFrameCount > AnimationClipEvent.durationFrame)
            {
                AnimOverLine.style.display = DisplayStyle.None;
            }
            else
            {
                AnimOverLine.style.display = DisplayStyle.Flex;

                var overLinePos = AnimOverLine.transform.position;
                overLinePos.x = clipFrameCount * FrameUnitWidth - 1;
                AnimOverLine.transform.position = overLinePos;
            }

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            int index = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData.IndexOf(clipData as AnimationClipEvent);
            if (index != -1)
            {
                AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData[index].startFrame = newStartFrame;
                AbilityTimelineEditorWindow.Instance.Save();
                clipData = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData[index];
            }
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            int index = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData.IndexOf(clipData as AnimationClipEvent);
            if (index != -1)
            {
                AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData[index].durationFrame = newDurationFrame;
                AbilityTimelineEditorWindow.Instance.Save();
                clipData = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData[index];
            }
        }

        #region Inspector

        private ObjectField clip;
        private Label startFrame;
        private Label endFrame;
        private IntegerField duration;
        private FloatField transition;

        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            // 动画Clip
            clip = new ObjectField("动画资源");
            clip.style.display = DisplayStyle.Flex;
            clip.objectType = typeof(AnimationClip);
            clip.value = AnimationClipEvent.Clip;
            clip.RegisterValueChangedCallback(OnClipChanged);
            inspector.Add(clip);

            // 起始
            startFrame = new Label($"起始帧:{StartFrameIndex}f/{StartFrameIndex * Time.fixedDeltaTime}s");
            startFrame.style.display = DisplayStyle.Flex;
            inspector.Add(startFrame);
            // 结束
            endFrame = new Label(
                $"结束帧:{AnimationClipEvent.EndFrame}f/{AnimationClipEvent.EndFrame * Time.fixedDeltaTime}s");
            endFrame.style.display = DisplayStyle.Flex;
            inspector.Add(endFrame);

            // 时长
            duration = new IntegerField("时长(f)");
            duration.value = AnimationClipEvent.durationFrame;
            duration.isDelayed = true;
            duration.RegisterValueChangedCallback(OnDurationChanged);
            duration.style.display = DisplayStyle.Flex;
            inspector.Add(duration);

            // 过渡
            transition = new FloatField("过渡时间");
            transition.style.display = DisplayStyle.Flex;
            transition.value = AnimationClipEvent.TransitionTime;
            transition.isDelayed = true;
            transition.RegisterValueChangedCallback(OnTransitionChanged);
            inspector.Add(transition);

            // 删除按钮
            var delete = new Button(Delete);
            delete.text = "删除动画";
            delete.style.backgroundColor = new Color(0.8f, 0.1f, 0.1f, 0.5f);
            inspector.Add(delete);

            return inspector;
        }

        private void OnDurationChanged(ChangeEvent<int> evt)
        {
            var max = AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount - StartFrameIndex;
            foreach (var clipEvent in track.AbilityAnimationData.animationClipData)
            {
                if (StartFrameIndex >= clipEvent.startFrame) continue;
                var length = Mathf.Max(1,clipEvent.startFrame - StartFrameIndex);
                max = Mathf.Min(max, length);
            }
            var newDuration = Mathf.Clamp(evt.newValue,1,max);
            AnimationClipEvent.durationFrame = newDuration;
            AbilityTimelineEditorWindow.Instance.Save();
            duration.value = newDuration;
            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData.Remove(
                AnimationClipEvent);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        private void OnTransitionChanged(ChangeEvent<float> evt)
        {
            var newTransition = Mathf.Max(0, evt.newValue);
            AnimationClipEvent.TransitionTime = newTransition;
            AbilityTimelineEditorWindow.Instance.Save();
            transition.value = newTransition;
            RefreshShow(FrameUnitWidth);
        }

        private void OnClipChanged(ChangeEvent<Object> evt)
        {
            var animClip = evt.newValue as AnimationClip;
            if (animClip != null)
            {
                AnimationClipEvent.Clip = animClip;
                AnimationClipEvent.durationFrame = (int)(animClip.length * animClip.frameRate);
                AbilityTimelineEditorWindow.Instance.Save();
                RefreshShow(FrameUnitWidth);
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", "动画资源不可为空！", "确定");
                RefreshShow(FrameUnitWidth);
            }
        }

        #endregion
    }
}