using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    /// EX程序化动画：移动模块 - 多足运动基类
    /// </summary>
    public abstract class BaseMultiLeggedLocomotion:MonoBehaviour
    {
        protected const string BASE_BOX_GROUP = "运动基础参数";
        
        [BoxGroup(BASE_BOX_GROUP)] 
        [LabelText("步态运动落地点分组")]
        [InfoBox("将肢体分为N组交替运动")]
        [TableList]
        public List<Transform[]> MotionGroup;
        
        /// <summary>
        /// 步态周期
        /// Gait Cycle (T)
        /// 单条腿完成 “支撑 - 摆动” 一次的总时间（s）
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [LabelText("步态周期(s)")]
        public float T = 0.5f;

        /// <summary>
        /// 步长
        /// Step Length (L)
        /// 单步前进的直线距离（m），与肢体长度、摆动角度正相关
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [LabelText("步长")]
        public float L;

        /// <summary>
        /// 步频
        /// Step Frequency (f)
        /// 单位时间内的步数（Hz），f=1/T
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [ShowInInspector]
        [LabelText("步频(Hz)")]
        [DisplayAsString]
        public float f => 1f / T;

        /// <summary>
        /// 肢体相位差
        /// 相邻肢体运动的时间差占步态周期的比例（%），决定步态类型
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [LabelText("肢体相位差(%)")]
        [PropertyRange(0,1)]
        public float LegPhaseDifference = 0.5f;

        /// <summary>
        /// 足数
        /// Number of Legs (n)
        /// 机器人支撑与驱动用肢体总数
        /// </summary>
        [BoxGroup(BASE_BOX_GROUP)]
        [ShowInInspector]
        [LabelText("足数")]
        [DisplayAsString]
        public virtual int N => 2;

        /// <summary>
        /// 腿节数
        /// Number of Leg Segments
        /// 单条腿的活动关节数量（如髋 / 膝 / 踝）
        /// </summary>
        [ShowInInspector]
        [LabelText("腿节数")]
        [DisplayAsString]
        [BoxGroup(BASE_BOX_GROUP)]
        public virtual int SegmentsNumber => 3;
        
        /// <summary>
        /// 移动速度
        /// Moving Speed (v)
        /// 机器人整体前进的平均速度（m/s），v=L×f
        /// </summary>
        [ShowInInspector]
        [LabelText("移动速度(m/s)")]
        [DisplayAsString]
        [BoxGroup(BASE_BOX_GROUP)]
        public float v => L * f;

        /// <summary>
        /// 转向半径
        /// Turning Radius (R)
        /// 转向时的旋转中心到机器人质心的距离（m），R=0 为原地转向
        /// </summary>
        [LabelText("转向半径")]
        [BoxGroup(BASE_BOX_GROUP)]
        public float R;

        /// <summary>
        /// 离地间隙
        /// Ground Clearance (h)
        /// 机器人机身最低处到地面的垂直距离（m），影响越障能力
        /// </summary>
        [LabelText("离地间隙")]
        [BoxGroup(BASE_BOX_GROUP)]
        public float h;
        
        /// <summary>
        /// 从指定点向下发射垂直射线获取地面交点
        /// </summary>
        /// <param name="origin">发射点位置</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="layerMask">要检测的层级</param>
        /// <returns>与地面的交点位置</returns>
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask)
        {
            // 射线方向（向下）
            var direction = Vector3.down;
        
            // 执行射线检测
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;
            
            // 没有找到交点或超过最大距离，使用最大距离点
            var fallbackPoint = origin + direction * maxDistance;
            return fallbackPoint;
        }
    }
}