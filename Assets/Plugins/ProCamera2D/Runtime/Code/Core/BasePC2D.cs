using System;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public abstract class BasePC2D : MonoBehaviour, ISerializationCallbackReceiver
    {
        public ProCamera2D ProCamera2D
        {
            get
            {
                if (_pc2D != null) return _pc2D;
                
                _pc2D = GetComponent<ProCamera2D>();
                    
                if (_pc2D == null && Camera.main != null)
                    _pc2D = Camera.main.GetComponent<ProCamera2D>();
                
                if (_pc2D == null)
                    _pc2D = FindObjectOfType<ProCamera2D>();
                
                #if UNITY_EDITOR
                if (!Application.isPlaying && _pc2D != null && UnityEditor.SceneManagement.EditorSceneManager.preventCrossSceneReferences && _pc2D.gameObject.scene != gameObject.scene)
                {
                    Debug.LogWarning("ProCamera2D is in a different scene. Cross scene references are not supported during edit mode but everything will work correctly during play. Unfortunately the cross scene reference warnings are unavoidable at the moment.");
                }
                #endif

                return _pc2D;
            }

            set { _pc2D = value; }
        }
        [SerializeField]
        private ProCamera2D _pc2D;

        protected Func<Vector3, float> Vector3H;
        protected Func<Vector3, float> Vector3V;
        protected Func<Vector3, float> Vector3D;
        protected Func<float, float, Vector3> VectorHV;
        protected Func<float, float, float, Vector3> VectorHVD;

        protected Transform _transform;

        bool _enabled;
        
        [SerializeField, HideInInspector] private MovementAxis _serializedAxis;

        protected virtual void Awake()
        {
            _transform = transform;

            if(enabled)
                Enable();

            ResetAxisFunctions();
        }

        protected virtual void OnEnable()
        {
            Enable();
        }

        protected virtual void OnDisable()
        {
            Disable();
        }

        protected virtual void OnDestroy()
        {
            Disable();
        }

        /// <summary>Called when the method Reset is called on the Core. Use it to reset an extension.</summary>
        public virtual void OnReset()
        {   
        }

        void Enable()
        {
            if (_enabled || _pc2D == null)
                return;
            
            _enabled = true;
            _pc2D.OnReset += OnReset;
        }

        void Disable()
        {
            if (_pc2D != null && _enabled)
            {
                _enabled = false;
                _pc2D.OnReset -= OnReset;
            }
        }

        void ResetAxisFunctions()
        {
            if (Vector3H != null || ProCamera2D == null)
                return;

            switch (_pc2D.Axis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    VectorHVD = (h, v, d) => new Vector3(h, v, d);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    Vector3D = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    VectorHVD = (h, v, d) => new Vector3(h, d, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.x;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    VectorHVD = (h, v, d) => new Vector3(d, v, h);
                    break;
            } 
        }

        #if UNITY_EDITOR
        int _drawGizmosCounter;

        void OnDrawGizmos()
        {
            if (!enabled)
                return;

            if (_pc2D == null && Camera.main != null)
                _pc2D = Camera.main.GetComponent<ProCamera2D>();

            if (_pc2D == null)
                return;

            // Don't draw gizmos on other cameras
            if (Camera.current != _pc2D.GameCamera &&
                ((UnityEditor.SceneView.lastActiveSceneView != null && Camera.current != UnityEditor.SceneView.lastActiveSceneView.camera) ||
                (UnityEditor.SceneView.lastActiveSceneView == null)))
                return;

            ResetAxisFunctions();

            // HACK to prevent Unity bug on startup: http://forum.unity3d.com/threads/screen-position-out-of-view-frustum.9918/
            _drawGizmosCounter++;
            if (_drawGizmosCounter < 5 && UnityEditor.EditorApplication.timeSinceStartup < 60f)
                return;

            DrawGizmos();
        }
        
        void OnDrawGizmosSelected()
        {
            if (!enabled)
                return;

            if (_pc2D == null && Camera.main != null)
                _pc2D = Camera.main.GetComponent<ProCamera2D>();

            if (_pc2D == null)
                return;

            // Don't draw gizmos on other cameras
            if (Camera.current != _pc2D.GameCamera &&
                ((UnityEditor.SceneView.lastActiveSceneView != null && Camera.current != UnityEditor.SceneView.lastActiveSceneView.camera) ||
                 (UnityEditor.SceneView.lastActiveSceneView == null)))
                return;

            ResetAxisFunctions();

            // HACK to prevent Unity bug on startup: http://forum.unity3d.com/threads/screen-position-out-of-view-frustum.9918/
            _drawGizmosCounter++;
            if (_drawGizmosCounter < 5 && UnityEditor.EditorApplication.timeSinceStartup < 60f)
                return;

            DrawGizmosSelected();
        }

        protected virtual void DrawGizmos()
        {
        }
        
        protected virtual void DrawGizmosSelected()
        {
        }
        #endif
        
        #region ISerializationCallbackReceiver
        
        public void OnBeforeSerialize()
        {
            if (ProCamera2D != null)
            {
                _serializedAxis = ProCamera2D.Axis;
            }
        } 

        public void OnAfterDeserialize()
        {
            switch (_serializedAxis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    VectorHVD = (h, v, d) => new Vector3(h, v, d);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    Vector3D = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    VectorHVD = (h, v, d) => new Vector3(h, d, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.x;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    VectorHVD = (h, v, d) => new Vector3(d, v, h);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } 

        #endregion 
    }
}