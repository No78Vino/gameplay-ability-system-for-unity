using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     将本物体高度同步到参考体（或一组目标点）的平均高度，
    ///     用于机身下方挂件（如武器吊舱、装甲板）跟随躯干贴地。
    /// </summary>
    public class SyncBodyHeight : MonoBehaviour
    {
        [Tooltip("主参考体（未配置 Targets 时使用）")]
        public Transform body;

        [Tooltip("可选目标点集合，取平均高度")]
        public List<Transform> targets = new List<Transform>();

        [Tooltip("相对参考高度的垂直偏移")]
        public float offset;

        [Tooltip("同步平滑速度（越大越快）")]
        public float smoothSpeed = 8f;

        private void Update()
        {
            var pos = transform.position;
            var targetY = GetTargetY() + offset;
            pos.y = Mathf.Lerp(pos.y, targetY, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
            transform.position = pos;
        }

        private float GetTargetY()
        {
            if (targets != null && targets.Count > 0)
            {
                var sum = 0f;
                var count = 0;
                foreach (var t in targets)
                {
                    if (t == null)
                        continue;
                    sum += t.position.y;
                    count++;
                }

                if (count > 0)
                    return sum / count;
            }

            return body != null ? body.position.y : transform.position.y;
        }
    }
}