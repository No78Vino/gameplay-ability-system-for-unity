using System.Collections.Generic;
using GAS.General;
using UnityEngine;
//using UnityEngine.Profiling;

namespace GAS.Runtime
{
    internal abstract class RuntimeClipInfo
    {
        public int endFrame;
        public int startFrame;
    }

    internal class RuntimeDurationCueClip : RuntimeClipInfo
    {
        public GameplayCueDurationalSpec cueSpec;
    }

    internal class RuntimeBuffClip : RuntimeClipInfo
    {
        public GameplayEffect buff;
        public GameplayEffectSpec buffSpec;
    }

    internal class RuntimeTaskClip : RuntimeClipInfo
    {
        public OngoingAbilityTask task;
    }

    internal class RuntimeTaskMark
    {
        public int startFrame;
        public InstantAbilityTask task;
    }

    public class TimelineAbilityPlayer<T> where T : AbstractAbility
    {
        private readonly TimelineAbilitySpecT<T> _abilitySpec;
        private readonly List<RuntimeBuffClip> _cacheBuffGameplayEffectTrack = new();

        private readonly List<RuntimeDurationCueClip> _cacheDurationalCueTrack = new();

        private readonly List<InstantCueMarkEvent> _cacheInstantCues = new();

        private readonly List<RuntimeTaskMark> _cacheInstantTasks = new();
        private readonly List<RuntimeTaskClip> _cacheOngoingTaskTrack = new();

        private readonly List<ReleaseGameplayEffectMarkEvent> _cacheReleaseGameplayEffect = new();

        // cache for target catcher, avoid new in TickFrame
        // 这个是一个泛型类, 这个变量就不作为static了
        private readonly List<AbilitySystemComponent> _targets = new();

        private int _currentFrame;
        private float _playTotalTime;

        public TimelineAbilityPlayer(TimelineAbilitySpecT<T> abilitySpec)
        {
            _abilitySpec = abilitySpec;
            Cache();
        }

        public bool IsPlaying { get; private set; }

        public TimelineAbilityAssetBase AbilityAsset => _abilitySpec.Ability.DataReference as TimelineAbilityAssetBase;
        private int FrameCount => AbilityAsset.FrameCount;
        private int FrameRate => GASTimer.FrameRate;

        private void Cache()
        {
            Cache_InstantCues();
            Cache_ReleaseGameplayEffects();
            Cache_InstantTasks();
            Cache_DurationalGameplayCues();
            Cache_BuffGameplayEffects();
            Cache_OngoingTasks();
        }

        private void Cache_InstantCues()
        {
            _cacheInstantCues.Clear();
            foreach (var trackData in AbilityAsset.InstantCues)
            {
                _cacheInstantCues.AddRange(trackData.markEvents);
            }

            _cacheInstantCues.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
        }

        private void Cache_ReleaseGameplayEffects()
        {
            _cacheReleaseGameplayEffect.Clear();
            foreach (var trackData in AbilityAsset.ReleaseGameplayEffect)
            {
                _cacheReleaseGameplayEffect.AddRange(trackData.markEvents);
            }

            _cacheReleaseGameplayEffect.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            foreach (var releaseGameplayEffectMarkEvent in _cacheReleaseGameplayEffect)
            {
                releaseGameplayEffectMarkEvent.CacheTargetCatcher();
            }
        }

        private void Cache_InstantTasks()
        {
            _cacheInstantTasks.Clear();
            foreach (var trackData in AbilityAsset.InstantTasks)
            {
                foreach (var markEvent in trackData.markEvents)
                {
                    foreach (var taskData in markEvent.InstantTasks)
                    {
                        var runtimeTaskMark = new RuntimeTaskMark
                        {
                            startFrame = markEvent.startFrame,
                            task = taskData.CreateTask(_abilitySpec)
                        };
                        _cacheInstantTasks.Add(runtimeTaskMark);
                    }
                }
            }

            _cacheInstantTasks.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
        }

        private void Cache_DurationalGameplayCues()
        {
            _cacheDurationalCueTrack.Clear();
            foreach (var track in AbilityAsset.DurationalCues)
            {
                foreach (var clipEvent in track.clipEvents)
                {
                    var cueSpec = clipEvent.cue.ApplyFrom(_abilitySpec);
                    if (cueSpec == null) continue;
                    var runtimeDurationCueClip = new RuntimeDurationCueClip
                    {
                        startFrame = clipEvent.startFrame,
                        endFrame = clipEvent.EndFrame,
                        cueSpec = cueSpec
                    };
                    _cacheDurationalCueTrack.Add(runtimeDurationCueClip);
                }
            }
        }

        private void Cache_BuffGameplayEffects()
        {
            _cacheBuffGameplayEffectTrack.Clear();
            foreach (var track in AbilityAsset.BuffGameplayEffects)
            {
                foreach (var clipEvent in track.clipEvents)
                {
                    // 只有持续型的GameplayEffect可视作buff
                    if (clipEvent.gameplayEffect.DurationPolicy is EffectsDurationPolicy.Duration
                        or EffectsDurationPolicy.Infinite)
                    {
                        var runtimeBuffClip = new RuntimeBuffClip
                        {
                            startFrame = clipEvent.startFrame,
                            endFrame = clipEvent.EndFrame,
                            buff = new GameplayEffect(clipEvent.gameplayEffect),
                            buffSpec = null
                        };
                        _cacheBuffGameplayEffectTrack.Add(runtimeBuffClip);
                    }
                }
            }
        }

