using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EXToyLib
{
    [Serializable]
    public class SecondOrderDynamicInstance
    {
        [BoxGroup("次级运动")]
        [LabelText("使用替身")]
        public bool avator = true; // 是否使用替身
        
        [BoxGroup("次级运动")]
        [ShowIf("avator")]
        [LabelText("替身目标")]
        public Transform target; // 目标物体

        [BoxGroup("次级运动")]
        [LabelText("自动更新参数")]
        public bool autoUpdate;
        
        [BoxGroup("次级运动")]
        [EnumToggleButtons]
        [LabelText("影响属性")]
        public SecondOrderDynamicValueType ValueType = SecondOrderDynamicValueType.Position;
        
        [BoxGroup("次级运动")]
        [LabelText("震荡频率")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerFrequency))]
        public float Frequency = 1f;

        [BoxGroup("次级运动")]
        [MinValue(0),MaxValue(1)]
        [LabelText("阻尼")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerDamping))]
        public float Damping = 1f;

        [BoxGroup("次级运动")]
        [MinValue(-10),MaxValue(10)]
        [LabelText("缩放因子")]
        [OnValueChanged("UpdateDynamicsFactors")]
        [CustomValueDrawer(nameof(DrawerScale))]
        public float Scale = 0f;
        
        public SecondOrderDynamics Dynamics { get; private set; } = new SecondOrderDynamics(); // 二阶动力学实例
        
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
        
        
        private static float DrawerFrequency(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, value, 0.1f, 10f);
        }       
        
        private static float DrawerDamping(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, value, 0f, 1f);
        }
        
        private static float DrawerScale(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, value, -10f, 10f);
        }
    }
}