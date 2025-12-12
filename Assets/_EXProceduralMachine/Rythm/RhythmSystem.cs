using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    public class RhythmSystem : MonoBehaviour
    {
        public enum BlendMode
        {
            Additive, // 值相加
            Multiplicative, // 值相乘
            Max, // 取最大值
            Min // 取最小值
        }

        [Header("周期列表")] public List<RhythmCycle> cycles = new();

        [Header("混合参数")] [Tooltip("组合方式：叠加、相乘、最大、最小")]
        public BlendMode blendMode = BlendMode.Additive;

        [Header("输出")] [ReadOnly] public float combinedValue;

        [ReadOnly] public float rawValue;

        [Header("时间控制")] public bool isPlaying = true;

        public float timeScale = 1.0f;

        private void Start()
        {
            // 确保至少有一个周期
            if (cycles.Count == 0) cycles.Add(new RhythmCycle());
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            var scaledDeltaTime = Time.deltaTime * timeScale;

            // 更新所有周期
            foreach (var cycle in cycles) cycle.Update(scaledDeltaTime);

            // 计算合并值
            CombineValues();
        }

        private void CombineValues()
        {
            if (cycles.Count == 0)
            {
                combinedValue = 0f;
                return;
            }

            var value = cycles[0].GetValue();

            switch (blendMode)
            {
                case BlendMode.Additive:
                    for (var i = 1; i < cycles.Count; i++)
                        if (cycles[i].enabled)
                            value += cycles[i].GetValue();
                    break;

                case BlendMode.Multiplicative:
                    for (var i = 1; i < cycles.Count; i++)
                        if (cycles[i].enabled)
                            value *= cycles[i].GetValue();
                    break;

                case BlendMode.Max:
                    for (var i = 1; i < cycles.Count; i++)
                        if (cycles[i].enabled)
                            value = Mathf.Max(value, cycles[i].GetValue());
                    break;

                case BlendMode.Min:
                    for (var i = 1; i < cycles.Count; i++)
                        if (cycles[i].enabled)
                            value = Mathf.Min(value, cycles[i].GetValue());
                    break;
            }

            rawValue = value;
            combinedValue = Mathf.Clamp(value, -1f, 1f);
        }

        // 公共接口
        public float GetValue()
        {
            return combinedValue;
        }

        public float GetValueUnclamped()
        {
            return rawValue;
        }

        public float GetCyclePhase(int index)
        {
            if (index >= 0 && index < cycles.Count)
                return cycles[index].phase;
            return 0f;
        }

        public float GetCycleValue(int index)
        {
            if (index >= 0 && index < cycles.Count)
                return cycles[index].GetValue();
            return 0f;
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
            foreach (var cycle in cycles) cycle.Reset();
            CombineValues();
        }

        public void ResetAll()
        {
            foreach (var cycle in cycles) cycle.Reset();
            CombineValues();
        }

        public void SetCyclePhase(int index, float phase)
        {
            if (index >= 0 && index < cycles.Count)
                cycles[index].SetPhase(phase);
        }

        public void SyncCyclePhases()
        {
            if (cycles.Count < 2)
                return;

            var basePhase = cycles[0].phase;
            for (var i = 1; i < cycles.Count; i++) cycles[i].SetPhase(basePhase);
        }

        public void OffsetCyclePhase(int index, float offset)
        {
            if (index >= 0 && index < cycles.Count)
                cycles[index].AddPhaseOffset(offset);
        }

        // 工具方法：获取当前呼吸状态描述
        public string GetRhythmDescription()
        {
            if (cycles.Count == 0)
                return "No cycles";

            var desc = "";
            for (var i = 0; i < cycles.Count; i++)
                desc += $"Cycle {i}: phase={cycles[i].phase:F2}, value={cycles[i].GetValue():F2}\n";
            desc += $"Combined: {combinedValue:F2}";
            return desc;
        }
    }
}