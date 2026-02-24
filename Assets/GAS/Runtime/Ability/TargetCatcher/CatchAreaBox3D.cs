using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public class CatchAreaBox3D : TargetCatcherBase<XParamCatchAreaBox3D>
    {
        // 最大捕获数量为64，超出则无视64之后的目标
        private static readonly Collider[] Colliders = new Collider[64];

        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget,
            List<AbilitySystemComponent> results)
        {
            // 根据XParamCatchAreaBox3D参数，捕获范围内的目标
            int count;
            if (Parameter.isWorldSpace)
            {
                count = Physics.OverlapBoxNonAlloc(
                    Parameter.offset,
                    Parameter.size*0.5f,
                    Colliders,
                    Quaternion.Euler(Parameter.rotation),
                    Parameter.layer.value);
            }
            else
            {
                count = Physics.OverlapBoxNonAlloc(
                    mainTarget.transform.TransformPoint(Parameter.offset),
                    Parameter.size*0.5f,
                    Colliders,
                    Quaternion.Euler(mainTarget.transform.TransformDirection(Parameter.rotation)),
                    Parameter.layer.value);
            }

            for (var i = 0; i < count; ++i)
            {
                var targetUnit = Colliders[i].GetComponent<AbilitySystemComponent>();
                if (targetUnit != null)
                {
                    results.Add(targetUnit);
                }
            }
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
                        size = new Vector3(x, y, z);
                    }
                }
            }
            
            // rotation
            if (paramData.Count > 4)
            {
                var strData = paramData[4] as string;
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
            if (paramData.Count > 5)
            {
                var strData = paramData[5] as string;
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
                layer.ToString()
            };
            return data;
        }
#endif
    }
}