        private void Cache_OngoingTasks()
        {
            _cacheOngoingTaskTrack.Clear();
            foreach (var track in AbilityAsset.OngoingTasks)
            {
                foreach (var clip in track.clipEvents)
                {
                    var runtimeTaskClip = new RuntimeTaskClip
                    {
                        startFrame = clip.startFrame,
                        endFrame = clip.EndFrame,
                        task = clip.ongoingTask.CreateTask(_abilitySpec)
                    };
                    _cacheOngoingTaskTrack.Add(runtimeTaskClip);
                }
            }
        }


        private void Prepare()
        {
            foreach (var runtimeBuffClip in _cacheBuffGameplayEffectTrack)
            {
                runtimeBuffClip.buffSpec = null;
            }
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

            foreach (var clip in _cacheDurationalCueTrack)
            {
                if(_currentFrame <= clip.endFrame)
                    clip.cueSpec.OnRemove();
            }

            foreach (var clip in _cacheBuffGameplayEffectTrack)
            {
                if (clip.buffSpec != null)
                    _abilitySpec.Owner.RemoveGameplayEffect(clip.buffSpec);
            }

            foreach (var clip in _cacheOngoingTaskTrack)
            {
                clip.task.OnEnd(clip.endFrame);
            }

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
        /// 播放结束
        /// </summary>
        private void OnPlayEnd()
        {
            IsPlaying = false;

            if (!AbilityAsset.manualEndAbility)
                _abilitySpec.TryEndAbility();

        }

        /// <summary>
        /// 当前帧的事件
        /// </summary>
        /// <param name="frame"></param>
        private void TickFrame(int frame)
        {
            TickFrame_InstantGameplayCues(frame);
            TickFrame_ReleaseGameplayEffects(frame);
            TickFrame_InstantTasks(frame);
            TickFrame_DurationalGameplayCues(frame);
            TickFrame_BuffGameplayEffects(frame);
            TickFrame_OngoingTasks(frame);
        }

        private void TickFrame_InstantGameplayCues(int frame)
        {
            foreach (var cueMark in _cacheInstantCues)
            {
                if (frame == cueMark.startFrame)
                {
                    foreach (var cue in cueMark.cues)
                    {
                        cue.ApplyFrom(_abilitySpec);
                    }
                }
            }
        }

        private void TickFrame_ReleaseGameplayEffects(int frame)
        {
            foreach (var mark in _cacheReleaseGameplayEffect)
            {
                if (frame == mark.startFrame)
                {
                    var catcher = mark.TargetCatcher;
                    catcher.Init(_abilitySpec.Owner);

                    catcher.CatchTargetsNonAllocSafe(_abilitySpec.Target, _targets);

                    foreach (var asc in _targets)
                    {
                        foreach (var gea in mark.gameplayEffectAssets)
                        {
                            var ge = new GameplayEffect(gea);
                            _abilitySpec.Owner.ApplyGameplayEffectTo(ge, asc);
                        }
                    }

                    _targets.Clear();
                }
            }
        }

        private void TickFrame_InstantTasks(int frame)
        {
            foreach (var instantTask in _cacheInstantTasks)
            {
                if (frame == instantTask.startFrame)
                {
                    instantTask.task.OnExecute();
                }
            }
        }

        private void TickFrame_DurationalGameplayCues(int frame)
        {
            foreach (var cueClip in _cacheDurationalCueTrack)
            {
                if (frame == cueClip.startFrame)
                {
                    cueClip.cueSpec.OnAdd();
                }

                if (frame >= cueClip.startFrame && frame <= cueClip.endFrame)
                {
                    cueClip.cueSpec.OnTick();
                }

                if (frame == cueClip.endFrame)
                {
                    cueClip.cueSpec.OnRemove();
                }
            }
        }

        private void TickFrame_BuffGameplayEffects(int frame)
        {
            // buff持续时间以Timeline配置时间为准（执行策略全部改为Infinite）
            // Profiler.BeginSample("TickFrame_BuffGameplayEffects");
            // {
                foreach (var buffClip in _cacheBuffGameplayEffectTrack)
                {
                    if (frame == buffClip.startFrame)
                    {
                        //Profiler.BeginSample("buffGameplayEffect.Start");
                        var buffSpec = _abilitySpec.Owner.ApplyGameplayEffectToSelf(buffClip.buff);
                        buffSpec.SetDurationPolicy(EffectsDurationPolicy.Infinite);
                        buffClip.buffSpec = buffSpec;
                        //Profiler.EndSample();
                    }

                    if (frame == buffClip.endFrame)
                    {
                        if (buffClip.buffSpec != null)
                        {
                            //Profiler.BeginSample("buffGameplayEffect.End");
                            _abilitySpec.Owner.RemoveGameplayEffect(buffClip.buffSpec);
                            //Profiler.EndSample();
                        }

                        buffClip.buffSpec = null;
                    }
                }
            // }
            // Profiler.EndSample();
        }

        private void TickFrame_OngoingTasks(int frame)
        {
            // Profiler.BeginSample("TickFrame_OngoingTasks");
            // {
                foreach (var taskClip in _cacheOngoingTaskTrack)
                {
                    if (frame == taskClip.startFrame)
                    {
                        //Profiler.BeginSample("Ongoing Task.OnStart()");
                        taskClip.task.OnStart(frame);
                        //Profiler.EndSample();
                    }

                    if (frame >= taskClip.startFrame && frame <= taskClip.endFrame)
                    {
                        //Profiler.BeginSample("Ongoing Task.OnTick()");
                        taskClip.task.OnTick(frame, taskClip.startFrame, taskClip.endFrame);
                        //Profiler.EndSample();
                    }

                    if (frame == taskClip.endFrame)
                    {
                        //Profiler.BeginSample("Ongoing Task.OnEnd()");
                        taskClip.task.OnEnd(frame);
                        //Profiler.EndSample();
                    }
                }
            // }
            // Profiler.EndSample();
        }
    }
}