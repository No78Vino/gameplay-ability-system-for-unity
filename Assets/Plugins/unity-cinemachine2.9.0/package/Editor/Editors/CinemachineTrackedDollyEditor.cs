using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineTrackedDolly))]
    [CanEditMultipleObjects]
    internal sealed class CinemachineTrackedDollyEditor : BaseEditor<CinemachineTrackedDolly>
    {
        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            switch (Target.m_CameraUp)
            {
                default:
                    break;
                case CinemachineTrackedDolly.CameraUpMode.PathNoRoll:
                case CinemachineTrackedDolly.CameraUpMode.FollowTargetNoRoll:
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case CinemachineTrackedDolly.CameraUpMode.Default:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            bool needWarning = false;
            for (int i = 0; !needWarning && i < targets.Length; ++i)
                needWarning = (targets[i] as CinemachineTrackedDolly).m_Path == null;
            if (needWarning)
                EditorGUILayout.HelpBox("A Path is required", MessageType.Warning);

            needWarning = false;
            for (int i = 0; !needWarning && i < targets.Length; ++i)
                needWarning = (targets[i] as CinemachineTrackedDolly).m_AutoDolly.m_Enabled 
                    && (targets[i] as CinemachineTrackedDolly).FollowTarget == null;
            if (needWarning)
                EditorGUILayout.HelpBox("AutoDolly requires a Follow Target", MessageType.Warning);

            DrawRemainingPropertiesInInspector();
        }
        
        [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy, typeof(CinemachineTrackedDolly))]
        private static void DrawTrackeDollyGizmos(CinemachineTrackedDolly target, GizmoType selectionType)
        {
            if (target.IsValid)
            {
                CinemachinePathBase path = target.m_Path;
                if (path != null)
                {
                    var isActive = CinemachineCore.Instance.IsLive(target.VirtualCamera);
                    CinemachinePathEditor.DrawPathGizmo(path, path.m_Appearance.pathColor, isActive);
                    Vector3 pos = path.EvaluatePositionAtUnit(target.m_PathPosition, target.m_PositionUnits);
                    Color oldColor = Gizmos.color;
                    Gizmos.color = isActive
                        ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
                        : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;
                    Gizmos.DrawLine(pos, target.VirtualCamera.State.RawPosition);
                    Gizmos.color = oldColor;
                }
            }
        }
    }
}
