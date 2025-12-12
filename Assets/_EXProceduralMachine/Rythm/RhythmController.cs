using UnityEngine;

namespace EXProceduralMachine
{
    // 使用示例组件
    public class RhythmController : MonoBehaviour
    {
        [Header("呼吸系统")] public RhythmSystem rhythmSystem;

        [Header("控制对象")] public Transform controlledTransform;
        public Vector3 scaleRange = new(0.1f, 0.1f, 0.1f);
        public Vector3 positionRange = Vector3.zero;
        public Vector3 rotationRange = Vector3.zero;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private Vector3 originalScale;

        private void Start()
        {
            if (rhythmSystem == null)
                rhythmSystem = GetComponent<RhythmSystem>();

            if (controlledTransform != null)
            {
                originalScale = controlledTransform.localScale;
                originalPosition = controlledTransform.localPosition;
                originalRotation = controlledTransform.localRotation;
            }
        }

        private void Update()
        {
            if (rhythmSystem == null)
                return;

            var breathValue = rhythmSystem.GetValue();

            if (controlledTransform != null)
            {
                // 控制缩放
                controlledTransform.localScale = originalScale +
                                                 Vector3.Scale(scaleRange, Vector3.one * breathValue);

                // 控制位置
                controlledTransform.localPosition = originalPosition +
                                                    Vector3.Scale(positionRange, Vector3.one * breathValue);

                // 控制旋转
                if (rotationRange != Vector3.zero)
                    controlledTransform.localRotation = originalRotation *
                                                        Quaternion.Euler(rotationRange * breathValue);
            }
        }
    }
}