using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     单足落地/摆动状态机：地面投射 → 自研 IK 解算 → 脚掌贴地。
    /// </summary>
    public class FootPlacement
    {
        private readonly BaseMultiLeggedLocomotion _locomotion;
        private readonly FootMotionGroup _group;
        private readonly FootConfig _config;

        /// <summary>落点锚点（地面投射起点，运行时生成，挂在组节点下）</summary>
        private readonly Transform _stepAnchor;

        private Vector3 _castPosition;
        private Vector3 _groundNormal = Vector3.up;

        // 摆动状态
        private bool _isMoving;
        private float _startMoveTime;
        private float _endMoveTime;
        private Vector3 _startPos;
        private Vector3 _targetPos;

        public Transform StepPoint => _stepAnchor;
        public Transform IdlePoint => _config.idlePoint;
        public Vector3 CastPosition => _castPosition;
        public Vector3 StartPos => _startPos;
        public Vector3 TargetPos => _targetPos;

        /// <summary>当前足骨骼（解算后）的世界位置</summary>
        public Vector3 FootWorldPosition => _config.foot != null ? _config.foot.position : _stepAnchor.position;

        public FootPlacement(FootMotionGroup group, FootConfig config, Transform stepAnchor)
        {
            _group = group;
            _locomotion = group.Locomotion;
            _config = config;
            _stepAnchor = stepAnchor;
        }

        public void Tick()
        {
            UpdateCast();

            // 步态评估内聚到 Tick：cast 更新后立即判断是否需要迈步，
            // 消除"基类先用旧 cast 评估、下一帧才触发"的延迟，组内双足天然同步。
            if (!_isMoving
                && (_castPosition - FootWorldPosition).sqrMagnitude
                > _locomotion.L * _group.PhaseDifference * _locomotion.L * _group.PhaseDifference)
            {
                SetMoving();
            }

            if (_isMoving)
            {
                var duration = Mathf.Max(1e-4f, _endMoveTime - _startMoveTime);
                var timeNormalized = Mathf.Clamp01((Time.time - _startMoveTime) / duration);

                var pos = _locomotion.CalculateFootPlacementMovingPoint(_startPos, _targetPos, timeNormalized);
                TwoBoneIK.Solve(_config.hip, _config.knee, _config.foot, pos, _config.pole);

                if (timeNormalized >= 1f)
                {
                    _isMoving = false;
                    // 精确落点，避免浮点误差
                    TwoBoneIK.Solve(_config.hip, _config.knee, _config.foot, _targetPos, _config.pole);
                }
            }
            else
            {
                // 落地钉脚：持续解算到当前地面投射点，跟随地形起伏
                TwoBoneIK.Solve(_config.hip, _config.knee, _config.foot, _castPosition, _config.pole);

                // 脚掌贴地仅在落地时执行（摆动中脚在空中，强行对齐地面法线会乱转）
                if (_config.alignFootToGround)
                {
                    var weight = 1f - Mathf.Exp(-10f * Time.deltaTime);
                    TwoBoneIK.AlignFootToGround(_config.foot, _groundNormal, weight, _config.footUpAxis);
                }
            }
        }

        /// <summary>刷新地面投射点与法线</summary>
        private void UpdateCast()
        {
            EXMachHelper.GetGroundInfo(_stepAnchor.position, _locomotion.castMaxDistance,
                _locomotion.groundLayer, Vector3.down, out var point, out var normal);

            _castPosition = point + _config.offset;
            _groundNormal = normal;
        }

        /// <summary>
        ///     触发摆动：以当前地面投射点为落点，时长 = clamp(min(步态周期×相位差, 单步最大时长), 最小时长, ∞)。
        /// </summary>
        public void SetMoving()
        {
            var rawDuration = Mathf.Min(_locomotion.T * _group.PhaseDifference, _locomotion.stepTime);
            var duration = Mathf.Clamp(rawDuration, _locomotion.minSwingDuration, _locomotion.stepTime);
            _targetPos = _castPosition;

            // 极端参数保护：时长过短直接瞬移
            if (duration <= 0.02f)
            {
                TwoBoneIK.Solve(_config.hip, _config.knee, _config.foot, _targetPos, _config.pole);
                _isMoving = false;
                return;
            }

            // 已在摆动中则保持当前摆动（不打断）
            if (_isMoving)
                return;

            _startPos = FootWorldPosition;
            _startMoveTime = Time.time;
            _endMoveTime = _startMoveTime + duration;
            _isMoving = true;
        }

        /// <summary>把落点锚点吸附回待机锚点（可沿移动方向前伸），触发新一轮摆动。</summary>
        public void RefreshStepPoint()
        {
            if (_config.idlePoint == null)
                return;

            var pos = _config.idlePoint.position;

            // 落点前伸：沿平滑速度方向前伸 L×stepAheadRatio，腿迈向前方而非原地踏步
            var direction = _locomotion.Direction;
            if (_locomotion.stepAheadRatio > 0.001f && direction.sqrMagnitude > 0.01f)
                pos += direction * (_locomotion.L * _locomotion.stepAheadRatio);

            _stepAnchor.position = pos;
        }

        public bool IsMoving() => _isMoving;

        /// <summary>落点相对待机锚点在移动方向上的带符号投影距离</summary>
        public float DistanceInSpeedDirection()
        {
            if (_config.idlePoint == null)
                return 0f;

            return EXMachHelper.CalculateProjectionDistance(_stepAnchor.position, _config.idlePoint.position,
                _locomotion.Direction);
        }
    }
}