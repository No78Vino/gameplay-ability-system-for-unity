using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // 添加Odin命名空间

namespace EXProceduralMachine
{
    /// <summary>时间模式：次级运动使用何种 deltaTime 驱动</summary>
    public enum SecondOrderTimeMode
    {
        /// <summary>受 timeScale 影响（默认，暂停时次级运动也停）</summary>
        ScaledDeltaTime,
        /// <summary>不受 timeScale 影响（暂停时惯性延续）</summary>
        UnscaledDeltaTime,
        /// <summary>物理帧同步（在 FixedUpdate 中驱动）</summary>
        FixedDeltaTime
    }

    [System.Serializable]
    public class SecondOrderDynamicsComponent : MonoBehaviour
    {
        [ShowInInspector]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        [LabelText("二阶运动实例队列")]
        public List<SecondOrderDynamicInstance> instances = new List<SecondOrderDynamicInstance>();

        [Title("驱动")]
        [LabelText("时间模式")]
        [Tooltip("Scaled：受 timeScale 影响；Unscaled：暂停也延续；Fixed：物理帧同步")]
        public SecondOrderTimeMode timeMode = SecondOrderTimeMode.ScaledDeltaTime;

        [LabelText("启用时重置状态")]
        [Tooltip("OnEnable 时用当前 Transform 值重置动力学状态（避免唤醒突跳）")]
        public bool resetOnEnable = true;

        [LabelText("初始值取自自身Transform")]
        [Tooltip("Awake 时用当前 Transform 值初始化输出（消除启动瞬间从原点突跳）")]
        public bool initializeFromCurrentTransform = true;

        [Title("预览")]
        [LabelText("绘制值变化示例曲线")]
        public bool drawCurve;
        
        [ShowIf(nameof(drawCurve))]
        [LabelText("所绘制曲线的索引")]
        [ValueDropdown(nameof(CurveChoices))]
        public int drawCurveIndex;

        [Title("调试")]
        [LabelText("绘制 Gizmos")]
        [Tooltip("Scene 中绘制替身目标与输出位置连线")]
        public bool gizmosOn;

        private List<int> CurveChoices
        {
            get
            {
                var choices = new List<int>();
                for (var i = 0; i < instances.Count; i++)
                    choices.Add(i);
                return choices;
            }
        }

        private void Awake()
        {
            foreach (var inst in instances)
            {
                if (inst == null) continue;
                // 反序列化后参数同步（避免 Inspector 参数与内核脱节）
                inst.UpdateDynamicsFactors();
                if (initializeFromCurrentTransform)
                    inst.ResetFrom(transform);
            }
        }

        private void OnEnable()
        {
            if (!resetOnEnable) return;
            foreach (var inst in instances)
            {
                if (inst == null) continue;
                inst.ResetFrom(transform);
            }
        }

        private void Update()
        {
            if (timeMode != SecondOrderTimeMode.FixedDeltaTime)
                Tick(GetDeltaTime());
        }

        private void FixedUpdate()
        {
            if (timeMode == SecondOrderTimeMode.FixedDeltaTime)
                Tick(Time.fixedDeltaTime);
        }

