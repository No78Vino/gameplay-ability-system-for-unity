using System.Collections.Generic;
using UnityEditor;

namespace GAS.Editor
{
    public class DirtyManager
    {
        private static HashSet<UnityEngine.Object> _dirtyObjects = new HashSet<UnityEngine.Object>();
        private static double _lastMarkTime;
        private const double MARK_INTERVAL = 0.5; // 500ms间隔

        public static void MarkDirty(UnityEngine.Object obj)
        {
            _dirtyObjects.Add(obj);
        
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - _lastMarkTime >= MARK_INTERVAL)
            {
                FlushDirty();
            }
        }

        public static void FlushDirty()
        {
            if (_dirtyObjects.Count == 0) return;

            foreach (var obj in _dirtyObjects)
            {
                if (obj != null)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
            _dirtyObjects.Clear();
            _lastMarkTime = EditorApplication.timeSinceStartup;
        }
    }
    public class BatchOperationManager 
    {
        private bool _isBatchOperating;
        private HashSet<UnityEngine.Object> _modifiedObjects = new HashSet<UnityEngine.Object>();

        public void BeginBatchOperation()
        {
            _isBatchOperating = true;
            _modifiedObjects.Clear();
        }

        public void MarkObjectDirty(UnityEngine.Object obj)
        {
            if (_isBatchOperating)
            {
                _modifiedObjects.Add(obj);
            }
            else
            {
                EditorUtility.SetDirty(obj);
            }
        }

        public void EndBatchOperation()
        {
            if (!_isBatchOperating) return;
        
            foreach (var obj in _modifiedObjects)
            {
                if (obj != null)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
        
            _modifiedObjects.Clear();
            _isBatchOperating = false;
        }
    }
    public class DelayedDirtyManager
    {
        private static HashSet<UnityEngine.Object> _pendingDirtyObjects = new HashSet<UnityEngine.Object>();
        private static bool _isSubscribed;

        public static void MarkDirtyDelayed(UnityEngine.Object obj)
        {
            _pendingDirtyObjects.Add(obj);
        
            if (!_isSubscribed)
            {
                EditorApplication.update += ProcessDirtyObjects;
                _isSubscribed = true;
            }
        }

        private static void ProcessDirtyObjects()
        {
            if (_pendingDirtyObjects.Count == 0)
            {
                EditorApplication.update -= ProcessDirtyObjects;
                _isSubscribed = false;
                return;
            }

            foreach (var obj in _pendingDirtyObjects)
            {
                if (obj != null)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
            _pendingDirtyObjects.Clear();
        }
    }
}