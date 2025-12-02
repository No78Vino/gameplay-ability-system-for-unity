using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine
{
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
        public List<XVisualLineData> lines;
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 如果不显示Gizmo则直接返回
            if (!showGizmo) return;

            foreach (var data in lines)
            {
                // 设置颜色
                Gizmos.color = data.gizmoColor;
                // 划线
                Gizmos.DrawLine(data.pointA,data.pointB);
            }
        }

        // 避免在构建版本中产生警告
        private void Start()
        {
        }

        private void Update()
        {
        }
#endif
    }
}