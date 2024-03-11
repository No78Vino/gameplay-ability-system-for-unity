using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.Platformer
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInputBot : MonoBehaviour
    {
        public Transform Body;

        // Player Handling
        public float gravity = 20;
        public float runSpeed = 12;
        public float acceleration = 30;
        public float jumpHeight = 12;
        public int jumpsAllowed = 2;

        private float currentSpeed;
        private Vector3 amountToMove;
        int totalJumps;

        bool _fakeInputJump;
        float _fakeInputHorizontalAxis;

        CharacterController _characterController;

        void Start()
        {
            _characterController = GetComponent<CharacterController>();

            StartCoroutine(RandomInputJump());
            StartCoroutine(RandomInputSpeed());
        }

        IEnumerator RandomInputJump()
        {
            var waitForEndOfFrame = new WaitForEndOfFrame();
            while (true)
            {
                _fakeInputJump = true;
                yield return waitForEndOfFrame;
                yield return waitForEndOfFrame;
                _fakeInputJump = false;
                yield return new WaitForSeconds(Random.Range(.2f, 1f));
            }
        }

        IEnumerator RandomInputSpeed()
        {
            while (true)
            {
                _fakeInputHorizontalAxis = Random.Range(-1f, 1f);
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
        }

        void Update()
        {
            // Reset acceleration upon collision
            if ((_characterController.collisionFlags & CollisionFlags.Sides) != 0)
            {
                currentSpeed = 0;
            }
		
            // If player is touching the ground
            if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
            {
                amountToMove.y = -1f;
                totalJumps = 0;
            }
            else
            {
                amountToMove.y -= gravity * Time.deltaTime;
            }

            // Jump
            if (_fakeInputJump && totalJumps < jumpsAllowed)
            {
                totalJumps++;
                amountToMove.y = jumpHeight;	
            }
		
            // Input
            var targetSpeed = _fakeInputHorizontalAxis * runSpeed;
            currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);

            // Reset z
            if(transform.position.z != 0)
            {
                amountToMove.z = -transform.position.z;
            }
		
            // Set amount to move
            amountToMove.x = currentSpeed;

            if(amountToMove.x != 0)
                Body.localScale = new Vector2(Mathf.Sign(amountToMove.x), 1);

            _characterController.Move(amountToMove * Time.deltaTime);
        }
	
        // Increase n towards target by speed
        private float IncrementTowards(float n, float target, float a)
        {
            if (n == target)
            {
                return n;	
            }
            else
            {
                float dir = Mathf.Sign(target - n); 
                n += a * Time.deltaTime * dir;
                return (dir == Mathf.Sign(target - n)) ? n : target;
            }
        }
    }
}
