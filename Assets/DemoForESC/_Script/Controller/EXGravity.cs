using UnityEngine;

namespace DemoForESC._Script
{
    public class EXGravity : MonoBehaviour
    {
        [Header("基础设置")] [Tooltip("启用/禁用重力")] public bool gravityEnabled = true;

        [Tooltip("重力缩放系数（1=标准重力）")] public float gravityScale = 1.0f;

        [Header("地面检测")] [Tooltip("地面层级")] public LayerMask groundLayer = 1;

        [Tooltip("检测距离（单位：米）")] public float groundCheckDistance = 0.2f;

        [Tooltip("有效地面角度（0-90度）")] [Range(0, 90)]
        public float maxSlopeAngle = 45f;

        private float _groundAngleThreshold;
        private bool _isGrounded;

        private void Awake()
        {
            _groundAngleThreshold = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        }

        private void Update()
        {
            CheckGroundStatus();
        }

        private void FixedUpdate()
        {
            if (ShouldApplyGravity()) ApplyCustomGravity();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position,
                transform.position + Vector3.down * groundCheckDistance);
        }

        private bool ShouldApplyGravity()
        {
            return gravityEnabled && !_isGrounded ; //&& _rb.velocity.y <= 0;
        }

        private void ApplyCustomGravity()
        {
            var gravity = gravityScale * Physics.gravity;
            var motion = gravity * Time.fixedDeltaTime;
            transform.position += motion;
        }

        private void CheckGroundStatus()
        {
            RaycastHit hit;
            _isGrounded = Physics.Raycast(transform.position,
                Vector3.down,
                out hit,
                groundCheckDistance,
                groundLayer);

            if (_isGrounded)
            {
                var slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                _isGrounded = slopeAngle <= maxSlopeAngle;
            }
        }

        // 公共接口
        public void SetGravityScale(float scale)
        {
            gravityScale = scale;
        }

        public void ToggleGravity(bool state)
        {
            gravityEnabled = state;
        }

        public bool IsGrounded()
        {
            return _isGrounded;
        }
    }
}