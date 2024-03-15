#if  UNITY_EDITOR

namespace GAS.Editor
{
    using System;
    using UnityEngine.UIElements;
    
    public class PointerIMGUIContainer : IMGUIContainer
    {
        private Action<MouseDownEvent> _onPointerDown;
        private Action<MouseUpEvent> _onPointerUp;

        public PointerIMGUIContainer()
        {
        }
        public void OnPointerDown(MouseDownEvent eventData)
        {
            _onPointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(MouseUpEvent eventData)
        {
            _onPointerUp?.Invoke(eventData);
        }

        public void RegisterMouseDown(Action<MouseDownEvent> onPointerDown)
        {
            _onPointerDown = onPointerDown;
        }

        public void RegisterMouseUp(Action<MouseUpEvent> onPointerUp)
        {
            _onPointerUp = onPointerUp;
        }

        public new class UxmlFactory : UxmlFactory<PointerIMGUIContainer, UxmlTraits>
        {
        }
    }
}
#endif