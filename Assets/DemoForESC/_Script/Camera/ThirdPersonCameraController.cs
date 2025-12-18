using Cinemachine;
using UnityEngine;

namespace DemoForESC._Script
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("核心配置")]
        [Tooltip("锁定跟踪的目标物体")]
        public Transform trackingTarget;
        [Range(1f, 15f), Tooltip("相机跟随距离")]
        public float followDistance = 5f;

        [Header("灵敏度控制")]
        [Range(0.1f, 5f), Tooltip("水平旋转灵敏度")]
        public float xSensitivity = 1.5f;
        [Range(0.1f, 5f), Tooltip("垂直旋转灵敏度")]
        public float ySensitivity = 0.8f;

        [Header("阻尼系统")]
        [Range(0f, 1f), Tooltip("水平阻尼时间")]
        public float xDamping = 0.2f;
        [Range(0f, 1f), Tooltip("垂直阻尼时间")]
        public float yDamping = 0.3f;
        [Range(0f, 2f), Tooltip("自动回中时间")]
        public float autoCenterDelay = 1f;

        [Header("轨道配置")]
        [SerializeField, Range(0.5f, 5f)] 
        private float _topRigHeight = 4f;
        [SerializeField, Range(0.5f, 5f)] 
        private float _middleRigHeight = 2.5f;
        [SerializeField, Range(0.5f, 5f)] 
        private float _bottomRigHeight = 1f;
        [SerializeField] 
        private Vector3 _followOffset = new Vector3(0, 1.8f, 0);

        [Header("碰撞检测")]
        [Range(0.1f, 3f), Tooltip("避障检测距离")]
        public float collisionDistance = 1f;
        [Range(0f, 1f), Tooltip("避障阻尼")]
        public float collisionDamping = 0.2f;

        private CinemachineFreeLook _freeLookCam;
        private CinemachineCollider _cameraCollider;
        private float _xVelocity;
        private float _yVelocity;

        private void Awake()
        {
            InitializeComponents();
            ConfigureCamera();
            SetupCollisionSystem();
        }

        private void InitializeComponents()
        {
            _freeLookCam = GetComponent<CinemachineFreeLook>();
            _cameraCollider = GetComponent<CinemachineCollider>() ?? gameObject.AddComponent<CinemachineCollider>();
        }

        private void ConfigureCamera()
        {
            if (trackingTarget)
            {
                _freeLookCam.Follow = trackingTarget;
                _freeLookCam.LookAt = trackingTarget;
            }

            // 初始化轨道参数
            UpdateOrbitParameters();
            ApplyCommonSettings();
        }

        private void ApplyCommonSettings()
        {
            // 基础配置
            _freeLookCam.m_Lens.FieldOfView = 40;
            _freeLookCam.m_CommonLens = true;

            // 禁用Cinemachine自带输入
            _freeLookCam.m_XAxis.m_MaxSpeed = 0;
            _freeLookCam.m_YAxis.m_MaxSpeed = 0;

            // 自动居中配置
            var recenter = _freeLookCam.m_RecenterToTargetHeading;
            recenter.m_enabled = true;
            recenter.m_WaitTime = autoCenterDelay;
            recenter.m_RecenteringTime = autoCenterDelay * 0.5f;
        }

        private void SetupCollisionSystem()
        {
            _cameraCollider.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;
            _cameraCollider.m_DistanceLimit = collisionDistance;
            _cameraCollider.m_Damping = collisionDamping;
        }

        private void Update()
        {
            if (!trackingTarget) return;

            HandleCameraRotation();
            UpdateDynamicParameters();
        }

        private void HandleCameraRotation()
        {
            if (GuideManager.I.IsInGuide && GuideManager.I.GuideInfo.limitFovRotation)
            {
                // 直接修改 Value 是不受 Input 影响的底层操作
                // 设置 X 轴的值等于玩家的 Y 轴旋转，即可对齐朝向
                _freeLookCam.m_XAxis.Value = 0f; //GameManager.I.Player.transform.eulerAngles.y;

                // 可选：重置高度到中间
                _freeLookCam.m_YAxis.Value = 0f; 
                return;
            }
            
            float targetX = Input.GetAxis("MouseX") * xSensitivity;
            float targetY = Input.GetAxis("MouseY") * ySensitivity;
            
            _freeLookCam.m_XAxis.Value += Mathf.SmoothDamp(0, targetX, ref _xVelocity, xDamping);
            _freeLookCam.m_YAxis.Value -= Mathf.SmoothDamp(0, targetY, ref _yVelocity, yDamping);
        }

        private void UpdateDynamicParameters()
        {
            // 更新轨道参数
            UpdateOrbitParameters();

            // 更新跟踪偏移
            var composer = _freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = _followOffset;

            // 更新碰撞参数
            _cameraCollider.m_DistanceLimit = collisionDistance;
            _cameraCollider.m_Damping = collisionDamping;
        }

        private void UpdateOrbitParameters()
        {
            SetOrbit(0, _topRigHeight);    // 上轨道
            SetOrbit(1, _middleRigHeight);  // 中轨道
            SetOrbit(2, _bottomRigHeight);  // 下轨道
        }

        private void SetOrbit(int rigIndex, float height)
        {
            var orbit = _freeLookCam.m_Orbits[rigIndex];
            orbit.m_Height = height;
            orbit.m_Radius = followDistance;
            _freeLookCam.m_Orbits[rigIndex] = orbit;
        }

        private void OnValidate()
        {
            if (!_freeLookCam)
            {
                _freeLookCam = GetComponent<CinemachineFreeLook>();
                _cameraCollider = GetComponent<CinemachineCollider>() ?? gameObject.AddComponent<CinemachineCollider>();
            }
            if (_freeLookCam) UpdateDynamicParameters();
        }

        public void SetTrackingTarget(Transform newTarget)
        {
            trackingTarget = newTarget;
            _freeLookCam.Follow = trackingTarget;
            _freeLookCam.LookAt = trackingTarget;
        }

        public void ResetCameraRotation()
        {
            _freeLookCam.m_XAxis.Value = 0;
            _freeLookCam.m_YAxis.Value = 0.5f; // 默认中间视角
        }
    }
}