using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // 添加Odin命名空间

namespace EXToyLib
{
    [System.Serializable]
    public class SecondOrderDynamicsComponent : MonoBehaviour
    {
        // [BoxGroup("Settings")]
        // [LabelText("使用替身")]
        // public bool avator = true; // 是否使用替身
        //
        // [BoxGroup("Settings")]
        // [ShowIf("avator")]
        // [LabelText("替身目标")]
        // public Transform inst.target; // 目标物体
        //
        // [BoxGroup("Settings")]
        // [LabelText("自动更新参数")]
        // public bool autoUpdate;
        //
        // [BoxGroup("Settings")]
        // [EnumToggleButtons]
        // [LabelText("影响属性")]
        // public SecondOrderDynamicValueType ValueType = SecondOrderDynamicValueType.Position;
        //
        // [BoxGroup("Parameters")]
        // [MinValue(0.1f), MaxValue(7f)]
        // [OnValueChanged("UpdateDynamicsFactors")]
        // public float Frequency = 1f;
        //
        // [BoxGroup("Parameters")]
        // [MinValue(0f), MaxValue(1f)]
        // [OnValueChanged("UpdateDynamicsFactors")]
        // public float Damping = 1f;
        //
        // [BoxGroup("Parameters")]
        // [MinValue(-10f), MaxValue(10f)]
        // [OnValueChanged("UpdateDynamicsFactors")]
        // public float Scale = 0f;
        //
        // private SecondOrderDynamics inst.Dynamics = new SecondOrderDynamics(); // 二阶动力学实例
        // public SecondOrderDynamics Dynamics => inst.Dynamics; // 二阶动力学实例
        //
        [ShowInInspector]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        [LabelText("二阶运动实例队列")]
        public List<SecondOrderDynamicInstance> instances = new List<SecondOrderDynamicInstance>();

        [Title("预览")]
        [LabelText("绘制值变化示例曲线")]
        public bool drawCurve;
        
        [ShowIf(nameof(drawCurve))]
        [LabelText("所绘制曲线的索引")]
        [ValueDropdown(nameof(CurveChoices))]
        public int drawCurveIndex;

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
        
        void Update()
        {
            foreach (var inst in instances)
            {
                if (inst.autoUpdate) inst.UpdateDynamicsFactors();

                if (inst.avator)
                {
                    if (inst.target != null)
                        switch (inst.ValueType)
                        {
                            case SecondOrderDynamicValueType.Position:
                                transform.position = inst.Dynamics.Update(Time.deltaTime, inst.target.position);
                                break;
                            case SecondOrderDynamicValueType.Rotation:
                                transform.localEulerAngles = inst.Dynamics.Update(Time.deltaTime, inst.target.localEulerAngles);
                                break;
                            case SecondOrderDynamicValueType.Scale:
                                transform.localScale = inst.Dynamics.Update(Time.deltaTime, inst.target.localScale);
                                break;
                            case SecondOrderDynamicValueType.Custom:
                            default:
                                break;
                        }
                }
                else
                    switch (inst.ValueType)
                    {
                        case SecondOrderDynamicValueType.Position:
                            transform.position = inst.Dynamics.Update(Time.deltaTime);
                            break;
                        case SecondOrderDynamicValueType.Rotation:
                            transform.localEulerAngles = inst.Dynamics.Update(Time.deltaTime);
                            break;
                        case SecondOrderDynamicValueType.Scale:
                            transform.localScale = inst.Dynamics.Update(Time.deltaTime);
                            break;
                        case SecondOrderDynamicValueType.Custom:
                        default:
                            break;
                    }
            }
        }
    }
}