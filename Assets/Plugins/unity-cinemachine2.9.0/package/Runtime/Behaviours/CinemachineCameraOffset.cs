using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that adds a final offset to the camera
/// </summary>
[AddComponentMenu("")] // Hide in menu
[ExecuteAlways]
[HelpURL(Documentation.BaseURL + "api/Cinemachine.CinemachineCameraOffset.html")]
[SaveDuringPlay]
public class CinemachineCameraOffset : CinemachineExtension
{
    /// <summary>
    /// Offset the camera's position by this much (camera space)
    /// </summary>
    [Tooltip("Offset the camera's position by this much (camera space)")]
    public Vector3 m_Offset = Vector3.zero;

    /// <summary>
    /// When to apply the offset
    /// </summary>
    [Tooltip("When to apply the offset")]
    public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Aim;

    /// <summary>
    /// If applying offset after aim, re-adjust the aim to preserve the screen position
    /// of the LookAt target as much as possible
    /// </summary>
    [Tooltip("If applying offset after aim, re-adjust the aim to preserve the screen position"
        + " of the LookAt target as much as possible")]
    public bool m_PreserveComposition;

    /// <summary>
    /// Applies the specified offset to the camera state
    /// </summary>
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
            bool preserveAim = m_PreserveComposition
                && state.HasLookAt && stage > CinemachineCore.Stage.Body;

            Vector3 screenOffset = Vector2.zero;
            if (preserveAim)
            {
                screenOffset = state.RawOrientation.GetCameraRotationToTarget(
                    state.ReferenceLookAt - state.CorrectedPosition, state.ReferenceUp);
            }

            Vector3 offset = state.RawOrientation * m_Offset;
            state.PositionCorrection += offset;
            if (!preserveAim)
                state.ReferenceLookAt += offset;
            else
            {
                var q = Quaternion.LookRotation(
                    state.ReferenceLookAt - state.CorrectedPosition, state.ReferenceUp);
                q = q.ApplyCameraRotation(-screenOffset, state.ReferenceUp);
                state.RawOrientation = q;
            }
        }
    }
}
