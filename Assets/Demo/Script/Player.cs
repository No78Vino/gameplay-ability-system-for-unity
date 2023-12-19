using GAS.Runtime.AbilitySystemComponent;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AbilitySystemComponent _asc;

    private void Awake()
    {
        _asc = GetComponent<AbilitySystemComponent>();
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