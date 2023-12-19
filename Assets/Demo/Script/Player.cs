using System;
using Demo.Script.MyAbilitySystem.Ability;
using GAS.Runtime.AbilitySystemComponent;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AbilitySystemComponent _asc;
    public Action<Vector3> onMove { get; private set; }
    
    private void Awake()
    {
        _asc = GetComponent<AbilitySystemComponent>();
        var ability = new AbilityMove();
        _asc.Init(ability);
        
        onMove = OnMove;
    }

    public void OnMove(Vector3 direction)
    {
        _asc.TryActivateAbility("Move", direction);
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