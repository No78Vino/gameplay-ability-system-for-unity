using System;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     单个节奏周期：由时长 + AnimationCurve + 相位组成。
    /// </summary>
    [Serializable]
    public class RhythmCycle
    {
        [Tooltip("周期时长（秒）")] public float duration = 2.0f;

        [Tooltip("变化曲线，X轴：0-1 表示一个周期，Y轴：节奏值")]
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1f),
            new Keyframe(0.75f, -1f),
            new Keyframe(1f, 0f)
        );

        [Tooltip("当前周期内的归一化时间（0-1）")] [Range(0f, 1f)]
        public float phase;

        [Tooltip("是否启用此周期")] public bool enabled = true;

        [Tooltip("强度系数")] public float intensity = 1.0f;

        private float timeAccumulator;

        public void Update(float deltaTime)
        {
            if (!enabled || duration <= 0f)
                return;

            timeAccumulator += deltaTime;
            if (timeAccumulator >= duration)
                timeAccumulator %= duration;

            phase = timeAccumulator / duration;
        }

        public float GetValue()
        {
            if (!enabled)
                return 0f;

            return curve.Evaluate(phase) * intensity;
        }

        public void Reset()
        {
            timeAccumulator = 0f;
            phase = 0f;
        }

        public void SetPhase(float newPhase)
        {
            phase = Mathf.Clamp01(newPhase);
            timeAccumulator = phase * duration;
        }

        public void AddPhaseOffset(float offset)
        {
            phase = (phase + offset) % 1f;
            if (phase < 0f)
                phase += 1f;
            timeAccumulator = phase * duration;
        }
    }
}