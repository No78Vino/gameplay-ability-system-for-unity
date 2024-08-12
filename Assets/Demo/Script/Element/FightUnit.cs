﻿using BehaviorDesigner.Runtime;
using UnityEngine;
using GAS.Runtime;

public abstract class FightUnit : MonoBehaviour
{
    public const int PostureMax = 100;
    
    protected const float Gravity = 3f;
    protected const float HalfGravity = 1.5f;
    private static readonly int IsInAir = Animator.StringToHash("IsInAir");
    private static readonly int UpOrDown = Animator.StringToHash("UpOrDown");
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Defending = Animator.StringToHash("Defending");
    [SerializeField] protected Animator _animator;
    [SerializeField] protected BoxCollider2D defendArea;
    [SerializeField] protected GameplayEffectAsset gePostureReductionBuff;
    [SerializeField] protected GameplayEffectAsset geOutOfPostureBuff;
    protected BehaviorTree _bt;
    protected Rigidbody2D _rb;
    private int _velocityX;
    protected float LastVelocityY;
    public bool Grounded { get; protected set; }
    public FightUnit target { get; protected set; }

    public AbilitySystemComponent ASC { get; private set; }

    public Transform Renderer => transform;
    public Rigidbody2D Rb => _rb;
    public BoxCollider2D DefendArea => defendArea;
    public float VelocityX => _velocityX;
    private bool IsMoving => ASC.HasTag(GTagLib.Event_Moving);
    public Animator Animator => _animator;
    private bool DoubleJumpValid => false; //_asc.HasTag(GTagLib.Event_DoubleJumpValid);

    protected virtual void Awake()
    {
        _bt = GetComponent<BehaviorTree>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = Gravity;
        ASC = GetComponent<AbilitySystemComponent>();
        ASC.InitWithPreset(1);

        ASC.ApplyGameplayEffectToSelf(new GameplayEffect(gePostureReductionBuff));
    }

    protected virtual void FixedUpdate()
    {
        if (!ASC.HasTag(GTagLib.Ban_Motion))
        {
            if (Grounded || IsMoving)
            {
                var velocity = _rb.velocity;
                velocity.x = _velocityX * ASC.AttrSet<AS_Fight>().SPEED.CurrentValue;
                _rb.velocity = velocity;
            }

            LastVelocityY = _rb.velocity.y;
        }

        // 设置动画机参数
        _animator.SetFloat(IsInAir, Grounded ? 0 : 1);
        _animator.SetFloat(UpOrDown, 0.5f * (1 - Mathf.Clamp(LastVelocityY, -1, 1)));
        var isDefending = ASC.HasTag(GTagLib.Event_Defending);
        _animator.SetFloat(Moving, IsMoving && !isDefending ? 1 : 0);
        _animator.SetBool(Defending, isDefending);
        
        // 
    }

    protected virtual void OnEnable()
    {
        ASC.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChange);
        ASC.AttrSet<AS_Fight>().POSTURE.RegisterPostBaseValueChange(OnPostureChange);
    }

    protected virtual void OnDisable()
    {
        ASC.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChange);
        ASC.AttrSet<AS_Fight>().POSTURE.UnregisterPostBaseValueChange(OnPostureChange);
    }

    protected bool IsAlive()
    {
        return ASC.AttrSet<AS_Fight>().HP.CurrentValue > 0;
    }

    protected bool InDeath()
    {
        return ASC.HasTag(GTagLib.State_Debuff_Death);
    }
    
    public abstract void InitAttribute();

    public void SetIsGrounded(bool grounded)
    {
        if (Grounded != grounded)
        {
            if (grounded)
                ASC.RemoveFixedTag(GTagLib.Event_InAir);
            else
                ASC.AddFixedTag(GTagLib.Event_InAir);
        }

        Grounded = grounded;
    }

    public void ActivateMove(float direction)
    {
        ASC.TryActivateAbility(MoveName, new Move.Args(direction));
    }

    public void DeactivateMove()
    {
        ASC.TryEndAbility(MoveName);
    }

    public void Jump()
    {
        if (Grounded || DoubleJumpValid)
            ASC.TryActivateAbility(JumpName, new Jump.Args(_rb));
    }

    public bool Attack()
    {
        return ASC.TryActivateAbility(AttackName);
    }

    public void ActivateDefend()
    {
        ASC.TryActivateAbility(DefendName);
    }

    public void DeactivateDefend()
    {
        ASC.TryEndAbility(DefendName);
        // 移除防御Buff
        ASC.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(new GameplayTagSet(GTagLib.State_Buff_DefendBuff));
    }

    public void Dodge()
    {
        ASC.TryActivateAbility(DodgeName);
    }

    public void Die()
    {
        ASC.TryActivateAbility(DieName);
    }

    private void OnHpChange(AttributeBase attr, float oldValue, float newValue)
    {
        Debug.Log($"HP changed from {oldValue} to {newValue}");
    }
    
    private void OnPostureChange(AttributeBase attr, float oldValue, float newValue)
    {
        Debug.Log($"Posture changed from {oldValue} to {newValue}");

        if (newValue >= PostureMax)
        {
            // 触发失衡Buff
            ASC.ApplyGameplayEffectToSelf(new GameplayEffect(geOutOfPostureBuff));
            ASC.AttrSet<AS_Fight>().POSTURE.SetBaseValue(0);
            
            // 打断所有能力
            foreach (var abilityName in ASC.AbilityContainer.AbilitySpecs().Keys)
                ASC.TryEndAbility(abilityName);
        }
    }

    public void SetVelocityX(int velocityX)
    {
        _velocityX = velocityX;
    }

    public virtual bool CatchTarget()
    {
        if (target == null)
            return false;

        var deltaVector3 = target.transform.position - transform.position;
        var distance = deltaVector3.magnitude;
        return distance < 3;
    }

    #region AbilityName

    protected abstract string MoveName { get; }
    protected abstract string JumpName { get; }
    protected abstract string AttackName { get; }
    protected abstract string DefendName { get; }
    protected abstract string DodgeName { get; }
    protected abstract string DieName { get; }

    #endregion
}