        private float GetDeltaTime()
        {
            return timeMode == SecondOrderTimeMode.UnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        private void Tick(float dt)
        {
            // 内核已内置 dt<=0 / NaN 防护；这里再兜底避免无意义遍历
            if (dt <= 0f) return;

            foreach (var inst in instances)
            {
                if (inst == null) continue;
                if (inst.autoUpdate) inst.UpdateDynamicsFactors();
                TickInstance(inst, dt);
            }
        }

        private void TickInstance(SecondOrderDynamicInstance inst, float dt)
        {
            switch (inst.ValueType)
            {
                case SecondOrderDynamicValueType.Position:
                    TickPosition(inst, dt);
                    break;
                case SecondOrderDynamicValueType.Rotation:
                    TickRotationEuler(inst, dt);
                    break;
                case SecondOrderDynamicValueType.QuaternionRotation:
                    TickRotationQuaternion(inst, dt);
                    break;
                case SecondOrderDynamicValueType.Scale:
                    TickScale(inst, dt);
                    break;
                case SecondOrderDynamicValueType.Custom:
                    TickCustom(inst, dt);
                    break;
                default:
                    break;
            }
        }

        private void TickPosition(SecondOrderDynamicInstance inst, float dt)
        {
            Vector3 output;
            if (inst.avator)
            {
                if (inst.target == null) return;
                output = inst.Dynamics.Update(dt, inst.target.position);
            }
            else
            {
                output = inst.Dynamics.Update(dt);
            }
            transform.position = inst.CheckEffectedAxis(output, transform.position);
        }

        private void TickRotationEuler(SecondOrderDynamicInstance inst, float dt)
        {
            Vector3 output;
            if (inst.avator)
            {
                if (inst.target == null) return;
                // 欧拉角回绕规范化：避免 350°↔10° 被当作 340° 长距离翻转
                output = inst.Dynamics.Update(dt, SecondOrderDynamicInstance.WrapEuler(inst.target.localEulerAngles));
            }
            else
            {
                output = inst.Dynamics.Update(dt, SecondOrderDynamicInstance.WrapEuler(inst.Dynamics.CurrentInput));
            }
            transform.localEulerAngles = inst.CheckEffectedAxis(output, transform.localEulerAngles);
        }

        private void TickRotationQuaternion(SecondOrderDynamicInstance inst, float dt)
        {
            Vector3 currentLog = SecondOrderDynamicInstance.QuaternionLog(transform.localRotation);
            Vector3 inputLog;
            if (inst.avator)
            {
                if (inst.target == null) return;
                inputLog = SecondOrderDynamicInstance.QuaternionLog(inst.target.localRotation);
            }
            else
            {
                // 自输入：期望欧拉角 → 四元数 → 对数空间
                inputLog = SecondOrderDynamicInstance.QuaternionLog(
                    Quaternion.Euler(inst.Dynamics.CurrentInput));
            }
            Vector3 outputLog = inst.Dynamics.Update(dt, inputLog);
            // 逐轴锁定在对数空间生效（注：log 轴 ≠ 欧拉轴，语义为"锁旋转向量分量"）
            outputLog = inst.CheckEffectedAxis(outputLog, currentLog);
            transform.localRotation = SecondOrderDynamicInstance.QuaternionExp(outputLog);
        }

        private void TickScale(SecondOrderDynamicInstance inst, float dt)
        {
            Vector3 output;
            if (inst.avator)
            {
                if (inst.target == null) return;
                output = inst.Dynamics.Update(dt, inst.target.localScale);
            }
            else
            {
                output = inst.Dynamics.Update(dt);
            }
            transform.localScale = inst.CheckEffectedAxis(output, transform.localScale);
        }

        private void TickCustom(SecondOrderDynamicInstance inst, float dt)
        {
            if (inst.customInput == null) return;
            Vector3 output = inst.Dynamics.Update(dt, inst.customInput());
            if (inst.customOutput != null)
                inst.customOutput(output, transform);
        }

        private void OnDrawGizmos()
        {
            if (!gizmosOn) return;

            foreach (var inst in instances)
            {
                if (inst == null || !inst.avator || inst.target == null) continue;
                switch (inst.ValueType)
                {
                    case SecondOrderDynamicValueType.Position:
                        // 目标点（绿）→ 输出点（自身，黄）连线
                        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.8f);
                        Gizmos.DrawWireCube(inst.target.position, Vector3.one * 0.2f);
                        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.8f);
                        Gizmos.DrawLine(inst.target.position, transform.position);
                        break;
                    case SecondOrderDynamicValueType.Rotation:
                    case SecondOrderDynamicValueType.QuaternionRotation:
                        Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.8f);
                        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
