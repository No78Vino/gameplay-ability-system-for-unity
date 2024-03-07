using System.Collections.Generic;
using GAS.General;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public abstract class RuntimeClipInfo
    {
        public int startFrame;
        public int endFrame;
    }
    
    public class RuntimeDurationCueClip:RuntimeClipInfo
    {
        public GameplayCueDurationalSpec cueSpec;
    }
    
    public class RuntimeBuffClip:RuntimeClipInfo
    {
        public GameplayEffect buff;
        public GameplayEffectSpec buffSpec;
    }
    
    public class TimelineAbilityPlayer
    {
        bool _isPlaying;
        public bool IsPlaying => _isPlaying;
        private readonly TimelineAbilitySpec _abilitySpec;
        public TimelineAbilityAsset AbilityAsset=>_abilitySpec.AbilityAsset;

        private int _currentFrame;
        private float _playTotalTime;
        private int FrameCount => AbilityAsset.FrameCount;
        private int FrameRate => GASTimer.FrameRate;
        
        
        private List<InstantCueMarkEvent> _cacheInstantCues;
        private List<ReleaseGameplayEffectMarkEvent> _cacheReleaseGameplayEffect;
        private List<TaskMarkEvent> _cacheInstantTasks;
        
        private List<RuntimeDurationCueClip> _cacheDurationalCueTrack;
        private List<RuntimeBuffClip> _cacheBuffGameplayEffectTrack;
        private List<TaskClipEvent> _cacheOngoingTaskTrack;
        
        public TimelineAbilityPlayer(TimelineAbilitySpec abilitySpec)
        {
            _abilitySpec = abilitySpec;
            Cache();
        }

        private void Cache()
        {
            _cacheInstantCues = new List<InstantCueMarkEvent>();
            foreach (var trackData in AbilityAsset.InstantCues)
                _cacheInstantCues.AddRange(trackData.markEvents);
            _cacheInstantCues.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));

            _cacheReleaseGameplayEffect = new List<ReleaseGameplayEffectMarkEvent>();
            foreach (var trackData in AbilityAsset.ReleaseGameplayEffect)
                _cacheReleaseGameplayEffect.AddRange(trackData.markEvents);
            _cacheReleaseGameplayEffect.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            for (int i = 0; i < _cacheReleaseGameplayEffect.Count; i++)
                _cacheReleaseGameplayEffect[i].CacheTargetCatcher();
            
            _cacheInstantTasks = new List<TaskMarkEvent>();
            foreach (var trackData in AbilityAsset.InstantTasks)
                _cacheInstantTasks.AddRange(trackData.markEvents);
            _cacheInstantTasks.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            for (int i = 0; i < _cacheInstantTasks.Count; i++)
                for (int j = 0; j < _cacheInstantTasks[i].InstantTasks.Count; j++)
                    _cacheInstantTasks[i].InstantTasks[j].Cache(_abilitySpec);
            
            _cacheDurationalCueTrack = new List<RuntimeDurationCueClip>();
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

            _cacheBuffGameplayEffectTrack = new List<RuntimeBuffClip>();
            foreach (var track in AbilityAsset.BuffGameplayEffects)
            {
                foreach (var clipEvent in track.clipEvents)
                {
                    // 只有持续型的GameplayEffect可视作buff
                    if (clipEvent.gameplayEffect.DurationPolicy is EffectsDurationPolicy.Duration or EffectsDurationPolicy.Infinite)
                    {
                        var runtimeBuffClip = new RuntimeBuffClip
                        {
                            startFrame = clipEvent.startFrame,
                            endFrame = clipEvent.EndFrame,
                            buff = new GameplayEffect(clipEvent.gameplayEffect),
                            buffSpec = null,
                        };
                        _cacheBuffGameplayEffectTrack.Add(runtimeBuffClip);
                    }
                }
            }

            _cacheOngoingTaskTrack = new List<TaskClipEvent>();
            foreach (var track in AbilityAsset.OngoingTasks)
                _cacheOngoingTaskTrack.AddRange(track.clipEvents);
            for (int i = 0; i < _cacheOngoingTaskTrack.Count; i++)
                _cacheOngoingTaskTrack[i].ongoingTask.Cache(_abilitySpec);
        }
        
        private void Prepare()
        {
            for (int i = 0; i < _cacheBuffGameplayEffectTrack.Count; i++)
            {
                _cacheBuffGameplayEffectTrack[i].buffSpec = null;
            }
        }
        
        public void Play()
        {
            _currentFrame = -1; // 为了播放第0帧
            _playTotalTime = 0;
            _isPlaying = true;
            Prepare();
        }
        
        public void Stop()
        {
            if(!_isPlaying) return;

            foreach (var clip in _cacheDurationalCueTrack) clip.cueSpec.OnRemove();

            foreach (var clip in _cacheBuffGameplayEffectTrack)
                if (clip.buffSpec != null)
                    _abilitySpec.Owner.RemoveGameplayEffect(clip.buffSpec);
            
            foreach (var clip in _cacheOngoingTaskTrack) clip.ongoingTask.Task.OnEnd(clip.EndFrame);
            
            _isPlaying = false;
        }

        public void Tick()
        {
            if (!_isPlaying) return;
            
            _playTotalTime += Time.deltaTime;
            int targetFrame = (int)(_playTotalTime * FrameRate);
            // 追帧
            while(_currentFrame < targetFrame)
            {
                _currentFrame++;
                TickFrame(_currentFrame);
            }
            if (_currentFrame >= FrameCount) OnPlayEnd();
        }

        /// <summary>
        /// 播放结束
        /// </summary>
        private void OnPlayEnd()
        {
            _isPlaying = false;
            
            if(!AbilityAsset.manualEndAbility)
                _abilitySpec.TryEndAbility();
        }

        /// <summary>
        /// 当前帧的事件
        /// </summary>
        /// <param name="frame"></param>
        private void TickFrame(int frame)
        {
            // Cue 即时
            foreach (var cueMark in _cacheInstantCues)
            {
                if (frame == cueMark.startFrame)
                {
                    cueMark.cues.ForEach(cue => cue.ApplyFrom(_abilitySpec));
                }
            }
                
            // 释放型GameplayEffect
            foreach (var mark in _cacheReleaseGameplayEffect)
            {
                if (frame == mark.startFrame)
                {
                    var catcher = mark.LoadTargetCatcher();
                    catcher.Init(_abilitySpec.Owner);
                    var targets = catcher.CatchTargets(_abilitySpec.Target);
                    if (targets != null)
                    {
                        foreach (var asc in targets)
                            mark.gameplayEffectAssets
                                .ForEach(effect => _abilitySpec.Owner.ApplyGameplayEffectTo(new GameplayEffect(effect), asc));
                    }
                }
            }
            
            // Instant Task
            foreach (var mark in _cacheInstantTasks)
            {
                if (frame == mark.startFrame)
                {
                    mark.InstantTasks.ForEach(task => task.Task.OnExecute());
                }
            }
            
            // Cue 持续
            foreach (var cueClip in _cacheDurationalCueTrack)
            {
                if(frame == cueClip.startFrame) cueClip.cueSpec.OnAdd();
                if (frame >= cueClip.startFrame && frame <= cueClip.endFrame)
                    cueClip.cueSpec.OnTick();
                if(frame == cueClip.endFrame) cueClip.cueSpec.OnRemove();
            }
            
            // Buff型GameplayEffect
            // buff持续时间以Timeline配置时间为准（执行策略全部改为Infinite）
            foreach (var buffClip in _cacheBuffGameplayEffectTrack)
            {
                if (frame == buffClip.startFrame)
                {
                    var buffSpec = _abilitySpec.Owner.ApplyGameplayEffectToSelf(buffClip.buff);
                    buffSpec.SetDurationPolicy(EffectsDurationPolicy.Infinite);
                    buffClip.buffSpec = buffSpec;
                }

                if (frame == buffClip.endFrame)
                {
                    if (buffClip.buffSpec != null)
                        _abilitySpec.Owner.RemoveGameplayEffect(buffClip.buffSpec);
                    buffClip.buffSpec = null;
                }
            }
                                                              
            // Ongoing Task
            foreach (var taskClip in _cacheOngoingTaskTrack)
            {
                if(frame == taskClip.startFrame) taskClip.ongoingTask.Task.OnStart(frame);
                if (frame >= taskClip.startFrame && frame <= taskClip.EndFrame)
                    taskClip.ongoingTask.Task.OnTick(frame, taskClip.startFrame, taskClip.EndFrame);
                if(frame == taskClip.EndFrame) taskClip.ongoingTask.Task.OnEnd(frame);
            }
        }
    }
}