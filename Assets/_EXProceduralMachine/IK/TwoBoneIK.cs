using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     自研二骨骼解析式 IK 解算器（替代第三方 IK 插件）。
    ///     直接对 髋(hip)→膝(knee)→踝(foot) 骨骼链求解旋转，
    ///     使踝部到达目标点，并通过 pole 参考点控制膝盖弯曲方向。
    /// </summary>
    public static class TwoBoneIK
    {
        /// <summary>
        ///     求解二骨骼链，使末端骨骼 foot 到达 target。
        /// </summary>
        /// <param name="hip">髋（根）骨骼</param>
        /// <param name="knee">膝（中间）骨骼</param>
        /// <param name="foot">踝（末端）骨骼</param>
        /// <param name="target">期望末端世界坐标</param>
        /// <param name="pole">膝盖弯曲参考点（世界），为 null 时保持当前弯曲平面方向</param>
        /// <returns>目标是否在骨骼链可达范围内</returns>
        public static bool Solve(Transform hip, Transform knee, Transform foot, Vector3 target, Transform pole)
        {
            if (hip == null || knee == null || foot == null)
                return false;

            return Solve(hip, knee, foot, target, pole != null ? pole.position : knee.position);
        }

        /// <summary>
        ///     求解二骨骼链（pole 直接传世界坐标，不依赖 Transform）。
        /// </summary>
        public static bool Solve(Transform hip, Transform knee, Transform foot, Vector3 target, Vector3 pole)
        {
            if (hip == null || knee == null || foot == null)
                return false;

            var hipPos = hip.position;
            var kneePos = knee.position;
            var footPos = foot.position;

            var upperLen = Vector3.Distance(hipPos, kneePos);
            var lowerLen = Vector3.Distance(kneePos, footPos);
            if (upperLen < 1e-4f || lowerLen < 1e-4f)
                return false;

            // 目标距离（钳制到可达区间，防止过伸/折叠产生 NaN）
            var maxReach = upperLen + lowerLen;
            var minReach = Mathf.Abs(upperLen - lowerLen);
            var rawDist = Vector3.Distance(hipPos, target);
            var dist = Mathf.Clamp(rawDist, minReach * 1.0001f, maxReach * 0.9999f);
            var toTarget = (target - hipPos).normalized;

            // 膝在髋→目标连线上的投影 H = hip + toTarget * c，弯曲半径 r
            var c = (upperLen * upperLen - lowerLen * lowerLen + dist * dist) / (2f * dist);
            var r = Mathf.Sqrt(Mathf.Max(0f, upperLen * upperLen - c * c));

            // 期望膝盖径向方向（垂直于 toTarget 的分量）：优先用 pole，退化时保持当前弯曲
            var kneeRadial = Vector3.ProjectOnPlane(kneePos - hipPos, toTarget);
            var poleRadial = Vector3.ProjectOnPlane(pole - hipPos, toTarget);
            var radial = poleRadial.sqrMagnitude > 1e-6f ? poleRadial : kneeRadial;
            if (radial.sqrMagnitude < 1e-6f)
                radial = Vector3.Cross(toTarget, Vector3.up);
            if (radial.sqrMagnitude < 1e-6f)
                radial = Vector3.Cross(toTarget, Vector3.right);
            radial.Normalize();

            // 新膝位置与髋→膝方向
            var kneeTarget = hipPos + toTarget * c + radial * r;
            var hipToKneeDir = (kneeTarget - hipPos).normalized;

            // 应用旋转：髋（对齐髋→膝），膝（对齐膝→踝）
            var hipDelta = Quaternion.FromToRotation((kneePos - hipPos).normalized, hipToKneeDir);
            hip.rotation = hipDelta * hip.rotation;

            var lowerDir = hipDelta * (footPos - kneePos).normalized;
            var desiredLowerDir = (target - kneeTarget).normalized;
            knee.rotation = Quaternion.FromToRotation(lowerDir, desiredLowerDir) * knee.rotation;

            return rawDist <= maxReach * 1.0001f && rawDist >= minReach * 0.9999f;
        }

        /// <summary>
        ///     将末端骨骼（脚掌）对齐地面法线，实现脚掌贴地。
        /// </summary>
        /// <param name="foot">脚掌骨骼</param>
        /// <param name="groundNormal">地面法线（世界）</param>
        /// <param name="weight">对齐权重 0~1（建议 <1 平滑过渡）</param>
        /// <param name="localDownAxis">脚掌本地下方向轴；为 zero 时默认取 -up</param>
        public static void AlignFootToGround(Transform foot, Vector3 groundNormal, float weight, Vector3 localDownAxis)
        {
            if (foot == null || groundNormal.sqrMagnitude < 1e-6f || weight <= 0f)
                return;

            var down = localDownAxis.sqrMagnitude > 1e-6f ? foot.rotation * localDownAxis.normalized : -foot.up;
            var target = Quaternion.FromToRotation(down, groundNormal.normalized) * foot.rotation;
            foot.rotation = Quaternion.Slerp(foot.rotation, target, Mathf.Clamp01(weight));
        }
    }
}