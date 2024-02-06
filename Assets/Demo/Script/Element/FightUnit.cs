using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.InputSystem;


public class FightUnit : MonoBehaviour
{
    public const float Gravity = 3f;
    public const float HalfGravity = 1.5f;
    protected AbilitySystemComponent _asc;
    protected Rigidbody2D _rb;
    private float speed = 5;
    private float jumpVelocity = 10;
    
    private int _velocityX;
    protected float _lastVelocityY;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _renderer;

    protected bool _grounded;
    protected bool Moving => _asc.HasTag(GameplayTagSumCollection.Event_Moving);
    protected bool DoubleJumpValid => false; //_asc.HasTag(GameplayTagSumCollection.Event_DoubleJumpValid);

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = Gravity;
        _asc = GetComponent<AbilitySystemComponent>();
        _asc.InitWithPreset(1);
    }

    protected virtual void OnEnable()
    {
        _asc.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChange);
    }

    protected virtual void OnDisable()
    {
        _asc.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChange);
    }

    protected virtual void FixedUpdate()
    {
        if (_grounded || Moving)
        {
            var velocity = _rb.velocity;
            velocity.x = _velocityX * speed;
            _rb.velocity = velocity;
        }

        _lastVelocityY = _rb.velocity.y;

        // 设置动画机参数
        _animator.SetFloat("IsInAir", _grounded ? 0 : 1);
        _animator.SetFloat("UpOrDown", 0.5f * (1 - Mathf.Clamp(_lastVelocityY, -1, 1)));
        _animator.SetFloat("Moving", Moving ? 1 : 0);
    }

    public void SetIsGrounded(bool grounded)
    {
        _grounded = grounded;
    }

    public void ActivateMove(float direction)
    {
        if (Mathf.Abs(direction) > 0)
        {
            _velocityX = direction > 0 ? 1 : -1;
            _asc.AddFixedTag(GameplayTagSumCollection.Event_Moving);
            _renderer.localScale = new Vector3(_velocityX, 1, 1);
        }
        else _velocityX = 0;
    }

    public void DeactivateMove()
    {
        _velocityX = 0;
        _asc.RemoveFixedTag(GameplayTagSumCollection.Event_Moving);
    }

    protected void Jump()
    {
        if (_grounded || DoubleJumpValid)
            _asc.TryActivateAbility(AbilityCollection.Jump_Info.Name, _rb);
    }

    public void Attack()
    {
        // TODO
    }

    public void ActivateDefend()
    {
        // TODO
        //_asc.TryActivateAbility(AbilityCollection.Defend_Info.Name);
    }

    public void DeactivateDefend()
    {
        // TODO
        //_asc.TryEndAbility(AbilityCollection.Defend_Info.Name);
    }



    private void OnHpChange(AttributeBase attr, float oldValue, float newValue)
    {
        Debug.Log($"HP changed from {oldValue} to {newValue}");
    }
}
