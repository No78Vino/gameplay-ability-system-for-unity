using UnityEditor;
using UnityEngine;  
  
namespace GAS.Runtime  
{  
    /// <summary>  
    /// 播放音效的Cue  
    /// - 支持播放速度控制 (AudioSource.pitch)  
    /// - 非循环音效播完后自动 RemoveSelf + KillSelf  
    /// </summary>  
    public class CuePlaySound : GameplayCueBase<XParamPlaySound>  
    {  
        private AudioSource _audioSource;  
        private AudioClip _audioClip;  
        private bool _isOwnAudioSource;  
        private float _playStartTime;  
        private bool _isPlaying;  
  
        public override void OnAdd(float time)  
        {  
            base.OnAdd(time);  
  
            var go = _abilitySystemCell?.GameObject;  
            if (go == null) return;  
  
            // 加载 AudioClip  
            if (!string.IsNullOrEmpty(Parameter.AudioClipPath))  
            {  
                _audioClip = GASResourceLoader.LoadSync<AudioClip>(Parameter.AudioClipPath);  
#if UNITY_EDITOR  
                if (_audioClip == null)  
                    Debug.LogWarning($"[CuePlaySound] AudioClip not found: {Parameter.AudioClipPath}");  
#endif  
            }  
  
            // 查找 AudioSource  
            if (!string.IsNullOrEmpty(Parameter.AudioSourceNodePath))  
            {  
                var node = go.transform.Find(Parameter.AudioSourceNodePath);  
                if (node != null)  
                    _audioSource = node.GetComponent<AudioSource>();  
            }  
            else  
            {  
                _audioSource = go.GetComponent<AudioSource>();  
            }  
  
            // 没找到则自行创建  
            if (_audioSource == null)  
            {  
                _audioSource = go.AddComponent<AudioSource>();  
                _isOwnAudioSource = true;  
            }  
        }  
  
        public override void OnActivate(float time)  
        {  
            base.OnActivate(time);  
            PlaySound(time);  
        }  
  
        public override void OnTick(float time)  
        {  
            base.OnTick(time);  
  
            if (!_isPlaying) return;  
  
            // 循环音效不自动销毁  
            if (Parameter.Loop) return;  
  
            // 非循环音效：播完后自动关闭并销毁自己  
            if (_audioClip == null) return;  
  
            var speed = Mathf.Max(Parameter.Speed, 0.01f);  
            var actualDuration = _audioClip.length / speed;  
  
            if (time - _playStartTime >= actualDuration)  
            {  
                StopSound();  
                RemoveSelf();  
                KillSelf();  
            }  
        }  
  
        public override void OnDeactivate(float time)  
        {  
            base.OnDeactivate(time);  
            StopSound();  
        }  
  
        public override void OnRemove(float time)  
        {  
            base.OnRemove(time);  
            StopSound();  
            CleanUp();  
        }  
  
        public override void OnDestroy(float time)  
        {  
            base.OnDestroy(time);  
            CleanUp();  
        }  
  
        public override void Reset()  
        {  
            StopSound();  
        }  
  
        private void PlaySound(float time)  
        {  
            if (_audioSource == null || _audioClip == null) return;  
  
            _audioSource.clip = _audioClip;  
            _audioSource.volume = Parameter.Volume;  
            _audioSource.pitch = Mathf.Max(Parameter.Speed, 0.01f);  
            _audioSource.loop = Parameter.Loop;  
            _audioSource.Play();  
  
            _playStartTime = time;  
            _isPlaying = true;  
        }  
  
        private void StopSound()  
        {  
            if (_audioSource != null && _audioSource.isPlaying)  
                _audioSource.Stop();  
  
            _isPlaying = false;  
        }  
  
        private void CleanUp()  
        {  
            if (_audioClip != null)  
            {  
                GASResourceLoader.Release(_audioClip);  
                _audioClip = null;  
            }  
  
            if (_isOwnAudioSource && _audioSource != null)  
            {  
                Object.Destroy(_audioSource);  
                _isOwnAudioSource = false;  
            }  
  
            _audioSource = null;  
            _isPlaying = false;  
        }  
  
#if UNITY_EDITOR  
        public override void OnPreview(GameObject target, int frame, int startFrame, int endFrame)  
        {  
            base.OnPreview(target, frame, startFrame, endFrame);  
  
            if (target == null || string.IsNullOrEmpty(Parameter.AudioClipPath)) return;  
  
            if (frame == startFrame)  
            {  
                // 编辑器环境下（未播放）加载音频文件
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Parameter.AudioClipPath);
                if (clip != null)  
                    AudioSource.PlayClipAtPoint(clip, target.transform.position, Parameter.Volume);  
            }  
        }  
#endif  
    }  
}