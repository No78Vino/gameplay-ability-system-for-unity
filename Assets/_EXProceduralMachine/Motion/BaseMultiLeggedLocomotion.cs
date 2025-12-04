using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     EX程序化动画：移动模块 - 多足运动基类
    /// </summary>
    public abstract class BaseMultiLeggedLocomotion : MonoBehaviour
    {
        protected const string BASE_TAB_GROUP = "运动参数";
        protected const string TAB_BASE = "基础参数";
        protected const string TAB_BIND = "绑定";

        [TabGroup(BASE_TAB_GROUP,TAB_BIND)] [LabelText("步态运动落地点分组")] [InfoBox("将肢体分为N组交替运动")] [ShowInInspector]
        public FootMotionGroup[] MotionGroup;

        [TabGroup(BASE_TAB_GROUP,TAB_BIND)] [LabelText("躯干")]
        public Transform Body;

        /// <summary>
        ///     步态周期
        ///     Gait Cycle (T)
        ///     单条腿完成 “支撑 - 摆动” 一次的总时间（s）
        /// </summary>
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        [ShowInInspector]
        [LabelText("步态周期(s)")]
        [DisplayAsString]
        public float T => v == 0 ? 0 : (L / v);

        /// <summary>
        ///     步长
        ///     Step Length (L)
        ///     单步前进的直线距离（m），与肢体长度、摆动角度正相关
        /// </summary>
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)] [LabelText("步长")]
        public float L;

        /// <summary>
        ///     肢体相位差
        ///     相邻肢体运动的时间差占步态周期的比例（%），决定步态类型
        /// </summary>
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)] [LabelText("肢体相位差占比(%)")]
        public float[] LegPhaseDifference = { 0.5f, 0.5f };

        /// <summary>
        ///     转向半径
        ///     Turning Radius (R)
        ///     转向时的旋转中心到机器人质心的距离（m），R=0 为原地转向
        /// </summary>
        [LabelText("转向半径")] [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public float R;

        /// <summary>
        ///     离地间隙
        ///     Ground Clearance (h)
        ///     机器人机身最低处到地面的垂直距离（m），影响越障能力
        /// </summary>
        [LabelText("离地间隙")] [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public float h;

        /// <summary>
        ///     步频
        ///     Step Frequency (f)
        ///     单位时间内的步数（Hz），f=1/T
        /// </summary>
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        [ShowInInspector]
        [LabelText("步频(Hz)")]
        [DisplayAsString]
        public float f => 1f / T;

        /// <summary>
        ///     足数
        ///     Number of Legs (n)
        ///     机器人支撑与驱动用肢体总数
        /// </summary>
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
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
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public virtual int SegmentsNumber => 3;

        /// <summary>
        ///     移动速度
        ///     Moving Speed (v)
        ///     机器人整体前进的平均速度（m/s），v=L×f
        /// </summary>
        [ShowInInspector]
        [LabelText("移动速度(m/s)")]
        [DisplayAsString]
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public float v => Velocity.magnitude;
        
        [ShowInInspector]
        [LabelText("速度(m/s)")]
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public Vector3 Velocity;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("移动方向")]
        public Vector3 Direction => Velocity.normalized;
        
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)] [LabelText("地面检测最远距离")]
        public float castMaxDistance;
        
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)] [LabelText("地面检测Layer")]
        public LayerMask groundLayer;
        
        [LabelText("是否自我计算速度")]
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)]
        public bool CheckSelfVelocity;
        
        [TabGroup(BASE_TAB_GROUP,TAB_BASE)] [LabelText("视觉辅助Gizmos")]
        public bool gizmosOn;

        public float stepTime = 1;
        
        public Transform MotionTransform { get; private set; }

        public bool IsMoving => v == 0;

        private Vector3 _lastBodyPosition;
        private Vector3 _lastBodyStepPos;

        private XVisualLine _visualLine;

        protected virtual void Awake()
        {
            MotionTransform = new GameObject(name).transform;
            MotionTransform.SetParent(EXProceduralMachineManager.Instance.ManagerRoot);

            for (var i = 0; i < MotionGroup.Length; i++)
            {
                var motionGroup = MotionGroup[i];
                motionGroup.Initialize(this, LegPhaseDifference[i]);
            }

            _lastBodyPosition = Body.position;
            _lastBodyStepPos = Body.position;
        }

        private void Update()
        {
            if (Body != null)
            {
                var bodyPosition = Body.position;
                var ave = GetAverageFootHeight();
                bodyPosition.y = ave + h;
                Body.position = bodyPosition;
            }

            //MotionTransform.position = Body.position;

            if ((Body.position - _lastBodyStepPos).magnitude > L)
            {
                _lastBodyStepPos = Body.position;
                var motionGroup = GetCurrentMotionGroup();
                var movingGroupIndex = GetMovingGroupIndex();
                if(movingGroupIndex<0 || motionGroup==MotionGroup[movingGroupIndex])
                    motionGroup.UpdateFootPlacements();
            }

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

            
            // var movingIndex = GetMovingGroupIndex();
            // if (movingIndex > 0)
            // {
            //     var group = MotionGroup[movingIndex];
            //     foreach (var footPlacement in group.FootPlacements)
            //     {
            //             var gPos = footPlacement.CastPosition;
            //             if ((gPos - footPlacement.IkTrackPoint.position).magnitude > group.SplitStepLength)
            //             {
            //                 footPlacement.SetMoving();
            //                 // groundPoints[i].position = gPos;
            //                 // movers[i].MoveToParabola(gPos,stepMoveTime,stepHeight);
            //             }
            //     }
            // }

            for (var i = 0; i < MotionGroup.Length; i++)
            {
                var group = MotionGroup[i];
                foreach (var footPlacement in group.FootPlacements)
                {
                   //  if (footPlacement.IsMoving())
                    {
                        var gPos = footPlacement.CastPosition;
                        if ((gPos - footPlacement.IkTrackPoint.position).magnitude > group.SplitStepLength)
                        {
                            footPlacement.SetMoving();
                            // groundPoints[i].position = gPos;
                            // movers[i].MoveToParabola(gPos,stepMoveTime,stepHeight);
                        }
                    }
                }
            }

            foreach (var group in MotionGroup)
                group.Tick();

            CheckVelocity();
#if UNITY_EDITOR
            CheckGizmos();
#endif
        }

        private void CheckVelocity()
        {
            if (CheckSelfVelocity)
            {
                Velocity = (Body.position - _lastBodyPosition) / Time.deltaTime;
                Velocity.y = 0;
            }

            _lastBodyPosition = Body.position;
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
            float maxDistRate = 0f;
            var index = 0;
            for (var i = 0; i < MotionGroup.Length; i++)
            {
                if(MotionGroup[i].IsMoving())
                    return MotionGroup[index];
                
                var rate = MotionGroup[i].DistanceRate();
                if (rate > maxDistRate)
                {
                    maxDistRate = rate;
                    index = i;
                }
            }

            return MotionGroup[index];
        }

        private float GetAverageFootHeight()
        {
            float height = 0;
            var count = 0;
            foreach (var group in MotionGroup)
            {
                count += group.FootPlacements.Length;
                foreach (var footPlacement in group.FootPlacements)
                    height += footPlacement.IkTrackPoint.position.y;
            }
            return height/count;
        }

        private void SetVisualLine(List<XVisualLine.XVisualLineData> list, int index,Color color,Vector3 A,Vector3 B)
        {
            if (index + 1 >= list.Count)
            {
                list.Add(new XVisualLine.XVisualLineData());
                index = list.Count - 1;
            }
            
            var lineData = _visualLine.lines[index];
            lineData.gizmoColor = color;
            lineData.pointA = A;
            lineData.pointB = B;
        }
        
        private void CheckGizmos()
        {
            if (gizmosOn)
            {
                if (_visualLine == null)
                {
                    _visualLine = gameObject.GetComponent<XVisualLine>();
                    if (_visualLine == null)
                        _visualLine = gameObject.AddComponent<XVisualLine>();
                }

                int i = -1;
                foreach (var motionGroup in MotionGroup)
                {
                    foreach (var footPlacement in motionGroup.FootPlacements)
                    {
                        i++;
                        var stepPos = footPlacement.StepPoint.position;
                        var ikPos = footPlacement.IkTrackPoint.position;
                        SetVisualLine(_visualLine.lines, i, Color.white, ikPos,
                            stepPos);
                        
                        i++;
                        var moveColor = footPlacement.IsMoving()?Color.green:Color.red;
                        SetVisualLine(_visualLine.lines, i, moveColor, footPlacement.CastPosition,
                            ikPos);
                        
                        i++;
                        SetVisualLine(_visualLine.lines, i, Color.cyan, footPlacement.IdlePoint.position,
                            stepPos);
                    }
                }
                i++;
                SetVisualLine(_visualLine.lines, i, Color.green, Body.position,
                    Body.position + Velocity);
            }
        }

        public int GetMovingGroupIndex()
        {
            for (var i = 0; i < MotionGroup.Length; i++)
            {
                if (MotionGroup[i].IsMoving())
                    return i;
            }
            return -1;
        }
        public abstract Vector3 CalculateFootPlacementMovingPoint(Vector3 startPos, Vector3 targetPos,
            float timeNormalized);
    }
}