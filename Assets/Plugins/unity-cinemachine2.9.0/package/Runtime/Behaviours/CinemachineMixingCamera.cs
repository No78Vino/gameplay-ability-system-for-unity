using UnityEngine;
using Cinemachine.Utility;
using System.Collections.Generic;

namespace Cinemachine
{
    /// <summary>
    /// CinemachineMixingCamera is a "manager camera" that takes on the state of
    /// the weighted average of the states of its child virtual cameras.
    ///
    /// A fixed number of slots are made available for cameras, rather than a dynamic array.
    /// We do it this way in order to support weight animation from the Timeline.
    /// Timeline cannot animate array elements.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/CinemachineMixingCamera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineMixingCamera.html")]
    public class CinemachineMixingCamera : CinemachineVirtualCameraBase
    {
        /// <summary>The maximum number of tracked cameras.  If you want to add
        /// more cameras, do it here in the source code, and be sure to add the
        /// extra member variables and to make the appropriate changes in
        /// GetWeight() and SetWeight().
        /// The inspector will figure itself out based on this value.</summary>
        public const int MaxCameras = 8;

        /// <summary>Weight of the first tracked camera</summary>
        [Tooltip("The weight of the first tracked camera")]
        public float m_Weight0 = 0.5f;
        /// <summary>Weight of the second tracked camera</summary>
        [Tooltip("The weight of the second tracked camera")]
        public float m_Weight1 = 0.5f;
        /// <summary>Weight of the third tracked camera</summary>
        [Tooltip("The weight of the third tracked camera")]
        public float m_Weight2 = 0.5f;
        /// <summary>Weight of the fourth tracked camera</summary>
        [Tooltip("The weight of the fourth tracked camera")]
        public float m_Weight3 = 0.5f;
        /// <summary>Weight of the fifth tracked camera</summary>
        [Tooltip("The weight of the fifth tracked camera")]
        public float m_Weight4 = 0.5f;
        /// <summary>Weight of the sixth tracked camera</summary>
        [Tooltip("The weight of the sixth tracked camera")]
        public float m_Weight5 = 0.5f;
        /// <summary>Weight of the seventh tracked camera</summary>
        [Tooltip("The weight of the seventh tracked camera")]
        public float m_Weight6 = 0.5f;
        /// <summary>Weight of the eighth tracked camera</summary>
        [Tooltip("The weight of the eighth tracked camera")]
        public float m_Weight7 = 0.5f;

        /// <summary>Get the weight of the child at an index.</summary>
        /// <param name="index">The child index. Only immediate CinemachineVirtualCameraBase
        /// children are counted.</param>
        /// <returns>The weight of the camera.  Valid only if camera is active and enabled.</returns>
        public float GetWeight(int index)
        {
            switch (index)
            {
                case 0: return m_Weight0;
                case 1: return m_Weight1;
                case 2: return m_Weight2;
                case 3: return m_Weight3;
                case 4: return m_Weight4;
                case 5: return m_Weight5;
                case 6: return m_Weight6;
                case 7: return m_Weight7;
            }
            Debug.LogError("CinemachineMixingCamera: Invalid index: " + index);
            return 0;
        }

        /// <summary>Set the weight of the child at an index.</summary>
        /// <param name="index">The child index. Only immediate CinemachineVirtualCameraBase
        /// children are counted.</param>
        /// <param name="w">The weight to set.  Can be any non-negative number.</param>
        public void SetWeight(int index, float w)
        {
            switch (index)
            {
                case 0: m_Weight0 = w; return;
                case 1: m_Weight1 = w; return;
                case 2: m_Weight2 = w; return;
                case 3: m_Weight3 = w; return;
                case 4: m_Weight4 = w; return;
                case 5: m_Weight5 = w; return;
                case 6: m_Weight6 = w; return;
                case 7: m_Weight7 = w; return;
            }
            Debug.LogError("CinemachineMixingCamera: Invalid index: " + index);
        }

        /// <summary>Get the weight of the child CinemachineVirtualCameraBase.</summary>
        /// <param name="vcam">The child camera.</param>
        /// <returns>The weight of the camera.  Valid only if camera is active and enabled.</returns>
        public float GetWeight(CinemachineVirtualCameraBase vcam)
        {
            ValidateListOfChildren();
            int index;
            if (m_indexMap.TryGetValue(vcam, out index))
                return GetWeight(index);
            Debug.LogError("CinemachineMixingCamera: Invalid child: "
                + ((vcam != null) ? vcam.Name : "(null)"));
            return 0;
        }

        /// <summary>Set the weight of the child CinemachineVirtualCameraBase.</summary>
        /// <param name="vcam">The child camera.</param>
        /// <param name="w">The weight to set.  Can be any non-negative number.</param>
        public void SetWeight(CinemachineVirtualCameraBase vcam, float w)
        {
            ValidateListOfChildren();
            int index;
            if (m_indexMap.TryGetValue(vcam, out index))
                SetWeight(index, w);
            else
                Debug.LogError("CinemachineMixingCamera: Invalid child: "
                    + ((vcam != null) ? vcam.Name : "(null)"));
        }

