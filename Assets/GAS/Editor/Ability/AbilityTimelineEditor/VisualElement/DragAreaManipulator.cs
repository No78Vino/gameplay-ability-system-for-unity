#if  UNITY_EDITOR

namespace GAS.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    
    public enum MouseCursorType
    {
        None,
        ResizeHorizontal,
        ResizeVertical,
    }
    
    public class DragAreaManipulator : PointerManipulator
    {
        public bool Enable = true;
        private readonly Action<Vector2> _onDragMove;

        private readonly Action<PointerDownEvent> _onDragStart;
        private readonly Action _onDragStop;

        private Vector3 m_Start;
        public float Offset = 0;

        private const int CursorWidth = 6;
        private MouseCursorType _cursorType;

        public DragAreaManipulator(MouseCursorType cursorType,Action<Vector2> onDragMove)
        {
            _onDragMove = onDragMove;
            _cursorType = cursorType;
            Active = false;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        public DragAreaManipulator(MouseCursorType cursorType,Action<Vector2> onDragMove,
            Action<PointerDownEvent> onDragStart, Action onDragStop) : this(cursorType,onDragMove)
        {
            _onDragStart = onDragStart;
            _onDragStop = onDragStop;
        }

        public bool Active { get; private set; }
        public IMGUIContainer ResizeHandle => target as IMGUIContainer;

        protected override void RegisterCallbacksOnTarget()
        {
            if(ResizeHandle!=null) ResizeHandle.onGUIHandler = OnGUIHandler;
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnGUIHandler()
        {
            if (_cursorType != MouseCursorType.None)
            {
                Handles.BeginGUI();
                var rect = ResizeHandle.worldBound;
                MouseCursor mouseCursor = MouseCursor.ResizeHorizontal;
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, CursorWidth, rect.height), mouseCursor);
                Handles.EndGUI();
            }
        }
        
        private void OnPointerDown(PointerDownEvent e)
        {
            if (!Enable) return;

            if (Active)
            {
                e.StopImmediatePropagation();
            }
            else if (CanStartManipulation(e))
            {
                m_Start = e.localPosition;
                Active = true;
                target.CapturePointer(e.pointerId);
                e.StopPropagation();

                _onDragStart?.Invoke(e);
            }
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (!Enable) return;

            if (Active && target.HasPointerCapture(e.pointerId))
            {
                Vector2 delta = e.localPosition - m_Start;
                ApplyDelta(delta);
                e.StopPropagation();
            }
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            if (!Enable) return;

            if (Active && CanStopManipulation(e))
            {
                Active = false;
                target.ReleasePointer(e.pointerId);
                e.StopPropagation();

                _onDragStop?.Invoke();
            }
        }

        private void ApplyDelta(Vector2 delta)
        {
            _onDragMove?.Invoke(delta);
        }
    }
}
#endif