using UnityEngine;
using UnityEngine.Serialization;

namespace EXProceduralMachine
{
    public class FourLegsSpiderLocomotion:BaseMultiLeggedLocomotion
    {
        public float maxRelativeHeight = 0.1f;
        public override int N => 4;

        public override Vector3 CalculateFootPlacementMovingPoint(Vector3 startPos, Vector3 targetPos,
            float timeNormalized)
        {
            // 1. 水平方向（X/Z）线性插值（匀速）
            var x = Mathf.Lerp(startPos.x, targetPos.x, timeNormalized);
            var z = Mathf.Lerp(startPos.z, targetPos.z, timeNormalized);

            // 2. 垂直方向（Y）抛物线插值（核心公式）
            var deltaY = targetPos.y - startPos.y;
            // 抛物线公式推导：y = -4h*t² + (Δy+4h)*t + y0 （h为相对高度）
            var y = -4 * maxRelativeHeight * Mathf.Pow(timeNormalized, 2)
                    + (deltaY + 4 * maxRelativeHeight) * timeNormalized
                    + startPos.y;

            return new Vector3(x, y, z);
        }
    }
}