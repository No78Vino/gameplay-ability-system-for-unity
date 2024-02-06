using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FightUnit
{
    [SerializeField] private PlayerInput _playerInput;
    private DemoController _inputActionReference;
    
    protected override void Awake()
    {
        base.Awake();
        
        _inputActionReference = new DemoController();
        _inputActionReference.Enable();
        _inputActionReference.Player.Move.performed += OnActivateMove;
        _inputActionReference.Player.Move.canceled += OnDeactivateMove;
        _inputActionReference.Player.Jump.performed += OnJump;
        _inputActionReference.Player.Attack.performed += OnAttack;
        _inputActionReference.Player.Defend.performed += OnActivateDefend;
        _inputActionReference.Player.Defend.canceled += OnDeactivateDefend;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!_grounded && _lastVelocityY<=0 && _inputActionReference.Player.Jump.IsPressed())
        {
            _rb.gravityScale = HalfGravity;
        }
        else
        {
            _rb.gravityScale = Gravity;
        }
    }

    private void OnActivateMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        Debug.Log($"OnMove. Value =  {value}");
        ActivateMove(value.x);
    }

    private void OnDeactivateMove(InputAction.CallbackContext context)
    {
        DeactivateMove();
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    
    private void OnAttack(InputAction.CallbackContext context)
    {
        Attack();
    }
    
    private void OnActivateDefend(InputAction.CallbackContext context)
    {
        ActivateDefend();
    }
    
    private void OnDeactivateDefend(InputAction.CallbackContext context)
    {
        DeactivateDefend();
    }
}