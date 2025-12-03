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
        public List<XVisualLineData> lines = new List<XVisualLineData>();
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 如果不显示Gizmo则直接返回
            if (!showGizmo) return;

            foreach (var data in lines)
            {
                // 设置颜色
                var c = data.gizmoColor;
                c.a = 0.5f;
                Gizmos.color = c;
                // 划线
                Gizmos.DrawLine(data.pointA,data.pointB);
                Gizmos.DrawCube(data.pointA,Vector3.one*0.2f);
                Gizmos.DrawCube(data.pointB,Vector3.one*0.2f);
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