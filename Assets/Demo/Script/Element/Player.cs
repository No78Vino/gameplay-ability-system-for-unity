using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FightUnit
{
    private DemoController _inputActionReference;
    private const int HpMax = 100;
    private const int MpMax = 100;
    private const int StaminaMax = 100;
    private const int PostureMax = 10;
    private const int ATK = 10;
    private const int Speed = 5;

    [SerializeField]private GameplayEffectAsset GEBuffStaminaRecover;
    
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
        _inputActionReference.Player.Dodge.performed += OnDodge;

        InitAttribute();
        ASC.ApplyGameplayEffectToSelf(new GameplayEffect(GEBuffStaminaRecover));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ASC.AttrSet<AS_Fight>().STAMINA.RegisterPreBaseValueChange(OnStaminaChangePre);
        ASC.AttrSet<AS_Fight>().STAMINA.RegisterPostBaseValueChange(OnStaminaChangePost);

        ASC.AttrSet<AS_Fight>().HP.RegisterPreBaseValueChange(OnHpChangePre);
        ASC.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChangePost);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ASC.AttrSet<AS_Fight>().STAMINA.UnregisterPostBaseValueChange(OnStaminaChangePost);
        ASC.AttrSet<AS_Fight>().STAMINA.UnregisterPreBaseValueChange(OnStaminaChangePre);
        
        ASC.AttrSet<AS_Fight>().HP.UnregisterPreBaseValueChange(OnHpChangePre);
        ASC.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChangePost);
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (_inputActionReference.Player.Move.IsPressed())
        {
            ActivateMove(_inputActionReference.Player.Move.ReadValue<Vector2>().x);
        }
        
        if (!Grounded && LastVelocityY<=0 && _inputActionReference.Player.Jump.IsPressed())
        {
            _rb.gravityScale = HalfGravity;
        }
        else
        {
            _rb.gravityScale = Gravity;
        }
    }

    public override void InitAttribute()
    {
        ASC.AttrSet<AS_Fight>().InitHP(HpMax);
        ASC.AttrSet<AS_Fight>().InitMP(MpMax);
        ASC.AttrSet<AS_Fight>().InitSTAMINA(StaminaMax);
        ASC.AttrSet<AS_Fight>().InitPOSTURE(PostureMax);
        ASC.AttrSet<AS_Fight>().InitATK(ATK);
        ASC.AttrSet<AS_Fight>().InitSPEED(Speed);
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

    public void OnDodge(InputAction.CallbackContext context)
    {
        Dodge();
    }

    void Dodge()
    {
        ASC.TryActivateAbility(AbilityCollection.PlayerDodge_Info.Name);
    }
    
    private float OnStaminaChangePre(AttributeBase attr, float newValue)
    {
        return  Mathf.Clamp(newValue,0,StaminaMax);
    }
    
    private void OnStaminaChangePost(AttributeBase attr, float oldValue, float newValue)
    {
        Debug.Log($"Stamina changed from {oldValue} to {newValue}");
        XUI.M.VM<MainUIVM>().UpdateStamina(newValue);
    }
    
    private float OnHpChangePre(AttributeBase attr, float newValue)
    {
        return  Mathf.Clamp(newValue,0,HpMax);
    }
    
    private void OnHpChangePost(AttributeBase attr, float oldValue, float newValue)
    {
        Debug.Log($"HP changed from {oldValue} to {newValue}");
        XUI.M.VM<MainUIVM>().UpdateHp(newValue);
    }
}