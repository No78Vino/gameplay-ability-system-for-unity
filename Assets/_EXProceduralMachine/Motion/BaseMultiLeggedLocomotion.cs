using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     EX程序化动画：移动模块 - 多足运动基类
    /// </summary>
    public abstract class BaseMultiLeggedLocomotion : MonoBehaviour
    {
        protected const string BASE_BOX_GROUP = "运动基础参数";

        [BoxGroup(BASE_BOX_GROUP)] [LabelText("步态运动落地点分组")] [InfoBox("将肢体分为N组交替运动")] [ShowInInspector]
        public FootMotionGroup[] MotionGroup;

        /// <summary>
        ///     步态周期
        ///     Gait Cycle (T)
        ///     单条腿完成 “支撑 - 摆动” 一次的总时间（s）
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)] [LabelText("步态周期(s)")]
        public float T = 0.5f;

        /// <summary>
        ///     步长
        ///     Step Length (L)
        ///     单步前进的直线距离（m），与肢体长度、摆动角度正相关
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)] [LabelText("步长")]
        public float L;

        /// <summary>
        ///     肢体相位差
        ///     相邻肢体运动的时间差占步态周期的比例（%），决定步态类型
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)] [LabelText("肢体相位差占比(%)")]
        public float[] LegPhaseDifference = { 0.5f, 0.5f };

        /// <summary>
        ///     转向半径
        ///     Turning Radius (R)
        ///     转向时的旋转中心到机器人质心的距离（m），R=0 为原地转向
        /// </summary>
        [LabelText("转向半径")] [BoxGroup(BASE_BOX_GROUP)]
        public float R;

        /// <summary>
        ///     离地间隙
        ///     Ground Clearance (h)
        ///     机器人机身最低处到地面的垂直距离（m），影响越障能力
        /// </summary>
        [LabelText("离地间隙")] [BoxGroup(BASE_BOX_GROUP)]
        public float h;

        /// <summary>
        ///     步频
        ///     Step Frequency (f)
        ///     单位时间内的步数（Hz），f=1/T
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [ShowInInspector]
        [LabelText("步频(Hz)")]
        [DisplayAsString]
        public float f => 1f / T;

        /// <summary>
        ///     足数
        ///     Number of Legs (n)
        ///     机器人支撑与驱动用肢体总数
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [ShowInInspector]
        [LabelText("足数")]
        [DisplayAsString]
        public virtual int N => 2;

        /// <summary>
        ///     腿节数
        ///     Number of Leg Segments
        ///     单条腿的活动关节数量（如髋 / 膝 / 踝）
        /// </summary>
        [ShowInInspector]
        [LabelText("腿节数")]
        [DisplayAsString]
        [BoxGroup(BASE_BOX_GROUP)]
        public virtual int SegmentsNumber => 3;

        /// <summary>
        ///     移动速度
        ///     Moving Speed (v)
        ///     机器人整体前进的平均速度（m/s），v=L×f
        /// </summary>
        [ShowInInspector]
        [LabelText("移动速度(m/s)")]
        [DisplayAsString]
        [BoxGroup(BASE_BOX_GROUP)]
        public float v => L * f;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("移动方向")]
        public Vector3 Direction { get; private set; }

        public Transform MotionTransform { get; private set; }

        public bool IsMoving => v == 0;

        protected virtual void Awake()
        {
            MotionTransform = new GameObject(name).transform;
            MotionTransform.SetParent(EXProceduralMachineManager.Instance.ManagerRoot);

            for (var i = 0; i < MotionGroup.Length; i++)
            {
                var motionGroup = MotionGroup[i];
                motionGroup.Initialize(this, LegPhaseDifference[i]);
            }
        }

        private void Update()
        {
            var motionGroup = GetCurrentMotionGroup();
            if (!motionGroup.IsMoving())
            {
                motionGroup.UpdateFootPlacements();
            }

            foreach (var group in MotionGroup)
            {
                group.Tick();
            }
            
            // if ((transform.position - _lastPos).magnitude > moveStep)
            // {
            //     _lastPos = transform.position;
            //     var zigzagDist1 = Vector3.Distance(Zigzag1.position, _lastPos);
            //     var zigzagDist2 = Vector3.Distance(Zigzag2.position, _lastPos);
            //     if (zigzagDist1 > zigzagDist2)
            //     {
            //         if(!movers[1].IsMoving||(Zigzag1.position - _lastPos).magnitude > moveMaxStep)
            //             Zigzag1.position = _lastPos;
            //     }
            //     else
            //     {
            //         if(!movers[0].IsMoving||(Zigzag2.position - _lastPos).magnitude > moveMaxStep)
            //             Zigzag2.position = _lastPos;
            //     }
            // }

            // for (var i = 0; i < castPoints.Count; i++)
            // {
            //     var cast = castPoints[i];
            //     var gPos = GetGroundPoint(cast.position, maxDistance, LayerMask.GetMask("Terrain"));
            //     if ((gPos - groundPoints[i].position).magnitude > moveStep)
            //     {
            //         groundPoints[i].position = gPos;
            //         movers[i].MoveToParabola(gPos,stepMoveTime,stepHeight);
            //     }
            // }
        }

        protected void OnDestroy()
        {
            if (MotionTransform != null)
                Destroy(MotionTransform.gameObject);
        }

        /// <summary>
        ///     获取当前应该运动的足对
        /// </summary>
        /// <returns></returns>
        private FootMotionGroup GetCurrentMotionGroup()
        {
            var minDistance = float.MaxValue;
            var index = 0;
            for (var i = 0; i < MotionGroup.Length; i++)
            {
                var d = MotionGroup[i].DistanceInDirection();
                if (d < minDistance)
                {
                    minDistance = d;
                    index = i;
                }
            }

            return MotionGroup[index];
        }

        public abstract Vector3 CalculateFootPlacementMovingPoint(Vector3 startPos, Vector3 targetPos,
            float timeNormalized);
    }
}