using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public sealed class CatchAreaBox3D : TargetCatcherBase<XParamCatchAreaBox3D>  
    {  
        private static readonly Collider[] Colliders = new Collider[64];  
  
        protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)  
        {  
            int count;  
            if (Parameter.isWorldSpace)  
            {  
                count = Physics.OverlapBoxNonAlloc(  
                    Parameter.offset,  
                    Parameter.size * 0.5f,  
                    Colliders,  
                    Quaternion.Euler(Parameter.rotation),  
                    Parameter.layer.value);  
            }  
            else  
            {  
                var mainTransform = mainTarget.GameObject.transform;  
                count = Physics.OverlapBoxNonAlloc(  
                    mainTransform.TransformPoint(Parameter.offset),  
                    Parameter.size * 0.5f,  
                    Colliders,  
                    Quaternion.Euler(mainTransform.TransformDirection(Parameter.rotation)),  
                    Parameter.layer.value);  
            }  
  
            for (var i = 0; i < count; ++i)  
            {  
                // 通过 MonoBehaviour 上的 AbilitySystemComponent 拿到 Cell  
                var mono = Colliders[i].GetComponent<AbilitySystemComponent>();  
                if (mono != null)  results.Add(mono.Cell);  
            }  
        }

        public override void OnEditorPreview(GameObject obj)
        {
            if (Parameter == null) return;  
  
            Vector3 center;  
            Quaternion rotation;  
  
            if (Parameter.isWorldSpace)  
            {  
                // 世界空间：直接使用参数中的 offset 和 rotation  
                center = Parameter.offset;  
                rotation = Quaternion.Euler(Parameter.rotation);  
            }  
            else  
            {  
                // 本地空间：相对于 previewObject 的 Transform 做变换  
                // 与运行时 CatchTargetsNonAlloc 的逻辑完全对应  
                if (obj == null) return;  
                var t = obj.transform;  
                center = t.TransformPoint(Parameter.offset);  
                rotation = Quaternion.Euler(t.TransformDirection(Parameter.rotation));  
            }  
  
            // 使用 Gizmos 绘制线框盒子（在 Scene 视图中可见）  
            var oldMatrix = UnityEditor.Handles.matrix;  
            UnityEditor.Handles.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);  
            UnityEditor.Handles.color = Color.green;  
            UnityEditor.Handles.DrawWireCube(Vector3.zero, Parameter.size);  
            UnityEditor.Handles.matrix = oldMatrix;  
  
            // 也可以用 Debug.DrawLine 系列，但 Handles.DrawWireCube 在 Scene 视图中更直观  
            UnityEditor.SceneView.RepaintAll();  
        }
    }

    public class XParamCatchAreaBox3D : XParam
    {
        [LabelText("是否是世界空间坐标系")]
        public bool isWorldSpace;
        
        [LabelText("偏移")]
        public Vector3 offset;
        
        [LabelText("大小")]
        public Vector3 size;
        
        [LabelText("旋转")]
        public Vector3 rotation;
        
        [LabelText("监测层级")]
        public LayerMask layer;
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            // isWorldSpace
            if (paramData.Count > 0)
            {
                var strData = paramData[0] as string;
                if (string.IsNullOrEmpty(strData)) return;
                
                if (!bool.TryParse(strData, out isWorldSpace))
                    isWorldSpace = false;
            }
            
            // offset 
            if (paramData.Count > 1)
            {
                var strData = paramData[1] as string;
                if (string.IsNullOrEmpty(strData)) return;
                
                var data = strData.Split(',');
                if (data.Length == 3)
                {
                    if (float.TryParse(data[0], out var x) &&
                        float.TryParse(data[1], out var y) &&
                        float.TryParse(data[2], out var z))
                    {
                        offset = new Vector3(x, y, z);
                    }
                }
            }
            
            // size
            if (paramData.Count > 2)
            {
                var strData = paramData[2] as string;
                if (string.IsNullOrEmpty(strData)) return;
                
                var data = strData.Split(',');
                if (data.Length == 3)
                {
                    if (float.TryParse(data[0], out var x) &&
                        float.TryParse(data[1], out var y) &&
                        float.TryParse(data[2], out var z))
                    {
                        size = new Vector3(x, y, z);
                    }
                }
            }
            
            // rotation
            if (paramData.Count > 3)
            {
                var strData = paramData[3] as string;
                if (string.IsNullOrEmpty(strData)) return;

                var data = strData.Split(',');
                if (data.Length == 3)
                {
                    if (float.TryParse(data[0], out var x) &&
                        float.TryParse(data[1], out var y) &&
                        float.TryParse(data[2], out var z))
                    {
                        rotation = new Vector3(x, y, z);
                    }
                }
            }

            // layer
            if (paramData.Count > 4)
            {
                var strData = paramData[4] as string;
                if (string.IsNullOrEmpty(strData)) return;

                if (int.TryParse(strData, out var layerNumber)) layer = layerNumber;
            }
        }

        public List<object> EncodeExcelData()
        {
            var data = new List<object>
            {
                isWorldSpace.ToString(),
                $"{offset.x},{offset.y},{offset.z}",
                $"{size.x},{size.y},{size.z}",
                $"{rotation.x},{rotation.y},{rotation.z}",
                layer.value.ToString()
            };
            return data;
        }
#endif
    }
}