using System;
using Demo.Script.MyAbilitySystem.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Component;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private AbilitySystemComponent _asc;
    private Rigidbody2D _rb;
    private float speed = 5;
    private float jumpVelocity = 10;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private DemoController _inputActionReference; 
    private Vector2 _motion;
    private float _velocityX;
    private float _accY;
    private float _lastVelocityY;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _asc = GetComponent<AbilitySystemComponent>();
        //var ability = new AbilityMove();
        //_asc.Init();
        //AttrSet_Fight attrSet = new AttrSet_Fight();
        _inputActionReference = new DemoController();
        _inputActionReference.Enable();
        _inputActionReference.Player.Move.performed += OnMove;
        _inputActionReference.Player.Jump.performed += OnJump;
    }

    private void FixedUpdate()
    {
        _accY = (_rb.velocity.y - _lastVelocityY) / Time.fixedDeltaTime;

        if (IsGrounded() || !(_accY != 0 && !_inputActionReference.Player.Move.IsPressed()))
        {
            var velocity = _rb.velocity;
            velocity.x = _velocityX * speed;
            _rb.velocity = velocity;
        }

        _lastVelocityY = _rb.velocity.y;
    }

    bool IsGrounded()
    {
        return _rb.velocity.y==0 && _accY >= 0; // && !LockMotion();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        Debug.Log($"OnMove. Value =  {value}");
        _motion = value;
        _velocityX = value.x;
    }    
    
    public void OnJump(InputAction.CallbackContext context)
    {
        var velocity = _rb.velocity;
        if (IsGrounded()||DoubleJumpValid())
        {
            velocity.y = jumpVelocity;
            _rb.velocity = velocity;
        }
    }

    bool DoubleJumpValid()
    {
        return false;
    }

    public void OnMoveEnd()
    {
        //_asc.TryEndAbility("Move");
    }
    public void OnPressQ()
    {
        //_asc.TryActivateAbility("Q");
    }

    public void OnPressE()
    {
        //_asc.TryActivateAbility("E");
    }

    public void OnPressR()
    {
        //_asc.TryActivateAbility("R");
    }

    public void OnPressMouseLeft()
    {
        //_asc.TryActivateAbility("MouseLeft");
    }

    public void OnMousePosition(Vector3 position)
    {
        //_asc.TryActivateAbility("MousePosition", position);
    }
}