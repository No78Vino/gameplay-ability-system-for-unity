using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     节奏系统使用示例：用呼吸值驱动目标的缩放 / 位置 / 旋转。
    /// </summary>
    public class RhythmController : MonoBehaviour
    {
        [Header("呼吸系统")]
        public RhythmSystem rhythmSystem;

        [Header("控制对象")]
        public Transform controlledTransform;

        public Vector3 scaleRange = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3 positionRange = Vector3.zero;
        public Vector3 rotationRange = Vector3.zero;

        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

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
            if (rhythmSystem == null || controlledTransform == null)
                return;

            var breathValue = rhythmSystem.GetValue();

            controlledTransform.localScale = originalScale + Vector3.Scale(scaleRange, Vector3.one * breathValue);
            controlledTransform.localPosition = originalPosition + Vector3.Scale(positionRange, Vector3.one * breathValue);

            if (rotationRange != Vector3.zero)
                controlledTransform.localRotation = originalRotation * Quaternion.Euler(rotationRange * breathValue);
        }
    }
}