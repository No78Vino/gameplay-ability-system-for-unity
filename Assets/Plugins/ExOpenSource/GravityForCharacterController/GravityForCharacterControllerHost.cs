using UnityEngine;

namespace EXToyLib
{
    public class GravityForCharacterControllerHost : MonoBehaviour
    {
        private void FixedUpdate()
        {
            GravityForCharacterController.Instance.UpdateGravity();
        }
    }
}