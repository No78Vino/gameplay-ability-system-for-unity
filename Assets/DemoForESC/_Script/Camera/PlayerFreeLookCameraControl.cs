using System;
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

        // 用来备份原本的轴名称 (例如 "Mouse X", "Mouse Y")
        private string originalXAxisName;
        private string originalYAxisName;

        private void Awake()
        {
            if (_instance == null) _instance = this;
        }

        private void Start()
        {
            if (freeLookCamera == null)
                freeLookCamera = GetComponent<CinemachineFreeLook>();

            // 1. 在游戏开始时，记录下你在 Inspector 里填写的轴名称
            // 如果这里是空的，说明你可能没在面板里设置 Mouse X / Mouse Y
            originalXAxisName = freeLookCamera.m_XAxis.m_InputAxisName;
            originalYAxisName = freeLookCamera.m_YAxis.m_InputAxisName;
        }

        /// <summary>
        /// 控制输入是否生效
        /// </summary>
        /// <param name="active">true=允许鼠标控制, false=禁止鼠标控制</param>
        public void SetInputActive(bool active)
        {
            if (freeLookCamera == null) return;

            if (active)
            {
                // 恢复：把名字设回去，让它能读到 "Mouse X"
                freeLookCamera.m_XAxis.m_InputAxisName = originalXAxisName;
                freeLookCamera.m_YAxis.m_InputAxisName = originalYAxisName;
            }
            else
            {
                // 禁用：把名字清空，Cinemachine 读不到任何输入
                freeLookCamera.m_XAxis.m_InputAxisName = "";
                freeLookCamera.m_YAxis.m_InputAxisName = "";

                // 重要：强制把当前的输入值归零，防止禁用瞬间的惯性漂移
                freeLookCamera.m_XAxis.m_InputAxisValue = 0;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0;
            }

            freeLookCamera.enabled = false;
            freeLookCamera.enabled = true;
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
            // freeLookCamera.m_YAxis.Value = 0.5f; 
        }
    }
}