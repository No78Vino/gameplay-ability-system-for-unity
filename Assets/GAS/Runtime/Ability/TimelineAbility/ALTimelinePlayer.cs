using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    internal abstract class RuntimeClipInfo
    {
        public int endFrame;
        public int startFrame;
    }

    internal class RuntimeTaskClip : RuntimeClipInfo
    {
        public AbilityTaskBase task;
    }


    public class ALTimelinePlayer
    {
        private readonly ALTimeline _alTimeline;

        private readonly List<RuntimeTaskClip> _cacheTaskTrack = new();

        private int _currentFrame;
        private float _playTotalTime;

        public ALTimelinePlayer(ALTimeline alTimeline)
        {
            _alTimeline = alTimeline;
        }

        public bool IsPlaying { get; private set; }
        
        public XParamTimeline Param => _alTimeline.Data;
            
        private int LifeTime => Param.LifeTime;
        private int FrameRate => GASTimer.FrameRate;

        public void InitData()
        {
            _cacheTaskTrack.Clear();
            foreach (var track in Param.Tracks)
            foreach (var clip in track.TaskClips)
            {
                var runtimeTaskClip = new RuntimeTaskClip
                {
                    startFrame = clip.StartTime,
                    endFrame = clip.EndTime,
                    task = clip.InstantiateTask(_alTimeline)
                };
                _cacheTaskTrack.Add(runtimeTaskClip);
            }
        }
        
        private void Prepare()
        {
        }

        public void Play()
        {
            _currentFrame = -1; // 为了播放第0帧
            _playTotalTime = 0;
            IsPlaying = true;
            Prepare();
        }

        public void Stop()
        {
            if (!IsPlaying) return;

            foreach (var clip in _cacheTaskTrack) clip.task.Finish(clip.endFrame);

            IsPlaying = false;
        }

        public void Tick()
        {
            if (!IsPlaying) return;

            _playTotalTime += Time.deltaTime;
            var targetFrame = (int)(_playTotalTime * FrameRate);

            // 追帧
            while (_currentFrame < targetFrame)
            {
                _currentFrame++;
                TickFrame(_currentFrame);
            }

            if (_currentFrame >= LifeTime)
            {
                _currentFrame++; //确保不重复触发cue的onRemove
                OnPlayEnd();
            }
        }

        /// <summary>
        ///     播放结束
        /// </summary>
        private void OnPlayEnd()
        {
            IsPlaying = false;

            if (!Param.ManualEndAbility)
                _alTimeline.TryEndSelf();
        }

        /// <summary>
        ///     当前帧的事件
        /// </summary>
        /// <param name="frame"></param>
        private void TickFrame(int frame)
        {
            foreach (var taskClip in _cacheTaskTrack)
            {
                if (frame == taskClip.startFrame)
                    taskClip.task.Begin(frame);
                
                if (frame >= taskClip.startFrame && frame <= taskClip.endFrame)
                    taskClip.task.Tick(frame);
                
                if (frame == taskClip.endFrame)
                    taskClip.task.Finish(frame);
            }
        }
    }
}