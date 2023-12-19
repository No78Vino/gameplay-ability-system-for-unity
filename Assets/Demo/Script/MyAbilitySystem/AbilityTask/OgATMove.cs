using GAS.Runtime.Ability.AbilityTask;
using UnityEngine;

namespace Demo.Script.MyAbilitySystem.AbilityTask
{
    public class OgATMove:OngoingAbilityTask
    {
        public override void Execute(params object[] args)
        {
            Vector3 direction = (Vector3) args[0];
            Rigidbody rigidbody = (Rigidbody) args[1];
            float speed = (float) args[2];

            var basePos = rigidbody.position;
            rigidbody.MovePosition(basePos + direction * speed * Time.deltaTime);
        }
    }
}