using System;
using UnityEngine;

namespace Cinemachine.Examples
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    [SaveDuringPlay] // Enable SaveDuringPlay for this class
    public class MouseScrollZoom2D : MonoBehaviour
    {
        [Range(0, 10)]
        public float ZoomMultiplier = 1f;
        [Range(0, 100)]
        public float MinZoom = 1f;
        [Range(0, 100)]
        public float MaxZoom = 50f;

        CinemachineVirtualCamera m_VirtualCamera;
        float m_OriginalOrthoSize;

        void Awake()
        {
            m_VirtualCamera = GetComponent<CinemachineVirtualCamera>();
            m_OriginalOrthoSize = m_VirtualCamera.m_Lens.OrthographicSize;

#if UNITY_EDITOR
            // This code shows how to play nicely with the VirtualCamera's SaveDuringPlay functionality
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrthographicSize;
            SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrthographicSize;
#endif
        }

#if UNITY_EDITOR
        void OnDestroy()
        {
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrthographicSize;
        }
        
        void RestoreOriginalOrthographicSize()
        {
            m_VirtualCamera.m_Lens.OrthographicSize = m_OriginalOrthoSize;
        }
#endif

        void OnValidate()
        {
            MaxZoom = Mathf.Max(MinZoom, MaxZoom);
        }

        void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            float zoom = m_VirtualCamera.m_Lens.OrthographicSize + Input.mouseScrollDelta.y * ZoomMultiplier;
            m_VirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(zoom, MinZoom, MaxZoom);
#else
            InputSystemHelper.EnableBackendsWarningMessage();
#endif
        }
    }
}
