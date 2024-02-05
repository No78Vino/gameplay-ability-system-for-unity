using System;
using GAS.Runtime.Ability;
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
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _renderer;
    
    bool _grounded;
    
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
        _inputActionReference.Player.PlayerFacingLeft.performed += _ => _renderer.localScale = new Vector3(-1, 1, 1);
        _inputActionReference.Player.PlayerFacingRight.performed += _ => _renderer.localScale = Vector3.one;
        
        _asc.InitWithPreset(1);
    }

    private void OnEnable()
    {
        _asc.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChange);
    }
    
    private void OnDisable()
    {
        _asc.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChange);
    }

    private void FixedUpdate()
    {
        _accY = (_rb.velocity.y - _lastVelocityY) / Time.fixedDeltaTime;

        if (_grounded || !(_accY != 0 && !_inputActionReference.Player.Move.IsPressed()))
        {
            var velocity = _rb.velocity;
            velocity.x = _velocityX * speed;
            _rb.velocity = velocity;
        }

        _lastVelocityY = _rb.velocity.y;
        
        // 设置动画机参数
        _animator.SetFloat("IsInAir", _grounded ? 0 : 1);
        _animator.SetFloat("UpOrDown", 0.5f * (1 - Mathf.Clamp(_lastVelocityY, -1, 1)));
    }

    public void SetIsGrounded(bool grounded)
    {
        _grounded = grounded;
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
        if (_grounded||DoubleJumpValid())
            _asc.TryActivateAbility(AbilityCollection.Jump_Info.Name, _rb);
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
    
    public void OnHpChange(AttributeBase attr,float oldValue, float newValue)
    {
        Debug.Log($"HP changed from {oldValue} to {newValue}");
    }
}