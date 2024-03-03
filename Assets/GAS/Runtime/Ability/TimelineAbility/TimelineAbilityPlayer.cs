using System.Collections.Generic;
using GAS.General;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;
using GAS.Runtime.Component;
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
    
    public class RuntimeTaskClip:RuntimeClipInfo
    {
        public OngoingAbilityTaskSpec taskSpec;
    }
    
    public class TimelineAbilityPlayer
    {
        bool _isPlaying;
        public bool IsPlaying => _isPlaying;
        private readonly TimelineAbilitySpec _abilitySpec;
        public TimelineAbilityAsset AbilityAsset=>_abilitySpec.AbilityAsset;

        private int _currentFrame;
        private float _playTotalTime;
        private int MaxFrameCount => AbilityAsset.MaxFrameCount;
        private int FrameRate => GASTimer.FrameRate;
        
        
        //  Cache Cue 即时
        private List<InstantCueMarkEvent> _cacheInstantCues = new List<InstantCueMarkEvent>();
        //  Cache 释放型GameplayEffect
        private List<ReleaseGameplayEffectMarkEvent> _cacheReleaseGameplayEffect = new List<ReleaseGameplayEffectMarkEvent>();
        //  Cache Instant Task
        private List<TaskMarkEvent> _cacheInstantTasks = new List<TaskMarkEvent>();
        
        //  Cache Cue 持续
        private List<RuntimeDurationCueClip> _cacheDurationalCueTrack = new List<RuntimeDurationCueClip>();
        //  Cache Buff型GameplayEffect 
        private List<RuntimeBuffClip> _cacheBuffGameplayEffectTrack = new List<RuntimeBuffClip>();
        //  Cache Ongoing Task
        private List<RuntimeTaskClip> _cacheOngoingTaskTrack = new List<RuntimeTaskClip>();
        
        public TimelineAbilityPlayer(TimelineAbilitySpec abilitySpec)
        {
            _abilitySpec = abilitySpec;
        }

        private void Prepare()
        {
            _cacheInstantCues.Clear();
            _cacheInstantCues.AddRange(AbilityAsset.InstantCues.markEvents);
            _cacheInstantCues.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            
            _cacheReleaseGameplayEffect.Clear();
            _cacheReleaseGameplayEffect.AddRange(AbilityAsset.ReleaseGameplayEffect.markEvents);
            _cacheReleaseGameplayEffect.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            
            _cacheInstantTasks.Clear();
            _cacheInstantTasks.AddRange(AbilityAsset.InstantTasks.markEvents);
            _cacheInstantTasks.Sort((a, b) => a.startFrame.CompareTo(b.startFrame));
            
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
            
            _cacheBuffGameplayEffectTrack.Clear();
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
            
            _cacheOngoingTaskTrack.Clear();
            foreach (var track in AbilityAsset.OngoingTasks)
            {
                foreach (var clipEvent in track.clipEvents)
                {
                    var taskSpec = clipEvent.task.CreateBaseSpec(_abilitySpec);
                    if (taskSpec == null) continue;
                    var runtimeTaskClip = new RuntimeTaskClip
                    {
                        startFrame = clipEvent.startFrame,
                        endFrame = clipEvent.EndFrame,
                        taskSpec = taskSpec
                    };
                    _cacheOngoingTaskTrack.Add(runtimeTaskClip);
                }
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
            _cacheInstantCues.Clear();
            _cacheReleaseGameplayEffect.Clear();
            _cacheInstantTasks.Clear();

            foreach (var clip in _cacheDurationalCueTrack) clip.cueSpec.OnRemove();
            _cacheDurationalCueTrack.Clear();

            foreach (var clip in _cacheBuffGameplayEffectTrack)
                if (clip.buffSpec != null)
                    _abilitySpec.Owner.RemoveGameplayEffect(clip.buffSpec);
            _cacheBuffGameplayEffectTrack.Clear();
            
            foreach (var clip in _cacheOngoingTaskTrack) clip.taskSpec.OnEnd(clip.endFrame);
            _cacheOngoingTaskTrack.Clear();
            
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
            if (_currentFrame >= MaxFrameCount) OnPlayEnd();
        }

        /// <summary>
        /// 播放结束
        /// </summary>
        private void OnPlayEnd()
        {
            _isPlaying = false;
            _abilitySpec.TryEndAbility();
        }

        /// <summary>
        /// 当前帧的事件
        /// </summary>
        /// <param name="frame"></param>
        private void TickFrame(int frame)
        {
            // Cue 即时
            while(_cacheInstantCues.Count > 0 && frame == _cacheInstantCues[0].startFrame)
            {
                _cacheInstantCues[0].cues.ForEach(cue => cue.ApplyFrom(_abilitySpec));
                _cacheInstantCues.RemoveAt(0);
            }
                
            // 释放型GameplayEffect
            while(_cacheReleaseGameplayEffect.Count > 0 && frame == _cacheReleaseGameplayEffect[0].startFrame)
            {
                // TODO 捕获目标的手段 
                AbilitySystemComponent[] targets = new[] { _abilitySpec.Owner };
                foreach (var asc in targets)
                    _cacheReleaseGameplayEffect[0].gameplayEffectAssets
                        .ForEach(effect => _abilitySpec.Owner.ApplyGameplayEffectTo(new GameplayEffect(effect),asc));
                _cacheReleaseGameplayEffect.RemoveAt(0);
            }
            
            // Instant Task
            while(_cacheInstantTasks.Count > 0 && frame == _cacheInstantTasks[0].startFrame)
            {
                _cacheInstantTasks[0].InstantTasks.ForEach(task => task.CreateBaseSpec(_abilitySpec).OnExecute());
                _cacheInstantTasks.RemoveAt(0);
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
                if(frame == taskClip.startFrame) taskClip.taskSpec.OnStart(frame);
                if (frame >= taskClip.startFrame && frame <= taskClip.endFrame)
                    taskClip.taskSpec.OnTick(frame, taskClip.startFrame, taskClip.endFrame);
                if(frame == taskClip.endFrame) taskClip.taskSpec.OnEnd(frame);
            }
        }
    }
}