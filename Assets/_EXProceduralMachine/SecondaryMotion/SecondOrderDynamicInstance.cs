using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EXProceduralMachine
{
    [Serializable]
    public class SecondOrderDynamicInstance
    {
        [BoxGroup("次级运动")] [LabelText("使用替身")] public bool avator = true; // 是否使用替身

        [BoxGroup("次级运动")] [ShowIf("avator")] [LabelText("替身目标")]
        public Transform target; // 目标物体

        [BoxGroup("次级运动")] [LabelText("自动更新参数")]
        public bool autoUpdate;

        [BoxGroup("次级运动")] [EnumToggleButtons] [LabelText("影响属性")]
        public SecondOrderDynamicValueType ValueType = SecondOrderDynamicValueType.Position;

        [BoxGroup("次级运动/影响维度")]
        [HorizontalGroup("次级运动/影响维度/axis")]
        [HideIf("@ValueType==SecondOrderDynamicValueType.Custom")]
        [LabelText("x")]
        [LabelWidth(20)]
        public bool x = true;

        [HorizontalGroup("次级运动/影响维度/axis")]
        [HideIf("@ValueType==SecondOrderDynamicValueType.Custom")]
        [LabelText("y")]
        [LabelWidth(20)]
        public bool y = true;

        [HorizontalGroup("次级运动/影响维度/axis")]
        [HideIf("@ValueType==SecondOrderDynamicValueType.Custom")]
        [LabelText("z")]
        [LabelWidth(20)]
        public bool z = true;

        [BoxGroup("次级运动")]
        [LabelText("震荡频率")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerFrequency))]
        public float Frequency = 1f;

        [BoxGroup("次级运动")]
        [MinValue(0), MaxValue(1)]
        [LabelText("阻尼")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerDamping))]
        public float Damping = 1f;

        [BoxGroup("次级运动")]
        [MinValue(-10), MaxValue(10)]
        [LabelText("缩放因子")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerScale))]
        public float Scale = 0f;

        public SecondOrderDynamics Dynamics { get; private set; } = new SecondOrderDynamics(); // 二阶动力学实例

        /// <summary>Custom 类型的自定义输入源（每帧调用）</summary>
        [NonSerialized]
        public System.Func<Vector3> customInput;

        /// <summary>Custom 类型的自定义输出回写（值, 自身Transform）</summary>
        [NonSerialized]
        public System.Action<Vector3, Transform> customOutput;

        [BoxGroup("次级运动")]
        [HideIf(nameof(autoUpdate))]
        [Button("刷新参数")]
        public void UpdateDynamicsFactors()
        {
            Dynamics.SetF(Frequency);
            Dynamics.SetZ(Damping);
            Dynamics.SetR(Scale);
            Dynamics.UpdateFactors();
        }

        /// <summary>仅更新参数不重置状态（运行时动态调参安全）</summary>
        public void ConfigureDynamicsFactors()
        {
            Dynamics.Configure(Frequency, Damping, Scale);
        }

        /// <summary>按影响属性取当前 Transform 值（用于启动初始化/OnEnable 重置）</summary>
        public Vector3 CurrentValue(Transform t)
        {
            switch (ValueType)
            {
                case SecondOrderDynamicValueType.Position: return t.position;
                case SecondOrderDynamicValueType.Rotation: return t.localEulerAngles;
                case SecondOrderDynamicValueType.QuaternionRotation: return QuaternionLog(t.localRotation);
                case SecondOrderDynamicValueType.Scale: return t.localScale;
                default: return Vector3.zero;
            }
        }

        /// <summary>用当前 Transform 值重置动力学状态（消除启动突跳）</summary>
        public void ResetFrom(Transform t)
        {
            Vector3 v = CurrentValue(t);
            if (ValueType == SecondOrderDynamicValueType.Rotation) v = WrapEuler(v);
            Dynamics.Reset(v);
        }

        /// <summary>便捷稳态判定</summary>
        public bool IsSettled(float positionTolerance = 0.01f, float velocityTolerance = 0.01f)
        {
            return Dynamics.IsSettled(positionTolerance, velocityTolerance);
        }

        public Vector3 CheckEffectedAxis(Vector3 source, Vector3 origin)
        {
            Vector3 value = source;
            if (!x) value.x = origin.x;
            if (!y) value.y = origin.y;
            if (!z) value.z = origin.z;
            return value;
        }

        /// <summary>
        /// 将角度分量映射到 [-180, 180)，避免欧拉角回绕（如 350° ↔ 10° 实际相邻但数值差 340°）
        /// 被二阶系统当作长距离运动，导致方向切换时剧烈翻转。
        /// </summary>
        public static float WrapAngle(float a)
        {
            float wrapped = (a + 180f) % 360f;
            if (wrapped < 0f) wrapped += 360f;
            return wrapped - 180f;
        }

        public static Vector3 WrapEuler(Vector3 euler)
        {
            return new Vector3(WrapAngle(euler.x), WrapAngle(euler.y), WrapAngle(euler.z));
        }

        /// <summary>四元数 → 旋转向量（对数空间，角度规范化到 [-π, π] 保持连续）</summary>
        public static Vector3 QuaternionLog(Quaternion q)
        {
            q.ToAngleAxis(out float angleDeg, out Vector3 axis);
            float angle = angleDeg * Mathf.Deg2Rad;
            // 规范化到 [-π, π]：等价旋转取最短表示（避免 350° 当 350° 走）
            if (angle > Mathf.PI) angle -= Mathf.PI * 2f;
            return axis * angle;
        }

        /// <summary>旋转向量 → 四元数（对数空间逆变换）</summary>
        public static Quaternion QuaternionExp(Vector3 v)
        {
            float angle = v.magnitude;
            if (angle < 1e-6f) return Quaternion.identity;
            Vector3 axis = v / angle;
            return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
        }


        private static float DrawerFrequency(float value, GUIContent label)
        {
#if UNITY_EDITOR
            return EditorGUILayout.Slider(label, value, 0.1f, 10f);
#else
            return value;
#endif
        }

        private static float DrawerDamping(float value, GUIContent label)
        {
#if UNITY_EDITOR
            return EditorGUILayout.Slider(label, value, 0f, 1f);
#else
            return value;
#endif
        }

        private static float DrawerScale(float value, GUIContent label)
        {
#if UNITY_EDITOR
            return EditorGUILayout.Slider(label, value, -10f, 10f);
#else
            return value;
#endif
        }
    }
}