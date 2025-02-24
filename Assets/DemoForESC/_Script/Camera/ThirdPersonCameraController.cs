using Cinemachine;
using UnityEngine;

namespace DemoForESC._Script.Camera
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
    
        [Header("Camera")]
        public CinemachineFreeLook freeLookCamera;
        public float cameraSensitivity = 2f;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _verticalVelocity;
        private bool _isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 配置相机输入
            if (freeLookCamera)
            {
                freeLookCamera.m_XAxis.m_MaxSpeed = 0; // 禁用Cinemachine自带的X轴控制
                freeLookCamera.m_YAxis.m_MaxSpeed = 0; // 禁用Y轴控制
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleCameraRotation();
        }

        private void HandleMovement()
        {
            // 获取输入
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 获取相机方向
            Vector3 cameraForward = freeLookCamera.transform.forward;
            Vector3 cameraRight = freeLookCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 计算移动方向
            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

            // 应用移动
            _controller.Move(movement);

            // 角色朝向移动方向
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void HandleCameraRotation()
        {
            if (!freeLookCamera) return;

            // 获取鼠标输入
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            // 控制相机旋转
            freeLookCamera.m_XAxis.Value += mouseX;
            freeLookCamera.m_YAxis.Value -= mouseY; // 注意Y轴需要反转
        }
    }
}