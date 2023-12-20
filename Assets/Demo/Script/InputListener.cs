using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Script
{
    public class InputListener : MonoBehaviour
    {
        private Action<Vector3> _onMove;
        private Action _onMoveEnd;
        private Action _onPressQ;
        private Action _onPressE;
        private Action _onPressR;
        private Action _onPressMouseLeft;
        private Action<Vector3> _onMousePosition;

        private bool isMoveKeysDown;
        private Vector3 moveDirection = Vector3.zero;
        Dictionary<KeyCode,Vector3> _keyDirectionMap = new Dictionary<KeyCode, Vector3>()
        {
            {KeyCode.A,Vector3.left},
            {KeyCode.D,Vector3.right},
            {KeyCode.W,Vector3.forward},
            {KeyCode.S,Vector3.back},
        };
        
        private void Update()
        {
            CheckMotion();
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _onPressQ?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                _onPressE?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                _onPressR?.Invoke();
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _onPressMouseLeft?.Invoke();
            }
            
            _onMousePosition?.Invoke(Input.mousePosition);
        }

        void CheckMotion()
        {
            moveDirection = Vector3.zero;
            isMoveKeysDown = false;
            foreach (var kv in _keyDirectionMap)
            {
                bool isPressed = Input.GetKey(kv.Key);
                isMoveKeysDown = isMoveKeysDown || isPressed;
                if (isPressed) moveDirection += kv.Value;
            }
            Debug.Log($"isMoveKeysDown = {isMoveKeysDown}");
            
            if (isMoveKeysDown)
            {
                _onMove?.Invoke(moveDirection);
            }
            else
            {
                _onMoveEnd?.Invoke();
            }
        }
        
        public void RegisterOnMove(Action<Vector3> onMove)
        {
            _onMove = onMove;
        }
        
        public void RegisterOnMoveEnd(Action onMoveEnd)
        {
            _onMoveEnd = onMoveEnd;
        }
        
        public void RegisterOnPressQ(Action onPressQ)
        {
            _onPressQ = onPressQ;
        }
        
        public void RegisterOnPressE(Action onPressE)
        {
            _onPressE = onPressE;
        }
        
        public void RegisterOnPressR(Action onPressR)
        {
            _onPressR = onPressR;
        }
        
        public void RegisterOnPressMouseLeft(Action onPressMouseLeft)
        {
            _onPressMouseLeft = onPressMouseLeft;
        }
        
        public void RegisterOnMousePosition(Action<Vector3> onMousePosition)
        {
            _onMousePosition = onMousePosition;
        }
    }
}