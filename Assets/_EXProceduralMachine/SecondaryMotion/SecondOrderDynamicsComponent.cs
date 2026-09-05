using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // 添加Odin命名空间

namespace EXProceduralMachine
{
    [System.Serializable]
    public class SecondOrderDynamicsComponent : MonoBehaviour
    {
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

        /// <summary>
        /// 将角度分量映射到 [-180, 180)，避免欧拉角回绕（如 350° ↔ 10° 实际相邻但数值差 340°）
        /// 被二阶系统当作长距离运动，导致方向切换时剧烈翻转。
        /// </summary>
        private static float WrapAngle(float a)
        {
            float wrapped = (a + 180f) % 360f;
            if (wrapped < 0f) wrapped += 360f;
            return wrapped - 180f;
        }

        private static Vector3 WrapEuler(Vector3 euler)
        {
            return new Vector3(WrapAngle(euler.x), WrapAngle(euler.y), WrapAngle(euler.z));
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
                                var pos = inst.Dynamics.Update(Time.deltaTime, inst.target.position);
                                transform.position = inst.CheckEffectedAxis(pos,transform.position);
                                break;
                            case SecondOrderDynamicValueType.Rotation:
                                var angle = inst.Dynamics.Update(Time.deltaTime, WrapEuler(inst.target.localEulerAngles));
                                transform.localEulerAngles = inst.CheckEffectedAxis(angle,transform.localEulerAngles);
                                break;
                            case SecondOrderDynamicValueType.Scale:
                                var scale = inst.Dynamics.Update(Time.deltaTime, inst.target.localScale);
                                transform.localScale = inst.CheckEffectedAxis(scale,transform.localScale);
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
                            var pos = inst.Dynamics.Update(Time.deltaTime);
                            transform.position = inst.CheckEffectedAxis(pos,transform.position);
                            break;
                        case SecondOrderDynamicValueType.Rotation:
                            var angle = inst.Dynamics.Update(Time.deltaTime, WrapEuler(inst.Dynamics.CurrentInput));
                            transform.localEulerAngles = inst.CheckEffectedAxis(angle,transform.localEulerAngles);
                            break;
                        case SecondOrderDynamicValueType.Scale:
                            var scale = inst.Dynamics.Update(Time.deltaTime);
                            transform.localScale = inst.CheckEffectedAxis(scale,transform.localScale);
                            break;
                        case SecondOrderDynamicValueType.Custom:
                        default:
                            break;
                    }
            }
        }
    }
}