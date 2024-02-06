using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;
using UnityEngine;

public class FightUnit : MonoBehaviour
{
    protected const float Gravity = 3f;
    protected const float HalfGravity = 1.5f;
    private static readonly int IsInAir = Animator.StringToHash("IsInAir");
    private static readonly int UpOrDown = Animator.StringToHash("UpOrDown");
    private static readonly int Moving = Animator.StringToHash("Moving");
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _renderer;

    protected Rigidbody2D _rb;

    private int _velocityX;
    
    protected bool Grounded;
    private float jumpVelocity = 10;
    protected float LastVelocityY;

    public AbilitySystemComponent ASC { get; private set; }

    public Transform Renderer => _renderer;
    public float VelocityX => _velocityX;
    private bool IsMoving => ASC.HasTag(GameplayTagSumCollection.Event_Moving);
    private bool DoubleJumpValid => false; //_asc.HasTag(GameplayTagSumCollection.Event_DoubleJumpValid);

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = Gravity;
        ASC = GetComponent<AbilitySystemComponent>();
        ASC.InitWithPreset(1);
    }

    protected virtual void FixedUpdate()
    {
        if (Grounded || IsMoving)
        {
            var velocity = _rb.velocity;
            velocity.x = _velocityX * ASC.AttrSet<AS_Fight>().SPEED.CurrentValue;
            _rb.velocity = velocity;
        }

        LastVelocityY = _rb.velocity.y;

        // 设置动画机参数
        _animator.SetFloat(IsInAir, Grounded ? 0 : 1);
        _animator.SetFloat(UpOrDown, 0.5f * (1 - Mathf.Clamp(LastVelocityY, -1, 1)));
        _animator.SetFloat(Moving, IsMoving ? 1 : 0);
    }

    protected virtual void OnEnable()
    {
        ASC.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChange);
    }

    protected virtual void OnDisable()
    {
        ASC.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChange);
    }

    public virtual void InitAttribute()
    {
        
    }
    
    public void SetIsGrounded(bool grounded)
    {
        Grounded = grounded;
    }

    public void ActivateMove(float direction)
    {
        ASC.TryActivateAbility(AbilityCollection.Move_Info.Name, direction);
    }

    public void DeactivateMove()
    {
        ASC.TryEndAbility(AbilityCollection.Move_Info.Name);
    }

    public void Jump()
    {
        if (Grounded || DoubleJumpValid)
            ASC.TryActivateAbility(AbilityCollection.Jump_Info.Name, _rb);
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
    
    public void SetVelocityX(int velocityX)
    {
        _velocityX = velocityX;
    }
}