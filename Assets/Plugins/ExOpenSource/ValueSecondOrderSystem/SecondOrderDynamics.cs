using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXToyLib
{
    [Serializable]
    public class SecondOrderDynamics
    {
        private float _frequency = 1;
        private float _damping = 1;
        private float _scale = 0;

        public void SetF(float f) => _frequency = f;
        public void SetZ(float z) => _damping = z;
        public void SetR(float r) => _scale = r;

        // 状态变量（输入历史、输出及输出速度）
        private Vector3 _previousInput; // xp：上一帧输入
        private Vector3 _currentOutput; // y：当前输出
        private Vector3 _outputVelocity; // yd：当前输出速度

        // 动力学常数（由频率、阻尼、缩放因子计算）
        private float _k1; // 阻尼项系数（k1 = z/(πf)）
        private float _k2; // 刚度项系数（k2 = 1/(2πf)²，基础值）
        private float _k3; // 缩放项系数（k3 = r*z/(2πf)）

        /// <summary>
        ///  非替身情况下，使用_selfInput来作为原始输入
        /// </summary>
        private Vector3 _selfInput;
        
        public SecondOrderDynamics()
        {
            Set(1, 1, 0, Vector3.zero); // 默认值
        }

        /// <summary>
        /// 构造函数：初始化动力学常数和状态变量
        /// </summary>
        /// <param name="frequency">频率（Hz）：控制响应速度（越大越快）</param>
        /// <param name="damping">阻尼比（0~1）：0=无阻尼（震荡），1=临界阻尼（无超调）</param>
        /// <param name="scale">缩放因子：控制超调幅度（越大超调越明显）</param>
        /// <param name="initialValue">初始输出值</param>
        public void Set(float frequency, float damping, float scale, Vector3 initialValue)
        {
            _frequency = frequency;
            _damping = damping;
            _scale = scale;

            UpdateFactors();

            // 初始化状态变量（与原代码一致）
            _previousInput = initialValue;
            _currentOutput = initialValue;
            _outputVelocity = Vector3.zero;
        }

        public void UpdateFactors()
        {
            // 计算动力学常数（与原代码一致）
            float omega = 2 * Mathf.PI * _frequency; // 角频率（rad/s）
            _k1 = _damping / (Mathf.PI * _frequency); // k1 = z/(πf)
            _k2 = 1 / (omega * omega); // k2 = 1/(2πf)²（基础值）
            _k3 = _scale * _damping / omega; // k3 = r*z/(2πf)
        }

        public void SetInput(Vector3 input)
        {
            _selfInput = input;
        }
        
        /// <summary>
        /// 重置系统状态（用于Editor预览或动态调整参数后）
        /// </summary>
        /// <param name="newValue">新的初始输出值</param>
        public void Reset(Vector3 newValue)
        {
            _previousInput = newValue;
            _currentOutput = newValue;
            _outputVelocity = Vector3.zero;
        }


        /// <summary>
        /// 更新系统状态（核心改进：动态调整k2以保证稳定性）
        /// </summary>
        /// <param name="deltaTime">当前时间步长（T）</param>
        /// <param name="targetInput">目标输入值（x）</param>
        /// <param name="targetVelocity">目标输入速度（可选，若未提供则自动估计）</param>
        /// <returns>平滑后的输出值（y）</returns>
        public Vector3 Update(float deltaTime, Vector3 targetInput, Vector3? targetVelocity = null)
        {
            // 1. 估计目标输入速度（若未提供）
            Vector3 estimatedVelocity = targetVelocity ?? (targetInput - _previousInput) / deltaTime;
            _previousInput = targetInput; // 更新输入历史

            // 2. 计算稳定的k2值（k2_stable）：核心改进点
            // 公式来源：离散时间二阶系统的稳定性条件（确保特征根在单位圆内）
            // k2_stable = max(k2, 1.1*(T²/4 + T*k1/2))，其中1.1是安全裕度
            float tSquaredOver4 = deltaTime * deltaTime / 4;
            float tK1Over2 = deltaTime * _k1 / 2;
            float k2Stable = Mathf.Max(_k2, 1.1f * (tSquaredOver4 + tK1Over2));

            // 3. 数值积分更新状态（使用k2_stable代替原k2）
            _currentOutput += deltaTime * _outputVelocity; // 位置积分（y += T*yd）
            _outputVelocity += deltaTime *
                               (targetInput + _k3 * estimatedVelocity - _currentOutput - _k1 * _outputVelocity) /
                               k2Stable; // 速度积分（yd += T*(x + k3*xd - y - k1*yd)/k2_stable）

            // 4. 返回平滑后的输出
            return _currentOutput;
        }
        
        public Vector3 Update(float deltaTime, Vector3? targetVelocity = null)
        {
            return Update(deltaTime, _selfInput, targetVelocity);
        }
    }
}