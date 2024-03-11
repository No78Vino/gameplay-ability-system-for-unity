using UnityEngine;
using System.Collections.Generic;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum FollowMode
    {
        BothAxis,
        HorizontalAxis,
        VerticalAxis
    }

    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-rails/")]
    public class ProCamera2DRails : BasePC2D, IPreMover
    {
        public static string ExtensionName = "Rails";

        [HideInInspector]
        public List<Vector3> RailNodes = new List<Vector3>();

        public FollowMode FollowMode;

        public List<CameraTarget> CameraTargets = new List<CameraTarget>();

        Dictionary<CameraTarget, Transform> _cameraTargetsOnRails = new Dictionary<CameraTarget, Transform>();

        List<CameraTarget> _tempCameraTargets = new List<CameraTarget>();

        KDTree _kdTree;

        override protected void Awake()
        {
            base.Awake();

            _kdTree = KDTree.MakeFromPoints(RailNodes.ToArray());

            for (int i = 0; i < CameraTargets.Count; i++)
            {
                var railTransform = new GameObject(CameraTargets[i].TargetTransform.name + "_OnRails").transform;
                _cameraTargetsOnRails.Add(
                    CameraTargets[i],
                    railTransform
                    );

                var cameraTarget = ProCamera2D.AddCameraTarget(railTransform);
                cameraTarget.TargetOffset = CameraTargets[i].TargetOffset;
            }

            if (CameraTargets.Count == 0)
                enabled = false;

            ProCamera2D.AddPreMover(this);

            Step();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemovePreMover(this);
        }

        #region IPreMover implementation

        public void PreMove(float deltaTime)
        {
            if (enabled)
                Step();
        }

        public int PrMOrder { get { return _prmOrder; } set { _prmOrder = value; } }

        int _prmOrder = 1000;

        #endregion

        void Step()
        {
            var pos = Vector3.zero;
            for (int i = 0; i < CameraTargets.Count; i++)
            {
                switch (FollowMode)
                {
                    case FollowMode.HorizontalAxis:
                        pos = VectorHVD(
                            Vector3H(CameraTargets[i].TargetPosition) * CameraTargets[i].TargetInfluenceH, 
                            Vector3V(ProCamera2D.LocalPosition), 
                            0);
                        break;

                    case FollowMode.VerticalAxis:
                        pos = VectorHVD(
                            Vector3H(ProCamera2D.LocalPosition), 
                            Vector3V(CameraTargets[i].TargetPosition) * CameraTargets[i].TargetInfluenceV, 
                            0);
                        break;

                    case FollowMode.BothAxis:
                        pos = VectorHVD(
                            Vector3H(CameraTargets[i].TargetPosition) * CameraTargets[i].TargetInfluenceH, 
                            Vector3V(CameraTargets[i].TargetPosition) * CameraTargets[i].TargetInfluenceV,
                            0);
                        break;
                }
                _cameraTargetsOnRails[CameraTargets[i]].position = GetPositionOnRail(pos);
            }
        }

        /// <summary>Add a target for the camera to follow on rails.</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        /// <param name="targetInfluenceH">The influence this target horizontal position should have when calculating the average position of all the targets</param>
        /// <param name="targetInfluenceV">The influence this target vertical position should have when calculating the average position of all the targets</param>
        /// <param name="targetOffset">A vector that offsets the target position that the camera will follow</param>
        /// <param name="duration">The time it takes for this target to reach it's influence. Use for a more progressive transition.</param>
        public void AddRailsTarget(Transform targetTransform, float targetInfluenceH = 1f, float targetInfluenceV = 1f, Vector2 targetOffset = default(Vector2), float duration = 0f)
        {
            if (GetRailsTarget(targetTransform) != null)
                return;
            
            var newCameraTarget = new CameraTarget()
            {
                TargetTransform = targetTransform,
                TargetInfluenceH = targetInfluenceH,
                TargetInfluenceV = targetInfluenceV,
                TargetOffset = targetOffset
            };

            CameraTargets.Add(newCameraTarget);

            var railTransform = new GameObject(targetTransform.name + "_OnRails").transform;

            _cameraTargetsOnRails.Add(
                newCameraTarget,
                railTransform
            );

            ProCamera2D.AddCameraTarget(railTransform, targetInfluenceH, targetInfluenceV, duration, targetOffset);

            enabled = true;
        }

        /// <summary>Removes a target from being followed on the rails</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        public void RemoveRailsTarget(Transform targetTransform)
        {
            var cameraTarget = GetRailsTarget(targetTransform);

            if (cameraTarget != null)
            {
                CameraTargets.Remove(cameraTarget);
                ProCamera2D.RemoveCameraTarget(_cameraTargetsOnRails[cameraTarget]);
                Destroy(_cameraTargetsOnRails[cameraTarget].gameObject);
            }
        }

        /// <summary>Gets the corresponding CameraTarget from an object's transform.</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        public CameraTarget GetRailsTarget(Transform targetTransform)
        {
            for (int i = 0; i < CameraTargets.Count; i++)
            {
                if (CameraTargets[i].TargetTransform.GetInstanceID() == targetTransform.GetInstanceID())
                {
                    return CameraTargets[i];
                }
            }
            return null;
        }

        /// <summary>Disables the rails targets. This method is used by the Rails trigger</summary>
        /// <param name="transitionDuration">The time it takes to disable the targets</param>
        public void DisableTargets(float transitionDuration = 0f)
        {
            if(_tempCameraTargets.Count != 0)
                return;
                
            for (int i = 0; i < _cameraTargetsOnRails.Count; i++)
            {
                ProCamera2D.RemoveCameraTarget(_cameraTargetsOnRails[CameraTargets[i]], transitionDuration);
                _tempCameraTargets.Add(ProCamera2D.AddCameraTarget(CameraTargets[i].TargetTransform, CameraTargets[i].TargetInfluenceH, CameraTargets[i].TargetInfluenceV, transitionDuration, CameraTargets[i].TargetOffset));
            }
        }

        /// <summary>Enables the rails targets. This method is used by the Rails trigger</summary>
        /// <param name="transitionDuration">The time it takes to enable the targets</param>
        public void EnableTargets(float transitionDuration = 0f)
        {
            for (int i = 0; i < _tempCameraTargets.Count; i++)
            {
                ProCamera2D.RemoveCameraTarget(_tempCameraTargets[i].TargetTransform, transitionDuration);
                ProCamera2D.AddCameraTarget(_cameraTargetsOnRails[CameraTargets[i]], 1, 1, transitionDuration);
            }
            
            _tempCameraTargets.Clear();
        }

        Vector3 GetPositionOnRail(Vector3 pos)
        {
            var closestNode = _kdTree.FindNearest(pos);

            if (closestNode == 0)
            {
                return GetPositionOnRailSegment(RailNodes[0], RailNodes[1], pos);
            }
            else if (closestNode == RailNodes.Count - 1)
            {
                return GetPositionOnRailSegment(RailNodes[RailNodes.Count - 1], RailNodes[RailNodes.Count - 2], pos);
            }
            else
            {
                var leftSeg = GetPositionOnRailSegment(RailNodes[closestNode - 1], RailNodes[closestNode], pos);
                var rightSeg = GetPositionOnRailSegment(RailNodes[closestNode + 1], RailNodes[closestNode], pos);

                if ((pos - leftSeg).sqrMagnitude <= (pos - rightSeg).sqrMagnitude)
                    return leftSeg;
                else
                    return rightSeg;
            }
        }

        Vector3 GetPositionOnRailSegment(Vector3 node1, Vector3 node2, Vector3 pos)
        {
            var node1ToPos = pos - node1;
            var segmentDirection = (node2 - node1).normalized;

            float node1Dot = Vector3.Dot(segmentDirection, node1ToPos);

            if (node1Dot < 0f)
            {
                return node1;
            }
            else if (node1Dot * node1Dot > (node2 - node1).sqrMagnitude)
            {
                return node2;
            }
            else
            {
                var fromNode1 = segmentDirection * node1Dot;
                return node1 + fromNode1;
            }
        }

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(ProCamera2D.GameCamera, Mathf.Abs(Vector3D(transform.localPosition)));

            Gizmos.color = EditorPrefsX.GetColor(PrefsData.RailsColorKey, PrefsData.RailsColorValue);
            
            if(_tempCameraTargets.Count > 0)
                Gizmos.color = Gizmos.color * .7f;

            // Rails nodes
            for (int i = 0; i < RailNodes.Count; i++)
            {
                Gizmos.DrawWireCube(RailNodes[i], cameraDimensions * .01f);
            }

            // Rails path
            if (RailNodes.Count >= 2)
            {
                for (int i = 0; i < RailNodes.Count - 1; i++)
                {
                    Gizmos.DrawLine(RailNodes[i], RailNodes[i + 1]);
                }
            }
        }
        #endif
    }
}