using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     EX程序化动画 - 多足移动基类。
    ///     <para>步态模型：T = L / v（步态周期）、f = 1 / T（步频）、h（离地间隙）。</para>
    ///     <para>足部由自研 IK（TwoBoneIK）直接驱动骨骼，不依赖任何第三方 IK 插件。</para>
    /// </summary>
    public abstract class BaseMultiLeggedLocomotion : MonoBehaviour
    {
        protected const string TAB_GROUP = "运动参数";
        protected const string TAB_BIND = "绑定";
        protected const string TAB_BASE = "基础参数";
        protected const string TAB_RHYTHM = "节奏";

        // ==================== 绑定 ====================

        [TabGroup(TAB_GROUP, TAB_BIND)]
        [LabelText("步态足部分组")]
        [InfoBox("将肢体分为 N 组交替运动，每组内同相位")]
        public FootMotionGroup[] motionGroup;

        [TabGroup(TAB_GROUP, TAB_BIND)]
        [LabelText("躯干")]
        public Transform body;

        [TabGroup(TAB_GROUP, TAB_BIND)]
        [LabelText("足目标节点父级（运行时自动挂载到管理器根节点）")]
        public Transform footTargetGroupNode;

        [TabGroup(TAB_GROUP, TAB_BIND)]
        [LabelText("躯干偏航跟随根节点")]
        public bool syncRotationWithRoot;

        // ==================== 基础参数 ====================

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("步长 L (m)")]
        [Tooltip("单步前进的直线距离，与肢体长度、摆动角度正相关")]
        public float L = 0.7f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("肢体相位差占比")]
        [Tooltip("相邻肢体运动的时间差占步态周期的比例，决定步态类型")]
        public float[] legPhaseDifference = { 0.5f, 0.5f };

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("离地间隙 h (m)")]
        [Tooltip("机身最低处到地面的垂直距离，影响越障能力")]
        public float h = 0.5f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("地面检测最远距离")]
        public float castMaxDistance = 20f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("地面检测 Layer")]
        public LayerMask groundLayer;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("自动计算速度")]
        [Tooltip("勾选后由本组件测量躯干位移计算速度；否则由外部写入 velocity")]
        public bool checkSelfVelocity = true;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("速度 (m/s)")]
        [Tooltip("可外部写入；勾选自动计算速度时会被覆盖")]
        public Vector3 velocity;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("足部拉扯极限")]
        [Tooltip("足与锚点距离超过该值时的复位阈值")]
        public float stepDistanceLimit = 1f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("躯干旋转平滑度")]
        public float bodyRotationSmoothing = 8f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("躯干高度平滑度")]
        [Tooltip("躯干垂直跟随的平滑速度（越大贴地越快，越小越柔）。避免地形起伏/落点刷新时的瞬跳")]
        public float bodyHeightSmoothing = 10f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("停止后复位")]
        public bool resetWhenStop;

        [ShowIf(nameof(resetWhenStop))]
        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("复位延迟(s)")]
        public float resetWaitingDuration;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("单步最大时长(s)")]
        [Tooltip("单步摆动的时长上限，避免大步幅时摆动过慢")]
        public float stepTime = 1f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("单步最小时长(s)")]
        [Tooltip("单步摆动的时长下限，避免高速时步频失控（越小越灵敏）")]
        public float minSwingDuration = 0.08f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("视觉辅助 Gizmos")]
        [Tooltip("在场景视图中绘制步态调试线")]
        public bool gizmosOn;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("落点前伸比例")]
        [Tooltip("刷新落点时沿移动方向的前伸量（相对步长 L 的比例）。0=不前伸（原地步态），常见 0.2~0.5")]
        [Range(0f, 1f)]
        public float stepAheadRatio = 0.35f;

        [TabGroup(TAB_GROUP, TAB_BASE)]
        [LabelText("速度平滑系数")]
        [Tooltip("速度低通滤波强度，越大越快响应真实位移。过大导致步频抖动")]
        public float velocitySmoothing = 12f;

        // ==================== 节奏 ====================

        [TabGroup(TAB_GROUP, TAB_RHYTHM)]
        [LabelText("呼吸节奏系统")]
        public RhythmSystem rhythmSystem;

        [TabGroup(TAB_GROUP, TAB_RHYTHM)]
        [LabelText("呼吸强度")]
        public float intensity;

        // ==================== 只读推导参数 ====================

        [ShowInInspector]
        [ReadOnly]
        [LabelText("步态周期 T (s)")]
        [DisplayAsString]
        public float T => v == 0f ? 0f : L / v;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("步频 f (Hz)")]
        [DisplayAsString]
        public float f => 1f / T;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("速度 (m/s)")]
        public float v => velocity.magnitude;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("移动方向")]
        public Vector3 Direction => velocity.normalized;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("足数")]
        [DisplayAsString]
        public virtual int N => 2;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("腿节数")]
        [DisplayAsString]
        public virtual int SegmentsNumber => 3;

        /// <summary>是否在移动（基于实测位移，修复原 v==0 判断反了的问题）</summary>
        public bool IsMoving => _measuredVelocity.sqrMagnitude > 1e-6f;

        /// <summary>运行时根变换（挂在管理器根节点下）</summary>
        public Transform MotionTransform { get; private set; }

        private Vector3 _lastBodyPosition;
        private Vector3 _lastBodyStepPos;
        private Vector3 _measuredVelocity;
        private float _stopTime = -1f;

        private XVisualLine _visualLine;

        protected virtual void Awake()
        {
            if (motionGroup == null || motionGroup.Length == 0)
            {
                Debug.LogWarning($"[{nameof(BaseMultiLeggedLocomotion)}] {name}: motionGroup 未配置足部分组。", this);
                return;
            }

            if (body == null)
            {
                Debug.LogWarning($"[{nameof(BaseMultiLeggedLocomotion)}] {name}: body 未绑定躯干。", this);
                return;
            }

            MotionTransform = new GameObject(name + "_runtime").transform;
            MotionTransform.SetParent(EXProceduralMachineManager.Instance.ManagerRoot);

            if (footTargetGroupNode == null)
            {
                footTargetGroupNode = new GameObject(name + "_foot_target_group").transform;
                footTargetGroupNode.SetParent(MotionTransform);
            }
            else
            {
                footTargetGroupNode.name = name + "_foot_target_group";
                footTargetGroupNode.SetParent(MotionTransform);
            }

            for (var i = 0; i < motionGroup.Length; i++)
            {
                var group = motionGroup[i];
                var phase = legPhaseDifference != null && i < legPhaseDifference.Length ? legPhaseDifference[i] : 0.5f;
                group.Initialize(this, phase);
            }

            _lastBodyPosition = body.position;
            _lastBodyStepPos = body.position;
        }

        private void Update()
        {
            if (body == null || motionGroup == null || motionGroup.Length == 0)
                return;

            UpdateBodyHeightAndRotation();
            TryTriggerStep();
            foreach (var group in motionGroup)
                group.Tick();

            UpdateVelocity();
            UpdateStopReset();
#if UNITY_EDITOR
            UpdateGizmos();
#endif
        }

        // ==================== 躯干 ====================

        private void UpdateBodyHeightAndRotation()
        {
            // 高度：足部地面投射点平均高度 + 离地间隙 + 呼吸偏移
            var footCount = 0;
            var sumY = 0f;
            foreach (var group in motionGroup)
            {
                if (group?.FootPlacements == null)
                    continue;
                foreach (var foot in group.FootPlacements)
                {
                    sumY += foot.CastPosition.y;
                    footCount++;
                }
            }

            if (footCount > 0)
            {
                var targetY = sumY / footCount + h;

                if (rhythmSystem != null)
                    targetY += rhythmSystem.GetValue() * intensity;

                // 平滑跟随目标高度：避免地形起伏/足组刷新时躯干垂直瞬跳
                var bodyPos = body.position;
                bodyPos.y = Mathf.Lerp(bodyPos.y, targetY, 1f - Mathf.Exp(-bodyHeightSmoothing * Time.deltaTime));
                body.position = bodyPos;
            }

            // 旋转：三点定面求俯仰/横滚贴地，偏航由自身（或根节点）控制
            var rotation = ComputeBodyRotation();
            if (rotation.HasValue)
            {
                var euler = rotation.Value.eulerAngles;
                euler.y = syncRotationWithRoot ? transform.eulerAngles.y : body.eulerAngles.y;
                var targetRotation = Quaternion.Euler(euler);
                body.rotation = Quaternion.Slerp(body.rotation, targetRotation,
                    1f - Mathf.Exp(-bodyRotationSmoothing * Time.deltaTime));
            }
        }

        private Quaternion? ComputeBodyRotation()
        {
            // 收集足部参考点做三点定面：
            // 落地脚用实际 IK 位置（保留地形跟随），摆动脚回退到地面投射点（避免抬脚扭曲躯干姿态）
            var points = new List<Vector3>();
            foreach (var group in motionGroup)
            {
                if (group?.FootPlacements == null)
                    continue;
                foreach (var foot in group.FootPlacements)
                {
                    points.Add(foot.IsMoving() ? foot.CastPosition : foot.FootWorldPosition);
                }
            }

            if (points.Count < 3)
                return null;

            var forward = body.forward;
            forward.y = 0f;

            return EXMachHelper.CalculateBodyRotation(forward, points[0], points[1], points[2]);
        }

        // ==================== 步态驱动 ====================

        /// <summary>躯干走过一个步长 L 时，刷新落后最远的足组落点</summary>
        private void TryTriggerStep()
        {
            if ((body.position - _lastBodyStepPos).sqrMagnitude <= L * L)
                return;

            _lastBodyStepPos = body.position;
            var group = GetCurrentMotionGroup();
            var movingGroupIndex = GetMovingGroupIndex();

            if (movingGroupIndex < 0
                || group == motionGroup[movingGroupIndex]
                || group.DistanceInDirection() > stepDistanceLimit)
            {
                group.UpdateFootPlacements();
            }
        }

        /// <summary>实测躯干位移并平滑得到速度（供步态周期 T=L/v 使用）</summary>
        private void UpdateVelocity()
        {
            // 实测位移（去垂直分量），做低通平滑：稳定 T=L/v 的步态周期，避免步频抖动
            var measured = (body.position - _lastBodyPosition) / Mathf.Max(1e-4f, Time.deltaTime);
            measured.y = 0f;
            _measuredVelocity = Vector3.Lerp(_measuredVelocity, measured,
                1f - Mathf.Exp(-velocitySmoothing * Time.deltaTime));

            if (checkSelfVelocity)
            {
                velocity = _measuredVelocity;
                velocity.y = 0f;
            }

            _lastBodyPosition = body.position;
        }

        private void UpdateStopReset()
        {
            if (!resetWhenStop)
                return;

            if (_stopTime < 0f)
            {
                if (!IsMoving)
                    _stopTime = Time.time;
            }
            else if (IsMoving)
            {
                _stopTime = -1f;
            }
            else if (Time.time >= _stopTime + resetWaitingDuration)
            {
                _stopTime = -1f;
                foreach (var group in motionGroup)
                    group?.UpdateFootPlacements();
            }
        }

        protected void OnDestroy()
        {
            if (MotionTransform != null)
                Destroy(MotionTransform.gameObject);
        }

        // ==================== 查询 ====================

        /// <summary>当前偏离最远的足组（应最先迈步）</summary>
        private FootMotionGroup GetCurrentMotionGroup()
        {
            var maxRate = -1f;
            var index = 0;
            for (var i = 0; i < motionGroup.Length; i++)
            {
                var rate = motionGroup[i].DistanceRate();
                if (!(rate > maxRate))
                    continue;
                maxRate = rate;
                index = i;
            }

            return motionGroup[index];
        }

        /// <summary>当前正在摆动的足组下标，没有则返回 -1</summary>
        public int GetMovingGroupIndex()
        {
            for (var i = 0; i < motionGroup.Length; i++)
            {
                if (motionGroup[i].IsMoving())
                    return i;
            }

            return -1;
        }

        // ==================== 可视化 ====================

        private void SetVisualLine(List<XVisualLine.XVisualLineData> list, int index, Color color, Vector3 a, Vector3 b)
        {
            if (index + 1 >= list.Count)
            {
                list.Add(new XVisualLine.XVisualLineData());
                index = list.Count - 1;
            }

            var lineData = _visualLine.lines[index];
            lineData.gizmoColor = color;
            lineData.pointA = a;
            lineData.pointB = b;
        }

        private void UpdateGizmos()
        {
            if (!gizmosOn)
                return;

            if (_visualLine == null)
            {
                _visualLine = GetComponent<XVisualLine>();
                if (_visualLine == null)
                    _visualLine = gameObject.AddComponent<XVisualLine>();
            }

            var i = -1;
            foreach (var group in motionGroup)
            {
                if (group?.FootPlacements == null)
                    continue;

                foreach (var foot in group.FootPlacements)
                {
                    // 足骨骼位置 → 落点锚点
                    i++;
                    SetVisualLine(_visualLine.lines, i, Color.white, foot.FootWorldPosition, foot.StepPoint.position);

                    // 地面投射点 → 足骨骼位置（摆动中绿色，落地红色）
                    i++;
                    var moveColor = foot.IsMoving() ? Color.green : Color.red;
                    SetVisualLine(_visualLine.lines, i, moveColor, foot.CastPosition, foot.FootWorldPosition);

                    // 待机锚点 → 落点锚点
                    i++;
                    SetVisualLine(_visualLine.lines, i, Color.cyan, foot.IdlePoint.position, foot.StepPoint.position);
                }
            }

            // 速度指示
            i++;
            SetVisualLine(_visualLine.lines, i, Color.green, body.position, body.position + velocity);
        }

        /// <summary>
        ///     摆动轨迹插值：由子类实现（如抛物线抬脚）。
        /// </summary>
        public abstract Vector3 CalculateFootPlacementMovingPoint(Vector3 startPos, Vector3 targetPos,
            float timeNormalized);
    }
}