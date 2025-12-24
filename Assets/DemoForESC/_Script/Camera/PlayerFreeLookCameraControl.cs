using Cinemachine;
using UnityEngine;

namespace DemoForESC._Script
{
    public class PlayerFreeLookCameraControl : MonoBehaviour
    {
        private static PlayerFreeLookCameraControl _instance;
        public static PlayerFreeLookCameraControl Instance => _instance;
        
        [Header("设置")] public CinemachineFreeLook freeLookCamera;
        public Transform playerTransform;
        
        private void Awake()
        {
            if (_instance == null) _instance = this;
        }

        private void Start()
        {
            if (freeLookCamera == null)
                freeLookCamera = GetComponent<CinemachineFreeLook>();
        }

        /// <summary>
        /// 代码控制视角：让相机看向玩家背后
        /// </summary>
        public void AlignViewToPlayer()
        {
            if (freeLookCamera == null || playerTransform == null) return;

            // 直接修改 Value 是不受 Input 影响的底层操作
            // 设置 X 轴的值等于玩家的 Y 轴旋转，即可对齐朝向
            freeLookCamera.m_XAxis.Value = playerTransform.eulerAngles.y;

            // 可选：重置高度到中间
            freeLookCamera.m_YAxis.Value = 0.5f; 
        }
    }
}