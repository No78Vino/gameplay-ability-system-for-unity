using System;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     足对分组：同相位的若干足为一组，交替摆动形成步态。
    /// </summary>
    [Serializable]
    public class FootMotionGroup
    {
        [Tooltip("本组包含的足配置")]
        public FootConfig[] feet;

        private FootPlacement[] _footPlacements;
        private Transform _tfGroup;

        /// <summary>本组的单足运行时状态</summary>
        public FootPlacement[] FootPlacements => _footPlacements;

        /// <summary>本组的运行时节点</summary>
        public Transform GroupTransform => _tfGroup;

        public BaseMultiLeggedLocomotion Locomotion { get; private set; }

        /// <summary>本组相对步态周期的相位差（0~1）</summary>
        public float PhaseDifference { get; private set; }

        /// <summary>本组的拆分步长（相位差 × 步长）</summary>
        public float SplitStepLength => PhaseDifference * Locomotion.L;

        public void Initialize(BaseMultiLeggedLocomotion locomotion, float phaseDifference)
        {
            Locomotion = locomotion;
            PhaseDifference = phaseDifference;

            if (_footPlacements != null)
                return;

            if (feet == null || feet.Length == 0)
            {
                Debug.LogWarning($"[{nameof(FootMotionGroup)}] feet 未配置足引用。", locomotion);
                return;
            }

            if (_tfGroup == null)
            {
                _tfGroup = new GameObject("FootMotionGroup").transform;
                _tfGroup.SetParent(locomotion.MotionTransform);
            }

            _footPlacements = new FootPlacement[feet.Length];
            for (var i = 0; i < feet.Length; i++)
            {
                var footAnchor = new GameObject("Foot_" + i).transform;
                footAnchor.SetParent(_tfGroup);

                var config = feet[i];
                if (config.idlePoint != null)
                    footAnchor.position = config.idlePoint.position;

                _footPlacements[i] = new FootPlacement(this, config, footAnchor);
            }
        }

        /// <summary>本组是否有足正在摆动（任一只摆即视为组在摆，避免只查第一只失真）</summary>
        public bool IsMoving()
        {
            if (_footPlacements == null || _footPlacements.Length == 0)
                return false;

            foreach (var foot in _footPlacements)
            {
                if (foot.IsMoving())
                    return true;
            }

            return false;
        }

        /// <summary>本组待机中心与当前中心的世界距离（衡量“落后”程度）</summary>
        public float DistanceInDirection()
        {
            if (_footPlacements == null || _footPlacements.Length == 0)
                return 0f;

            return (IdleCenter() - CurrentCenter()).magnitude;
        }

        /// <summary>落后程度相对拆分步长的比例</summary>
        public float DistanceRate()
        {
            var dist = DistanceInDirection();
            var splitL = Locomotion.L * PhaseDifference;
            return splitL > 1e-4f ? dist / splitL : 0f;
        }

        /// <summary>待机锚点中心（随身体移动）</summary>
        public Vector3 IdleCenter()
        {
            if (_footPlacements == null || _footPlacements.Length == 0)
                return Locomotion.body.position;

            var sum = Vector3.zero;
            var count = 0;
            foreach (var foot in _footPlacements)
            {
                if (foot.IdlePoint == null)
                    continue;
                sum += foot.IdlePoint.position;
                count++;
            }

            return count > 0 ? sum / count : Locomotion.body.position;
        }

        /// <summary>当前落点中心</summary>
        public Vector3 CurrentCenter()
        {
            if (_footPlacements == null || _footPlacements.Length == 0)
                return Locomotion.body.position;

            var sum = Vector3.zero;
            var count = 0;
            foreach (var foot in _footPlacements)
            {
                if (foot.StepPoint == null)
                    continue;
                sum += foot.StepPoint.position;
                count++;
            }

            return count > 0 ? sum / count : Locomotion.body.position;
        }

        public void Tick()
        {
            if (_footPlacements == null)
                return;

            foreach (var foot in _footPlacements)
                foot.Tick();
        }

        /// <summary>
        ///     刷新本组落点：把落点锚点吸附到待机锚点，触发新一轮摆动。
        /// </summary>
        public void UpdateFootPlacements()
        {
            if (_footPlacements == null)
                return;

            foreach (var foot in _footPlacements)
                foot.RefreshStepPoint();
        }
    }
}