using GAS.Runtime.AttributeSet;
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

        InitAttribute();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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

    void Dodge()
    {
        // TODO
    }
}