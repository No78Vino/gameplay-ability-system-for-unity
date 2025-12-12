using UnityEngine;

namespace EXProceduralMachine
{
// 常见呼吸节奏预设
    public static class RhythmPresets
    {
        // 静止（死亡）
        public static AnimationCurve DeathBreath()
        {
            return AnimationCurve.Constant(0f, 1f, 0f);
        }

        // 正弦呼吸（平稳）
        public static AnimationCurve SineBreath()
        {
            return AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
        }

        // 急促呼吸（紧张）
        public static AnimationCurve RapidBreath()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.1f, 1f),
                new Keyframe(0.2f, -1f),
                new Keyframe(0.3f, 0f),
                new Keyframe(0.4f, 0.5f),
                new Keyframe(0.5f, -0.5f),
                new Keyframe(0.6f, 0.8f),
                new Keyframe(0.7f, -0.8f),
                new Keyframe(0.8f, 0.3f),
                new Keyframe(0.9f, -0.3f),
                new Keyframe(1f, 0f)
            );
        }

        // 心跳式呼吸
        public static AnimationCurve HeartbeatBreath()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.05f, 1f),
                new Keyframe(0.1f, 0f),
                new Keyframe(0.15f, 0.2f),
                new Keyframe(0.2f, 0f),
                new Keyframe(0.6f, 0f),
                new Keyframe(0.65f, 0.5f),
                new Keyframe(0.7f, 0f),
                new Keyframe(0.75f, 0.1f),
                new Keyframe(0.8f, 0f),
                new Keyframe(1f, 0f)
            );
        }

        // 波浪呼吸（逐渐增强减弱）
        public static AnimationCurve WaveBreath()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.25f, 0.3f),
                new Keyframe(0.5f, 0.8f),
                new Keyframe(0.75f, 0.3f),
                new Keyframe(1f, 0f)
            );
        }

        // 创建正弦波曲线
        public static AnimationCurve CreateSineCurve(int points = 20)
        {
            var curve = new AnimationCurve();
            for (var i = 0; i <= points; i++)
            {
                var t = (float)i / points;
                var value = Mathf.Sin(t * Mathf.PI * 2f);
                curve.AddKey(t, value);
            }

            return curve;
        }

        // 创建三角波曲线
        public static AnimationCurve CreateTriangleCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.25f, 1f),
                new Keyframe(0.75f, -1f),
                new Keyframe(1f, 0f)
            );
        }

        // 创建方波曲线
        public static AnimationCurve CreateSquareCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.49f, 1f),
                new Keyframe(0.5f, -1f),
                new Keyframe(0.99f, -1f),
                new Keyframe(1f, 1f)
            );
        }
    }
}