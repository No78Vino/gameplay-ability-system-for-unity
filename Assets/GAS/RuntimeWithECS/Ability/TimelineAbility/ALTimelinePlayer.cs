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

    internal class RuntimeCueClip : RuntimeClipInfo
    {
        public GameplayCueUnit cueUnit;
    }

    internal class RuntimeTaskClip : RuntimeClipInfo
    {
        public AbilityTaskBase task;
    }


    public class ALTimelinePlayer
    {
        private readonly ALTimeline _alTimeline;

        private readonly List<RuntimeCueClip> _cacheCueTrack = new();

        private readonly List<RuntimeTaskClip> _cacheTaskTrack = new();

        private int _currentFrame;
        private float _playTotalTime;

        public ALTimelinePlayer(ALTimeline alTimeline)
        {
            _alTimeline = alTimeline;
            Cache();
        }

        public bool IsPlaying { get; private set; }
        
        public AbilityParamTimeline Param => _alTimeline.GetParam();
            
        private int FrameCount => Param.FrameCount;
        private int FrameRate => GASTimer.FrameRate;

        private void Cache()
        {
            CacheGameplayCues();
            CacheTasks();
        }

        private void CacheGameplayCues()
        {
            _cacheCueTrack.Clear();
            foreach (var track in Param.Cues)
            foreach (var clipEvent in track.clipEvents)
            {
                var cueUnit = clipEvent.cue;
                cueUnit.SetSource(_alTimeline.GetAscEntity(),CueSourceType.AbilitySystemCell);
                cueUnit.Create();
                var runtimeDurationCueClip = new RuntimeCueClip
                {
                    startFrame = clipEvent.startFrame,
                    endFrame = clipEvent.EndFrame,
                    cueUnit = cueUnit
                };
                _cacheCueTrack.Add(runtimeDurationCueClip);
            }
        }

        private void CacheTasks()
        {
            _cacheTaskTrack.Clear();
            foreach (var track in Param.Tasks)
            foreach (var clip in track.clipEvents)
            {
                var runtimeTaskClip = new RuntimeTaskClip
                {
                    startFrame = clip.startFrame,
                    endFrame = clip.EndFrame,
                    task = clip.task.CreateTask(_alTimeline)
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

            foreach (var clip in _cacheCueTrack)
                if (_currentFrame <= clip.endFrame)
                    clip.cueUnit.Stop();

            foreach (var clip in _cacheTaskTrack) clip.task.OnEnd(clip.endFrame);

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

            if (_currentFrame >= FrameCount)
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
            TickFrameGameplayCues(frame);
            TickFrameTasks(frame);
        }

        private void TickFrameGameplayCues(int frame)
        {
            foreach (var cueClip in _cacheCueTrack)
            {
                if (frame == cueClip.startFrame)
                    cueClip.cueUnit.Play();

                if (frame >= cueClip.startFrame && frame <= cueClip.endFrame)
                    cueClip.cueUnit.Tick();

                if (frame == cueClip.endFrame)
                    cueClip.cueUnit.Stop();
            }
        }

        private void TickFrameTasks(int frame)
        {
            foreach (var taskClip in _cacheTaskTrack)
            {
                if (frame == taskClip.startFrame)
                    taskClip.task.OnStart(frame);
                
                if (frame >= taskClip.startFrame && frame <= taskClip.endFrame)
                    taskClip.task.OnTick(frame, taskClip.startFrame, taskClip.endFrame);
                
                if (frame == taskClip.endFrame)
                    taskClip.task.OnEnd(frame);
            }
        }
    }
}