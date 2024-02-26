using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public class ResizeBoundingManipulator : PointerManipulator
    {
        public bool Enable = true;
        private readonly Action<Vector2> _onDragMove;

        private readonly Action<PointerDownEvent> _onDragStart;
        private readonly Action _onDragStop;

        private Vector3 m_Start;
        public float Offset = 0;

        private const int CursorWidth = 4;

        public ResizeBoundingManipulator(Action<Vector2> onDragMove)
        {
            _onDragMove = onDragMove;
            Active = false;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        public ResizeBoundingManipulator(Action<Vector2> onDragMove,
            Action<PointerDownEvent> onDragStart, Action onDragStop) : this(onDragMove)
        {
            _onDragStart = onDragStart;
            _onDragStop = onDragStop;
        }

        public bool Active { get; private set; }
        public IMGUIContainer ResizeHandle => target as IMGUIContainer;

        protected override void RegisterCallbacksOnTarget()
        {
            ResizeHandle.onGUIHandler = OnGUIHandler;
            ResizeHandle.RegisterCallback<PointerDownEvent>(OnPointerDown);
            ResizeHandle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            ResizeHandle.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            ResizeHandle.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            ResizeHandle.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            ResizeHandle.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnGUIHandler()
        {
            Handles.BeginGUI();
            var rect = ResizeHandle.worldBound;
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, CursorWidth, rect.height), MouseCursor.ResizeHorizontal);
            Handles.color = new Color(1f, 0f, 0f, 1f);
            Handles.DrawLine(new Vector3(0, 0, 0), new Vector3(0, rect.height, 0));
            Handles.DrawLine(new Vector3(CursorWidth, 0, 0), new Vector3(CursorWidth, rect.height, 0));
            Handles.EndGUI();
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
                ResizeHandle.CapturePointer(e.pointerId);
                e.StopPropagation();

                _onDragStart?.Invoke(e);
            }
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (!Enable) return;

            if (Active && ResizeHandle.HasPointerCapture(e.pointerId))
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
                ResizeHandle.ReleasePointer(e.pointerId);
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