using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     呼吸/节奏系统：多个 RhythmCycle 按混合模式组合为一个输出值。
    ///     可驱动躯干起伏、武器摆动、动画叠加等。
    /// </summary>
    public class RhythmSystem : MonoBehaviour
    {
        public enum BlendMode
        {
            Additive, // 值相加
            Multiplicative, // 值相乘
            Max, // 取最大值
            Min // 取最小值
        }

        [LabelText("周期列表")] public List<RhythmCycle> cycles = new List<RhythmCycle>();

        [LabelText("混合模式")] [Tooltip("组合方式：叠加、相乘、最大、最小")]
        public BlendMode blendMode = BlendMode.Additive;

        [LabelText("输出(钳制)")]
        [ShowInInspector]
        [DisplayAsString]
        private float _combinedValue;

        [ReadOnly] public float rawValue;

        public bool isPlaying { get; private set; } = true;

        [Tooltip("时间缩放")]
        public float timeScale = 1.0f;

        private void Start()
        {
            // 确保至少有一个周期
            if (cycles.Count == 0)
                cycles.Add(new RhythmCycle());
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            var scaledDeltaTime = Time.deltaTime * timeScale;

            foreach (var cycle in cycles)
                cycle.Update(scaledDeltaTime);

            CombineValues();
        }

        private void CombineValues()
        {
            var value = 0f;
            var hasValue = false;

            foreach (var cycle in cycles)
            {
                if (!cycle.enabled)
                    continue;

                if (!hasValue)
                {
                    value = cycle.GetValue();
                    hasValue = true;
                    continue;
                }

                switch (blendMode)
                {
                    case BlendMode.Additive:
                        value += cycle.GetValue();
                        break;
                    case BlendMode.Multiplicative:
                        value *= cycle.GetValue();
                        break;
                    case BlendMode.Max:
                        value = Mathf.Max(value, cycle.GetValue());
                        break;
                    case BlendMode.Min:
                        value = Mathf.Min(value, cycle.GetValue());
                        break;
                }
            }

            rawValue = value;
            _combinedValue = Mathf.Clamp(value, -1f, 1f);
        }

        // ==================== 公共接口 ====================

        /// <summary>当前组合值（钳制到 -1~1）</summary>
        public float GetValue() => _combinedValue;

        /// <summary>当前组合值（不钳制）</summary>
        public float GetValueUnclamped() => rawValue;

        public float GetCyclePhase(int index)
        {
            return index >= 0 && index < cycles.Count ? cycles[index].phase : 0f;
        }

        public float GetCycleValue(int index)
        {
            return index >= 0 && index < cycles.Count ? cycles[index].GetValue() : 0f;
        }

        public void AddCycle(float duration, AnimationCurve curve)
        {
            var newCycle = new RhythmCycle();
            newCycle.duration = duration;
            newCycle.curve = curve;
            cycles.Add(newCycle);
        }

        public void RemoveCycle(int index)
        {
            if (index >= 0 && index < cycles.Count)
                cycles.RemoveAt(index);
        }

        public void ClearCycles()
        {
            cycles.Clear();
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void Stop()
        {
            isPlaying = false;
            foreach (var cycle in cycles)
                cycle.Reset();
            CombineValues();
        }

        public void ResetAll()
        {
            foreach (var cycle in cycles)
                cycle.Reset();
            CombineValues();
        }

        public void SetCyclePhase(int index, float phase)
        {
            if (index >= 0 && index < cycles.Count)
                cycles[index].SetPhase(phase);
        }

        /// <summary>所有周期相位与第一个周期同步</summary>
        public void SyncCyclePhases()
        {
            if (cycles.Count < 2)
                return;

            var basePhase = cycles[0].phase;
            for (var i = 1; i < cycles.Count; i++)
                cycles[i].SetPhase(basePhase);
        }

        public void OffsetCyclePhase(int index, float offset)
        {
            if (index >= 0 && index < cycles.Count)
                cycles[index].AddPhaseOffset(offset);
        }

        /// <summary>当前呼吸状态描述（调试用）</summary>
        public string GetRhythmDescription()
        {
            if (cycles.Count == 0)
                return "No cycles";

            var desc = "";
            for (var i = 0; i < cycles.Count; i++)
                desc += $"Cycle {i}: phase={cycles[i].phase:F2}, value={cycles[i].GetValue():F2}\n";
            desc += $"Combined: {_combinedValue:F2}";
            return desc;
        }
    }
}