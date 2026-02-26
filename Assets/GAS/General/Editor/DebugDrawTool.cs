#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GAS.General
{
    [InitializeOnLoad]
    public static class DebugDrawTool
    {
        // ===================== 数据结构 =====================

        public struct DrawConfig
        {
            public Color color;
            public float duration; // 持续时间，<=0 表示一直画，直到 Clear
            public int layer; // 自定义的调试图层
            public float lineWidth; // 暂时仅当作 key 使用，GL 本身不支持可变线宽
            public bool depthTest; // 是否走深度测试（这里仅预留，如果用 GL 画实线网格可细分）
        }

        private enum DrawType
        {
            Line,
            WireCube,
            WireSphere3Rings,
            Grid,
            Arrow,
            Label
        }

        private struct DrawCommand
        {
            public DrawType type;
            public Vector3[] positions; // 线段就是 [start,end]；网格是中心+size 等
            public Vector2 gridSize; // 网格大小
            public Vector2 gridSpacing; // 网格间隔
            public Color color;
            public float remainingTime; // 剩余时间
            public int layer;
            public float lineWidth;
            public bool depthTest;
            public string text; // Label 文本
        }

        private static class State
        {
            public static readonly List<DrawCommand> Commands = new List<DrawCommand>();
            public static readonly Dictionary<float, Material> LineMats = new Dictionary<float, Material>();
            public static bool Enabled = true;
            public static int CurrentLayer = 0;
        }

        // ===================== 初始化 =====================

        static DebugDrawTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += OnEditorUpdate;
            InitMaterials();
        }

        private static void InitMaterials()
        {
            CreateLineMaterial(1f);
            CreateLineMaterial(2f);
            CreateLineMaterial(3f);
        }

        private static void CreateLineMaterial(float width)
        {
            if (State.LineMats.ContainsKey(width)) return;

            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null)
            {
                Debug.LogError("DebugDrawTool: Shader 'Hidden/Internal-Colored' not found.");
                return;
            }

            var mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // 基本设置：不写入深度、不剔除
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_Cull", (int)CullMode.Off);
            mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);

            State.LineMats.Add(width, mat);
        }

        // ===================== 对外 API =====================

        /// <summary>
        /// 设置当前绘制使用的“图层”（只是一个整数标记，方便批量清除）
        /// </summary>
        public static void SetLayer(int layer)
        {
            State.CurrentLayer = layer;
        }

        public static void SetEnabled(bool enabled)
        {
            State.Enabled = enabled;
        }

        public static void ClearAll()
        {
            State.Commands.Clear();
        }

        public static void ClearLayer(int layer)
        {
            State.Commands.RemoveAll(c => c.layer == layer);
        }

        // ---- 线段 ----

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f, bool depthTest = true)
        {
            var config = new DrawConfig
            {
                color = color,
                duration = duration,
                layer = State.CurrentLayer,
                lineWidth = 2f,
                depthTest = depthTest
            };
            DrawLine(start, end, config);
        }

        public static void DrawLine(Vector3 start, Vector3 end, DrawConfig config)
        {
            if (!State.Enabled) return;

            var cmd = new DrawCommand
            {
                type = DrawType.Line,
                positions = new[] { start, end },
                color = config.color,
                remainingTime = config.duration,
                layer = config.layer,
                lineWidth = config.lineWidth,
                depthTest = config.depthTest
            };
            State.Commands.Add(cmd);
        }

        // ---- 线框立方体 ----

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color, float duration = 0f,
            bool depthTest = true)
        {
            if (!State.Enabled) return;

            var cmd = new DrawCommand
            {
                type = DrawType.WireCube,
                positions = new[] { center, size },
                color = color,
                remainingTime = duration,
                layer = State.CurrentLayer,
                lineWidth = 2f,
                depthTest = depthTest
            };
            State.Commands.Add(cmd);
        }

        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size, Color color,   
            float duration = 0f, bool depthTest = true)  
        {  
            if (!State.Enabled) return;  
  
            Vector3 hx = rotation * (Vector3.right   * size.x * 0.5f);  
            Vector3 hy = rotation * (Vector3.up      * size.y * 0.5f);  
            Vector3 hz = rotation * (Vector3.forward * size.z * 0.5f);  
  
            Vector3 p000 = center - hx - hy - hz;  
            Vector3 p001 = center - hx - hy + hz;  
            Vector3 p010 = center - hx + hy - hz;  
            Vector3 p011 = center - hx + hy + hz;  
            Vector3 p100 = center + hx - hy - hz;  
            Vector3 p101 = center + hx - hy + hz;  
            Vector3 p110 = center + hx + hy - hz;  
            Vector3 p111 = center + hx + hy + hz;  
  
            var cfg = new DrawConfig { color = color, duration = duration,   
                layer = State.CurrentLayer, lineWidth = 2f, depthTest = depthTest };  
  
            void Line(Vector3 a, Vector3 b) => DrawLine(a, b, cfg);  
  
            // 底面  
            Line(p000, p100); Line(p100, p101); Line(p101, p001); Line(p001, p000);  
            // 顶面  
            Line(p010, p110); Line(p110, p111); Line(p111, p011); Line(p011, p010);  
            // 立柱  
            Line(p000, p010); Line(p100, p110); Line(p101, p111); Line(p001, p011);  
        }
        
        // ---- 简易“球体”：用 3 个正交圆环线框近似 ----

        public static void DrawWireSphere3Rings(Vector3 center, float radius, Color color, float duration = 0f,
            bool depthTest = true)
        {
            if (!State.Enabled) return;

            var cmd = new DrawCommand
            {
                type = DrawType.WireSphere3Rings,
                positions = new[] { center, new Vector3(radius, 0, 0) },
                color = color,
                remainingTime = duration,
                layer = State.CurrentLayer,
                lineWidth = 2f,
                depthTest = depthTest
            };
            State.Commands.Add(cmd);
        }

        // ---- 网格 ----

        public static void DrawGrid(Vector3 center, Vector2 size, Vector2 spacing, Color color, float duration = 0f,
            bool depthTest = false)
        {
            if (!State.Enabled) return;

            var cmd = new DrawCommand
            {
                type = DrawType.Grid,
                positions = new[] { center },
                gridSize = size,
                gridSpacing = spacing,
                color = color,
                remainingTime = duration,
                layer = State.CurrentLayer,
                lineWidth = 1f,
                depthTest = depthTest
            };
            State.Commands.Add(cmd);
        }

        // ---- 箭头 ----

        public static void DrawArrow(Vector3 start, Vector3 end, Color color, float duration = 0f)
        {
            if (!State.Enabled) return;

            // 主干线
            DrawLine(start, end, color, duration);

            // 箭头小翅膀
            Vector3 dir = (end - start).normalized;
            if (dir.sqrMagnitude < 1e-6f) return;

            Vector3 right = Vector3.Cross(dir, Vector3.up);
            if (right.sqrMagnitude < 1e-6f) right = Vector3.Cross(dir, Vector3.forward);
            right.Normalize();
            Vector3 up = Vector3.Cross(right, dir);

            float headLength = 0.3f;
            float headWidth = 0.15f;

            Vector3 tip = end;
            Vector3 basePos = end - dir * headLength;

            Vector3 wing1 = basePos + right * headWidth;
            Vector3 wing2 = basePos - right * headWidth;
            Vector3 wing3 = basePos + up * headWidth;
            Vector3 wing4 = basePos - up * headWidth;

            DrawLine(tip, wing1, color, duration);
            DrawLine(tip, wing2, color, duration);
            DrawLine(tip, wing3, color, duration);
            DrawLine(tip, wing4, color, duration);
        }

        // ---- 标签 ----

        public static void DrawLabel(Vector3 position, string text, Color color, float duration = 0f)
        {
            if (!State.Enabled) return;

            var cmd = new DrawCommand
            {
                type = DrawType.Label,
                positions = new[] { position },
                color = color,
                remainingTime = duration,
                layer = State.CurrentLayer,
                lineWidth = 0,
                depthTest = false,
                text = text
            };
            State.Commands.Add(cmd);
        }

        // ===================== SceneView 绘制与更新 =====================

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!State.Enabled) return;

            // 用 GL 画几何用的是世界坐标，所以直接使用 identity
            Handles.BeginGUI(); // 确保 Label 等 GUI 绘制不会打断 GL
            Handles.EndGUI();

            // 绘制线段 / 图形
            foreach (var cmd in State.Commands)
            {
                switch (cmd.type)
                {
                    case DrawType.Line:
                        DrawCommandLine(cmd);
                        break;
                    case DrawType.WireCube:
                        DrawCommandWireCube(cmd);
                        break;
                    case DrawType.WireSphere3Rings:
                        DrawCommandWireSphere3Rings(cmd);
                        break;
                    case DrawType.Grid:
                        DrawCommandGrid(cmd);
                        break;
                    case DrawType.Arrow:
                        // 当前 Arrow 内部是拆成多条 Line 加入列表，不到这里
                        break;
                    case DrawType.Label:
                        DrawCommandLabel(cmd);
                        break;
                }
            }
        }

        private static void OnEditorUpdate()
        {
            if (!State.Enabled) return;

            float dt = (float)EditorApplication.timeSinceStartup;
            // 实际上 EditorApplication.deltaTime 旧版本没有，这里用帧到帧的思路也可以，
            // 简化：每帧减去固定一点时间或者用 Time.realtimeSinceStartup 记录上一帧。
            // 为了简洁，这里用 EditorApplication.timeSinceStartup 做一次“粗略”差值。

            // 简单版本：假设 60FPS，每帧约 1/60
            float approxDelta = 1f / 60f;

            for (int i = State.Commands.Count - 1; i >= 0; i--)
            {
                var cmd = State.Commands[i];
                if (cmd.remainingTime > 0f)
                {
                    cmd.remainingTime -= approxDelta;
                    if (cmd.remainingTime <= 0f)
                    {
                        State.Commands.RemoveAt(i);
                        continue;
                    }

                    State.Commands[i] = cmd;
                }
            }
        }

        // ===================== 各种具体 DrawCommand 实现 =====================

        private static void ApplyLineMaterial(float width, bool depthTest)
        {
            if (!State.LineMats.TryGetValue(width, out var mat))
                mat = State.LineMats[2f];

            mat.SetInt("_ZTest", (int)(depthTest ? CompareFunction.LessEqual : CompareFunction.Always));
            mat.SetPass(0);
        }

        private static void DrawCommandLine(DrawCommand cmd)
        {
            if (cmd.positions == null || cmd.positions.Length < 2) return;

            ApplyLineMaterial(cmd.lineWidth, cmd.depthTest);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);

            GL.Begin(GL.LINES);
            GL.Color(cmd.color);
            GL.Vertex(cmd.positions[0]);
            GL.Vertex(cmd.positions[1]);
            GL.End();

            GL.PopMatrix();
        }

        private static void DrawCommandWireCube(DrawCommand cmd)
        {
            Vector3 center = cmd.positions[0];
            Vector3 size = cmd.positions[1];

            Vector3 hx = Vector3.right * size.x * 0.5f;
            Vector3 hy = Vector3.up * size.y * 0.5f;
            Vector3 hz = Vector3.forward * size.z * 0.5f;

            Vector3 p000 = center - hx - hy - hz;
            Vector3 p001 = center - hx - hy + hz;
            Vector3 p010 = center - hx + hy - hz;
            Vector3 p011 = center - hx + hy + hz;
            Vector3 p100 = center + hx - hy - hz;
            Vector3 p101 = center + hx - hy + hz;
            Vector3 p110 = center + hx + hy - hz;
            Vector3 p111 = center + hx + hy + hz;

            // 用现有的 DrawLine 再加命令，会多一层列表；这里直接 GL 画，避免命令爆炸
            ApplyLineMaterial(cmd.lineWidth, cmd.depthTest);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            GL.Color(cmd.color);

            void Edge(Vector3 a, Vector3 b)
            {
                GL.Vertex(a);
                GL.Vertex(b);
            }

            // 底面
            Edge(p000, p100);
            Edge(p100, p101);
            Edge(p101, p001);
            Edge(p001, p000);

            // 顶面
            Edge(p010, p110);
            Edge(p110, p111);
            Edge(p111, p011);
            Edge(p011, p010);

            // 立柱
            Edge(p000, p010);
            Edge(p100, p110);
            Edge(p101, p111);
            Edge(p001, p011);

            GL.End();
            GL.PopMatrix();
        }

        private static void DrawCommandWireSphere3Rings(DrawCommand cmd)
        {
            Vector3 center = cmd.positions[0];
            float radius = cmd.positions[1].x;
            int segments = 24;

            ApplyLineMaterial(cmd.lineWidth, cmd.depthTest);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            GL.Color(cmd.color);

            // 三个正交圆环
            for (int ring = 0; ring < 3; ring++)
            {
                Vector3 axis1, axis2;
                if (ring == 0)
                {
                    axis1 = Vector3.right;
                    axis2 = Vector3.forward;
                } // 绕 Y
                else if (ring == 1)
                {
                    axis1 = Vector3.up;
                    axis2 = Vector3.forward;
                } // 绕 X
                else
                {
                    axis1 = Vector3.right;
                    axis2 = Vector3.up;
                } // 绕 Z

                for (int i = 0; i < segments; i++)
                {
                    float a0 = (float)i / segments * Mathf.PI * 2f;
                    float a1 = (float)(i + 1) / segments * Mathf.PI * 2f;
                    Vector3 p0 = center + (axis1 * Mathf.Cos(a0) + axis2 * Mathf.Sin(a0)) * radius;
                    Vector3 p1 = center + (axis1 * Mathf.Cos(a1) + axis2 * Mathf.Sin(a1)) * radius;
                    GL.Vertex(p0);
                    GL.Vertex(p1);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private static void DrawCommandGrid(DrawCommand cmd)
        {
            Vector3 center = cmd.positions[0];
            Vector2 size = cmd.gridSize;
            Vector2 spacing = cmd.gridSpacing;

            ApplyLineMaterial(cmd.lineWidth, cmd.depthTest);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            GL.Color(cmd.color);

            // X 方向线
            for (float x = -size.x / 2f; x <= size.x / 2f + 0.001f; x += spacing.x)
            {
                Vector3 a = center + new Vector3(x, 0f, -size.y / 2f);
                Vector3 b = center + new Vector3(x, 0f, size.y / 2f);
                GL.Vertex(a);
                GL.Vertex(b);
            }

            // Z 方向线
            for (float z = -size.y / 2f; z <= size.y / 2f + 0.001f; z += spacing.y)
            {
                Vector3 a = center + new Vector3(-size.x / 2f, 0f, z);
                Vector3 b = center + new Vector3(size.x / 2f, 0f, z);
                GL.Vertex(a);
                GL.Vertex(b);
            }

            GL.End();
            GL.PopMatrix();
        }

        private static void DrawCommandLabel(DrawCommand cmd)
        {
            if (string.IsNullOrEmpty(cmd.text)) return;
            Handles.color = cmd.color;
            Handles.Label(cmd.positions[0], cmd.text);
        }

        // ===================== 简单示例菜单 =====================

        [MenuItem("EXTool/DebugDraw/Example (SceneView 全局调试)")]
        private static void ExampleMenu()
        {
            ClearAll();
            SetLayer(0);

            // 世界坐标轴
            DrawLine(Vector3.zero, Vector3.right * 5, Color.red, 5f);
            DrawLine(Vector3.zero, Vector3.up * 5, Color.green, 5f);
            DrawLine(Vector3.zero, Vector3.forward * 5, Color.blue, 5f);

            // 线框方块
            DrawWireCube(new Vector3(0, 1, 0), new Vector3(2, 2, 2), Color.yellow, 5f);

            // 简易球
            DrawWireSphere3Rings(new Vector3(4, 1, 0), 1.5f, Color.cyan, 5f);

            // 网格
            DrawGrid(Vector3.zero, new Vector2(10, 10), new Vector2(1, 1), new Color(0.7f, 0.7f, 0.7f, 0.6f), 5f);

            // 箭头
            DrawArrow(new Vector3(-3, 0, -3), new Vector3(-1, 2, -1), Color.magenta, 5f);

            // 标签
            DrawLabel(new Vector3(0, 2.5f, 0), "Debug Center", Color.white, 5f);

            Debug.Log("DebugDrawTool Example 执行完毕：请切到 Scene 视图查看绘制效果。");
        }
    }
}
#endif