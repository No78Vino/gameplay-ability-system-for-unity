using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     常见呼吸/节奏曲线预设。
    /// </summary>
    public static class RhythmPresets
    {
        /// <summary>静止（死亡）：恒为 0</summary>
        public static AnimationCurve DeathBreath()
        {
            return AnimationCurve.Constant(0f, 1f, 0f);
        }

        /// <summary>正弦呼吸（平稳）</summary>
        public static AnimationCurve SineBreath()
        {
            return SineCurve();
        }

        /// <summary>急促呼吸（紧张）</summary>
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

        /// <summary>心跳式呼吸</summary>
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

        /// <summary>波浪呼吸（逐渐增强减弱）</summary>
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

        /// <summary>正弦波曲线</summary>
        public static AnimationCurve SineCurve(int points = 20)
        {
            var curve = new AnimationCurve();
            for (var i = 0; i <= points; i++)
            {
                var t = (float)i / points;
                curve.AddKey(t, Mathf.Sin(t * Mathf.PI * 2f));
            }

            return curve;
        }

        /// <summary>三角波曲线</summary>
        public static AnimationCurve TriangleCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.25f, 1f),
                new Keyframe(0.75f, -1f),
                new Keyframe(1f, 0f)
            );
        }

        /// <summary>方波曲线</summary>
        public static AnimationCurve SquareCurve()
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