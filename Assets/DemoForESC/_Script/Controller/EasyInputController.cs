using UnityEngine;

namespace DemoForESC._Script.Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class EasyInputController : MonoBehaviour
    {
        [Header("移动参数")] [SerializeField] [Range(0f, 5f)]
        private float walkSpeed = 2.5f; // 行走速度

        [SerializeField] [Range(0f, 10f)] private float runSpeed = 5f; // 奔跑速度

        [SerializeField] [Range(0.1f, 3f)] private float acceleration = 0.5f; // 加速时间

        [SerializeField] [Range(0f, 0.3f)] private float rotationOffset = 0.1f; // 转身缓冲



        private Vector3 _cameraForward;

        private CharacterController _controller;
        private float _currentSpeed;
        private bool _isRunning;
        private UnityEngine.Camera _mainCamera;
        private Vector3 _movement;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            HandleInput();
            UpdateMovement();
        }

        private void HandleInput()
        {
            // 获取原始输入（保持原始值用于动画混合）
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // 转换为基于相机的移动方向
            _cameraForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            var cameraRight = Vector3.Cross(Vector3.up, _cameraForward).normalized;

            // 构建移动向量
            _movement = (_cameraForward * vertical + cameraRight * horizontal).normalized;

            // 奔跑控制
            _isRunning = Input.GetKey(KeyCode.LeftShift);
        }

        private void UpdateMovement()
        {
            // 速度控制
            var targetSpeed = _isRunning ? runSpeed : walkSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, acceleration * Time.deltaTime);

            // 应用移动
            if (_movement.magnitude > 0.1f)
            {
                // 保持角色面向相机方向（带缓冲）
                var targetRotation = Quaternion.LookRotation(_cameraForward);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationOffset * Time.deltaTime * 100
                );

                // 移动执行
                var motion = _movement * _currentSpeed * Time.deltaTime;
                _controller.Move(motion);
            }
            Debug.Log("UpdateMovement:"+_movement);
        }

        // 供动画系统调用的参数
        public Vector3 GetMovementVector()
        {
            return _movement;
        }

        public bool IsMoving()
        {
            return _movement.magnitude > 0.1f;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}