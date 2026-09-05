using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     四足蜘蛛移动：抛物线抬脚的摆动轨迹实现。
    /// </summary>
    public class FourLegsSpiderLocomotion : BaseMultiLeggedLocomotion
    {
        [Tooltip("抬脚相对高度（抛物线顶点相对起点的提升量）")]
        public float maxRelativeHeight = 0.3f;

        public override int N => 4;

        public override Vector3 CalculateFootPlacementMovingPoint(Vector3 startPos, Vector3 targetPos,
            float timeNormalized)
        {
            // 水平：SmoothStep 加速-减速（起步慢→中间快→收步慢，观感更自然）
            var t = Mathf.SmoothStep(0f, 1f, timeNormalized);
            var x = Mathf.Lerp(startPos.x, targetPos.x, t);
            var z = Mathf.Lerp(startPos.z, targetPos.z, t);

            // 垂直：抛物线 y = y0 + Δy·t + 4h·t·(1-t)，顶点抬升 h（沿用时间 t 保证顶点中程，落点精确）
            var deltaY = targetPos.y - startPos.y;
            var y = startPos.y + deltaY * timeNormalized
                    + 4f * maxRelativeHeight * timeNormalized * (1f - timeNormalized);

            return new Vector3(x, y, z);
        }
    }
}