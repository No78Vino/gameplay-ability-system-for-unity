using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that adds a final tweak to the camera
/// comnposition.  It is intended for use in a Timeline context, where you want to hand-adjust
/// the output of procedural or recorded camera aiming.
/// </summary>
[AddComponentMenu("")] // Hide in menu
[ExecuteAlways]
[SaveDuringPlay]
[HelpURL(Documentation.BaseURL + "manual/CinemachineRecomposer.html")]
public class CinemachineRecomposer : CinemachineExtension
{
    /// <summary>
    /// When to apply the adjustment
    /// </summary>
    [Tooltip("When to apply the adjustment")]
    public CinemachineCore.Stage m_ApplyAfter;

    /// <summary>
    /// Tilt the camera by this much
    /// </summary>
    [Tooltip("Tilt the camera by this much")]
    public float m_Tilt;

    /// <summary>
    /// Pan the camera by this much
    /// </summary>
    [Tooltip("Pan the camera by this much")]
    public float m_Pan;

    /// <summary>
    /// Roll the camera by this much
    /// </summary>
    [Tooltip("Roll the camera by this much")]
    public float m_Dutch;

    /// <summary>
    /// Scale the zoom by this amount (normal = 1)
    /// </summary>
    [Tooltip("Scale the zoom by this amount (normal = 1)")]
    public float m_ZoomScale;

    /// <summary>
    /// Lowering this value relaxes the camera's attention to the Follow target (normal = 1)
    /// </summary>
    [Range(0, 1)]
    [Tooltip("Lowering this value relaxes the camera's attention to the Follow target (normal = 1)")]
    public float m_FollowAttachment;

    /// <summary>
    /// Lowering this value relaxes the camera's attention to the LookAt target (normal = 1)
    /// </summary>
    [Range(0, 1)]
    [Tooltip("Lowering this value relaxes the camera's attention to the LookAt target (normal = 1)")]
    public float m_LookAtAttachment;

    private void Reset()
    {
        m_ApplyAfter = CinemachineCore.Stage.Finalize;
        m_Tilt = 0;
        m_Pan = 0;
        m_Dutch = 0;
        m_ZoomScale = 1;
        m_FollowAttachment = 1;
        m_LookAtAttachment = 1;
    }

    private void OnValidate()
    {
        m_ZoomScale = Mathf.Max(0.01f, m_ZoomScale);
        m_FollowAttachment = Mathf.Clamp01(m_FollowAttachment);
        m_LookAtAttachment = Mathf.Clamp01(m_LookAtAttachment);
    }

    /// <summary>Callback to set the target attachment</summary>
    /// <param name="vcam">The virtual camera being processed</param>
    /// <param name="curState">Input state that must be mutated</param>
    /// <param name="deltaTime">The current applicable deltaTime</param>
    public override void PrePipelineMutateCameraStateCallback(
        CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime) 
    {
        vcam.FollowTargetAttachment = m_FollowAttachment;
        vcam.LookAtTargetAttachment = m_LookAtAttachment;
    }

    /// <summary>Callback to tweak the settings</summary>
    /// <param name="vcam">The virtual camera being processed</param>
    /// <param name="stage">The current pipeline stage</param>
    /// <param name="state">The current virtual camera state</param>
    /// <param name="deltaTime">The current applicable deltaTime</param>
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == m_ApplyAfter)
        {
            var lens = state.Lens;

            // Tilt by local X
            var qTilted = state.RawOrientation * Quaternion.AngleAxis(m_Tilt, Vector3.right);
            // Pan in world space
            var qDesired = Quaternion.AngleAxis(m_Pan, state.ReferenceUp) * qTilted;
            state.OrientationCorrection = Quaternion.Inverse(state.CorrectedOrientation) * qDesired;
            // And dutch at the end
            lens.Dutch += m_Dutch;
            // Finally zoom
            if (m_ZoomScale != 1)
            {
                lens.OrthographicSize *= m_ZoomScale;
                lens.FieldOfView *= m_ZoomScale;
            }
            state.Lens = lens;
        }
    }
}

