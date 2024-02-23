using GAS.General;
using UnityEngine;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    public class SequentialAbilityPlayer
    {
        bool _isPlaying;
        public bool IsPlaying => _isPlaying;
        public readonly GeneralSequentialAbilityAsset AbilityAsset;

        private int _currentFrame;
        private float _playTotalTime;
        private int MaxFrameCount => AbilityAsset.MaxFrameCount;
        private int FrameRate => GASTimer.FrameRate;
        
        public SequentialAbilityPlayer(GeneralSequentialAbilityAsset abilityAsset)
        {
            AbilityAsset = abilityAsset;
        }
        
        public void Play()
        {
            _currentFrame = -1; // 为了播放第0帧
            _playTotalTime = 0;
            _isPlaying = true;
        }
        
        public void Stop()
        {
            _isPlaying = false;
        }

        public void Tick()
        {
            if(_isPlaying)
            {
                _playTotalTime += Time.deltaTime;
                int targetFrame = (int)(_playTotalTime * FrameRate);
                // 追帧
                while(_currentFrame < targetFrame)
                {
                    _currentFrame++;
                    // TODO : 播放当前帧的事件
                    TickFrame(_currentFrame);
                }

                if(_currentFrame >= MaxFrameCount)
                {
                    _isPlaying = false;
                    // TODO : 播放结束
                }
            }
        }
        
        private void TickFrame(int frame)
        {
            // TODO : 播放当前帧的事件
            // 动画
            foreach (var animationClipEvent in AbilityAsset.AnimationData.animationClipData)
            {
                if (frame >= animationClipEvent.startFrame && frame <= animationClipEvent.EndFrame)
                {
                    // TODO : 播放动画
                }
            }
            // 能力是否可打断（取消）状态更新
            // Cue 持续性
            // Cue 瞬时性
            // 给予指定单位计时GameplayEffect
            // 对指定单位施加GameplayEffect 
            // 自定义事件
        }
    }
}