        /// <summary>Blended camera state</summary>
        private CameraState m_State = CameraState.Default;

        /// <summary>Get the current "best" child virtual camera, which is nominally
        /// the one with the greatest weight.</summary>
        private ICinemachineCamera LiveChild { get; set; }

        /// <summary>The blended CameraState</summary>
        public override CameraState State { get { return m_State; } }

        /// <summary>Not used</summary>
        override public Transform LookAt { get; set; }

        /// <summary>Not used</summary>
        override public Transform Follow { get; set; }

        /// <summary>This is called to notify the vcam that a target got warped,
        /// so that the vcam can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            ValidateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.OnTargetObjectWarped(target, positionDelta);
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            ValidateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.ForceCameraPosition(pos, rot);
            base.ForceCameraPosition(pos, rot);
        }
        
        /// <summary>Makes sure the internal child cache is up to date</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateListOfChildren();
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        public void OnTransformChildrenChanged()
        {
            InvalidateListOfChildren();
        }

        /// <summary>Makes sure the weights are non-negative</summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            for (int i = 0; i < MaxCameras; ++i)
                SetWeight(i, Mathf.Max(0, GetWeight(i)));
        }

        /// <summary>Check whether the vcam a live child of this camera.</summary>
        /// <param name="vcam">The Virtual Camera to check</param>
        /// <param name="dominantChildOnly">If truw, will only return true if this vcam is the dominat live child</param>
        /// <returns>True if the vcam is currently actively influencing the state of this vcam</returns>
        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            CinemachineVirtualCameraBase[] children = ChildCameras;
            for (int i = 0; i < MaxCameras && i < children.Length; ++i)
                if ((ICinemachineCamera)children[i] == vcam)
                    return GetWeight(i) > UnityVectorExtensions.Epsilon && children[i].isActiveAndEnabled;
            return false;
        }

        private CinemachineVirtualCameraBase[] m_ChildCameras;
        private Dictionary<CinemachineVirtualCameraBase, int> m_indexMap;

        /// <summary>Get the cached list of child cameras.
        /// These are just the immediate children in the hierarchy.
        /// Note: only the first entries of this list participate in the
        /// final blend, up to MaxCameras</summary>
        public CinemachineVirtualCameraBase[] ChildCameras
        {
            get { ValidateListOfChildren(); return m_ChildCameras; }
        }

        /// <summary>Invalidate the cached list of child cameras.</summary>
        protected void InvalidateListOfChildren()
        {
            m_ChildCameras = null;
            m_indexMap = null;
            LiveChild = null;
        }

        /// <summary>Rebuild the cached list of child cameras.</summary>
        protected void ValidateListOfChildren()
        {
            if (m_ChildCameras != null)
                return;

            m_indexMap = new Dictionary<CinemachineVirtualCameraBase, int>();
            List<CinemachineVirtualCameraBase> list = new List<CinemachineVirtualCameraBase>();
            CinemachineVirtualCameraBase[] kids
                = GetComponentsInChildren<CinemachineVirtualCameraBase>(true);
            foreach (CinemachineVirtualCameraBase k in kids)
            {
                if (k.transform.parent == transform)
                {
                    int index = list.Count;
                    list.Add(k);
                    if (index < MaxCameras)
                        m_indexMap.Add(k, index);
                }
            }
            m_ChildCameras = list.ToArray();
        }

        /// <summary>Notification that this virtual camera is going live.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            CinemachineVirtualCameraBase[] children = ChildCameras;
            for (int i = 0; i < MaxCameras && i < children.Length; ++i)
            {
                CinemachineVirtualCameraBase vcam = children[i];
                if (vcam.isActiveAndEnabled && GetWeight(i) > UnityVectorExtensions.Epsilon)
                    vcam.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            }
            InternalUpdateCameraState(worldUp, deltaTime);
        }

        /// <summary>Internal use only.  Do not call this methid.
        /// Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  This implementation
        /// computes and caches the weighted blend of the tracked cameras.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            CinemachineVirtualCameraBase[] children = ChildCameras;
            LiveChild = null;
            float highestWeight = 0;
            float totalWeight = 0;
            for (int i = 0; i < MaxCameras && i < children.Length; ++i)
            {
                CinemachineVirtualCameraBase vcam = children[i];
                if (vcam.isActiveAndEnabled)
                {
                    float weight = Mathf.Max(0, GetWeight(i));
                    if (weight > UnityVectorExtensions.Epsilon)
                    {
                        totalWeight += weight;
                        if (totalWeight == weight)
                            m_State = vcam.State;
                        else
                            m_State = CameraState.Lerp(m_State, vcam.State, weight / totalWeight);

                        if (weight > highestWeight)
                        {
                            highestWeight = weight;
                            LiveChild = vcam;
                        }
                    }
                }
            }
            InvokePostPipelineStageCallback(
                this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
        }
    }
}
