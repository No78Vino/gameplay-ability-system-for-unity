using UnityEngine;

namespace Demo.Script.Fight
{
    public class EffectBoundingBox:MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2 offset;
        private Color color = new Color(0, 1, 0, 0.5f);

        public Vector2 Size => size;

        public Vector3 Center
        {
            get
            {
                Vector3 tOffset = offset;
                tOffset.x *= transform.lossyScale.x > 0 ? 1 : -1;
                return tOffset  + transform.position;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawCube(Center, size);
        }
    }
}