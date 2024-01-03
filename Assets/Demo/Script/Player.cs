using System;
using Demo.Script.MyAbilitySystem.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Component;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AbilitySystemComponent _asc;
    private Rigidbody _rigidbody;
    private float speed = 0.5f;
    public Action<Vector3> onMove { get; private set; }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _asc = GetComponent<AbilitySystemComponent>();
        //var ability = new AbilityMove();
        //_asc.Init();
        //AttrSet_Fight attrSet = new AttrSet_Fight();
        onMove = OnMove;
    }

    public void OnMove(Vector3 direction)
    {
        _asc.TryActivateAbility("Move", direction,_rigidbody,speed);
    }

    public void OnMoveEnd()
    {
        _asc.EndAbility("Move");
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