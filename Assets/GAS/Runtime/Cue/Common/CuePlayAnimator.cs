using UnityEngine;

namespace GAS.Runtime
{
    public class CuePlayAnimator: GameplayCueBase<XParamAnimator>
    {
        private Animator _animator;

        public override void OnAdd(float time)
        {
            base.OnAdd(time);
            var go = _abilitySystemCell?.GameObject;
            if (go != null)
            {
                var node = go.transform.Find(Parameter.AnimatorNodePath);
                if (node != null)
                    _animator = node.GetComponent<Animator>();
                
            }
        }

        public override void OnActivate(float time)
        {
            base.OnActivate(time);
            if (_animator != null) _animator.Play(Parameter.AnimationName);
        }

        public override void OnRemove(float time)
        {
            base.OnRemove(time);
            _animator = null;
        }

        public override void OnDeactivate(float time)
        {
            base.OnDeactivate(time);
            if (_animator != null) _animator.StopPlayback();
        }

        public override void OnPreview(int frame, int startFrame, int endFrame,params object[] args)
        {
            base.OnPreview(frame, startFrame, endFrame);
            GameObject target = args.Length > 0 ? args[0] as GameObject : null;
            AnimationClip clip = args.Length > 1 ? args[1] as AnimationClip : null;
            if (target == null || clip == null) return;

            // 进入动画采样模式，这确保了对象属性的修改可以安全地被记录/撤销
            UnityEditor.AnimationMode.StartAnimationMode();
        
            // 计算实际时间（秒）。假设 normalizedTime 是 0 到 1 之间的值。
            var normalizedTime = (float)(frame - startFrame) / (endFrame - startFrame);
            var timeInSeconds = Mathf.Clamp01(normalizedTime) * clip.length;
        
            // 核心API：对目标对象采样指定动画在特定时间的姿态
            UnityEditor.AnimationMode.SampleAnimationClip(target, clip, timeInSeconds);
        
            // 采样完成后，可以选择结束动画模式
            UnityEditor.AnimationMode.StopAnimationMode();
        
            // 刷新场景视图以立即看到变化
            UnityEditor.SceneView.RepaintAll();
        }
    }
}