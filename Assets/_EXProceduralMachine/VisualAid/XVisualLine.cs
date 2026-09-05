using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     可视化辅助：在场景视图中绘制一组线段（程序化动画调试用）。
    /// </summary>
    public class XVisualLine : MonoBehaviour
    {
        [Serializable]
        public class XVisualLineData
        {
            public Color gizmoColor = Color.white;
            public Vector3 pointA;
            public Vector3 pointB;
        }

        public bool showGizmo = true;
        public List<XVisualLineData> lines = new List<XVisualLineData>();

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmo)
                return;

            foreach (var data in lines)
            {
                var color = data.gizmoColor;
                color.a = 0.5f;
                Gizmos.color = color;
                Gizmos.DrawLine(data.pointA, data.pointB);
                Gizmos.DrawCube(data.pointA, Vector3.one * 0.2f);
                Gizmos.DrawCube(data.pointB, Vector3.one * 0.2f);
            }
        }
#endif
    }
}