using UnityEngine;
using System.Collections.Generic;
using System;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-repeater/")]
    public class ProCamera2DRepeater : BasePC2D, IPostMover
    {
        public static string ExtensionName = "Repeater";

        public Transform ObjectToRepeat;
        public Vector2 ObjectSize = new Vector2(2f, 2f);
        public Vector2 ObjectBottomLeft = Vector2.zero;

        public bool ObjectOnStage = true;

        public bool RepeatHorizontal
        {
            get{ return _repeatHorizontal; }
            set
            {
                _repeatHorizontal = value;
                Refresh();
            }
        }

        public bool _repeatHorizontal = true;

        public bool RepeatVertical
        {
            get{ return _repeatVertical; }
            set
            {
                _repeatVertical = value;
                Refresh();
            }
        }

        public bool _repeatVertical = true;

        public Camera CameraToUse;
        Transform _cameraToUseTransform;

        Vector3 _objStartPosition;

        private List<RepeatedObject> _allRepeatedObjects;

        private Queue<RepeatedObject> _inactiveRepeatedObjects;

        IntPoint _prevStartIndex;
        IntPoint _prevEndIndex;
        private Dictionary<IntPoint, bool> _occupiedIndices;

        protected override void Awake()
        {
            base.Awake();

            SetRepeatingObject(ObjectToRepeat, ObjectOnStage);

            _cameraToUseTransform = CameraToUse.transform;

            ProCamera2D.AddPostMover(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemovePostMover(this);
        }

        #region IPostMover implementation

        public void PostMove(float deltaTime)
        {
            if(!enabled)
                return;
            
            // Get screen size at the object depth
            var cameraSize = Utils.GetScreenSizeInWorldCoords(CameraToUse, Vector3D(ProCamera2D.LocalPosition - _objStartPosition));

            // Find indices
            var cameraPos = _cameraToUseTransform.position;
            var camBottomLeft = new Vector2(
                Vector3H(cameraPos) - cameraSize.x / 2,
                Vector3V(cameraPos) - cameraSize.y / 2);

            var camOffset = new Vector2(
                camBottomLeft.x - _objStartPosition.x - ObjectBottomLeft.x,
                camBottomLeft.y - _objStartPosition.y - ObjectBottomLeft.y);

            var startIndex = new IntPoint(
                Mathf.FloorToInt(camOffset.x / ObjectSize.x),
                Mathf.FloorToInt(camOffset.y / ObjectSize.y));

            var copiesNeeded = new IntPoint(
                Mathf.CeilToInt(cameraSize.x / ObjectSize.x),
                Mathf.CeilToInt(cameraSize.y / ObjectSize.y));

            var endIndex = new IntPoint(
                startIndex.X + copiesNeeded.X,
                startIndex.Y + copiesNeeded.Y);

            // If the indices change
            if (!startIndex.Equals(_prevStartIndex) || !endIndex.Equals(_prevEndIndex))
            {
                // Free out of range objects
                FreeOutOfRangeObjects(startIndex, endIndex);

                // Fill the grid
                FillGrid(startIndex, endIndex);
            }

            _prevStartIndex = startIndex;
            _prevEndIndex = endIndex;
        }

        public int PMOrder { get { return _pmOrder; } set { _pmOrder = value; } }

        int _pmOrder = 2000;

        #endregion

        public void SetRepeatingObject(Transform objectToRepeat, bool isExistingObject)
        {
            ObjectToRepeat = objectToRepeat;
            
            if (ObjectToRepeat == null)
            {
                Debug.LogWarning("ProCamera2D Repeater extension - No ObjectToRepeat defined!");
                return;
            }

            _objStartPosition = new Vector3(
                Vector3H(ObjectToRepeat.position), 
                Vector3V(ObjectToRepeat.position),
                Vector3D(ObjectToRepeat.position));

            if (_allRepeatedObjects != null && _allRepeatedObjects.Count > 0)
            {
                for (var i = 0; i < _allRepeatedObjects.Count; i++)
                {
                    Destroy(_allRepeatedObjects[i].Transform.gameObject);
                }
            }
            
            _allRepeatedObjects = new List<RepeatedObject>();
            _inactiveRepeatedObjects = new Queue<RepeatedObject>();
            _occupiedIndices = new Dictionary<IntPoint, bool>();
            _prevStartIndex = new IntPoint();
            _prevEndIndex = new IntPoint();
            
            if(isExistingObject)
                InitCopy(ObjectToRepeat);
        }

        void FreeOutOfRangeObjects(IntPoint startIndex, IntPoint endIndex)
        {
            //Go through the objects list and make them inactive if possible
            for (int i = 0; i < _allRepeatedObjects.Count; i++)
            {
                var repeatedObject = _allRepeatedObjects[i];
                if ((repeatedObject.GridPos.X != int.MaxValue && (repeatedObject.GridPos.X < startIndex.X || repeatedObject.GridPos.X > endIndex.X)) ||
                    (repeatedObject.GridPos.Y != int.MaxValue && (repeatedObject.GridPos.Y < startIndex.Y || repeatedObject.GridPos.Y > endIndex.Y)))
                {
                    // Set index as unoccupied
                    _occupiedIndices.Remove(repeatedObject.GridPos);

                    // Add item to inactive list
                    _inactiveRepeatedObjects.Enqueue(repeatedObject);

                    // Position it off-screen (it's faster then disabling it)
                    PositionObject(repeatedObject, IntPoint.MaxValue);
                }
            }
        }

        void FillGrid(IntPoint startIndex, IntPoint endIndex)
        {
            if (!_repeatHorizontal)
            {
                startIndex.X = 0;
                endIndex.X = 0;
            }

            if (!_repeatVertical)
            {
                startIndex.Y = 0;
                endIndex.Y = 0;
            }

            // Cycle horizontally
            for (int i = startIndex.X; i <= endIndex.X; i++)
            {
                // Cycle vertically
                for (int j = startIndex.Y; j <= endIndex.Y; j++)
                {
                    // Place the copies on the provided range
                    var gridPos = new IntPoint(i, j); 
                    var isIndexOccupied = false;
                    if (!_occupiedIndices.TryGetValue(gridPos, out isIndexOccupied))
                    {
                        // Create object copy if needed
                        if (_inactiveRepeatedObjects.Count == 0)
                            InitCopy(Instantiate(ObjectToRepeat) as Transform, false);

                        // Set index as occupied
                        _occupiedIndices[gridPos] = true;
                    
                        // Retrieve item from inactive list
                        var repeatedObject = _inactiveRepeatedObjects.Dequeue();
                    
                        // Position it on-screen
                        PositionObject(repeatedObject, gridPos);
                    }
                }
            }
        }

        void InitCopy(Transform newCopy, bool positionOffscreen = true)
        {
            var repeatedObject = new RepeatedObject
            {
                Transform = newCopy
            };
            
            repeatedObject.Transform.SetParent(ObjectToRepeat.parent);

            _allRepeatedObjects.Add(repeatedObject);
            _inactiveRepeatedObjects.Enqueue(repeatedObject);

            if (positionOffscreen)
                PositionObject(repeatedObject, IntPoint.MaxValue);
        }

        void PositionObject(RepeatedObject obj, IntPoint index)
        {
            obj.GridPos = index;
            obj.Transform.position = VectorHVD(
                _objStartPosition.x + index.X * ObjectSize.x,
                _objStartPosition.y + index.Y * ObjectSize.y,
                _objStartPosition.z);
        }

        void Refresh()
        {
            // Free out of range objects
            FreeOutOfRangeObjects(IntPoint.MaxValue, IntPoint.MaxValue);

            // Fill the grid
            FillGrid(_prevStartIndex, _prevEndIndex);
        }

        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                return;
            
            // Don't draw gizmos on other cameras
            if (Camera.current != CameraToUse &&
                ((UnityEditor.SceneView.lastActiveSceneView != null && Camera.current != UnityEditor.SceneView.lastActiveSceneView.camera) ||
                (UnityEditor.SceneView.lastActiveSceneView == null)))
                return;

            Gizmos.color = Color.red;

            // Draw object size and origin
            if (ObjectToRepeat != null && ObjectOnStage)
            {
                var origin = ObjectToRepeat.position + VectorHV(ObjectBottomLeft.x, ObjectBottomLeft.y);

                if (_repeatHorizontal)
                {
                    Gizmos.DrawLine(origin, origin + VectorHV(ObjectSize.x, 0));
                    Utils.DrawArrowForGizmo(origin + VectorHV(ObjectSize.x - ObjectSize.x * .25f, 0), ObjectToRepeat.right * ObjectSize.x * .25f, ObjectSize.x * .25f);
                }

                if (_repeatVertical)
                {
                    Gizmos.DrawLine(origin, origin + VectorHV(0, ObjectSize.y));
                    Utils.DrawArrowForGizmo(origin + VectorHV(0, ObjectSize.y - ObjectSize.y * .25f), ObjectToRepeat.up * ObjectSize.y * .25f, ObjectSize.y * .25f);
                }
            }
        }
        #endif
    }

    struct IntPoint: IEquatable<IntPoint>
    {
        public static IntPoint MaxValue = new IntPoint
        {
            X = int.MaxValue,
            Y = int.MaxValue
        };

        public int X;
        public int Y;

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsEqual(IntPoint other)
        {
            return other.X == X && other.Y == Y;
        }

        public override string ToString()
        {
            return string.Format("X: " + X + " - Y: " + Y);
        }

        #region IEquatable implementation

        public bool Equals(IntPoint other)
        {
            return other.X == X && other.Y == Y;
        }

        #endregion

        public override int GetHashCode()
        {
            var result = 0;
            result = (result * 397) ^ X;
            result = (result * 397) ^ Y;
            return result;
        }
    }

    class RepeatedObject
    {
        public IntPoint GridPos;
        public Transform Transform;
    }
}