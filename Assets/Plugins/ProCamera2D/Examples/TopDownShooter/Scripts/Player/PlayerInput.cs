using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerInput : MonoBehaviour
    {
        public float RunSpeed = 12;
        public float Acceleration = 30;

        float _currentSpeedH;
        float _currentSpeedV;
        Vector3 _amountToMove;
        int _totalJumps;

        CharacterController _characterController;

        bool _movementAllowed = true;

        void Start()
        {
            _characterController = GetComponent<CharacterController>();

            var cinematics = FindObjectsOfType<ProCamera2DCinematics>();
            for (int i = 0; i < cinematics.Length; i++)
            {
                cinematics[i].OnCinematicStarted.AddListener(() =>
                    {
                        _movementAllowed = false; 
                        _currentSpeedH = 0;
                        _currentSpeedV = 0;
                    });

                cinematics[i].OnCinematicFinished.AddListener(() =>
                    {
                        _movementAllowed = true; 
                    });
            }
        }

        void Update()
        {
            if (!_movementAllowed)
                return;

            var targetSpeedH = Input.GetAxis("Horizontal") * RunSpeed;
            _currentSpeedH = IncrementTowards(_currentSpeedH, targetSpeedH, Acceleration);

            var targetSpeedV = Input.GetAxis("Vertical") * RunSpeed;
            _currentSpeedV = IncrementTowards(_currentSpeedV, targetSpeedV, Acceleration);

            _amountToMove.x = _currentSpeedH;
            _amountToMove.z = _currentSpeedV;

            _characterController.Move(_amountToMove * Time.deltaTime);
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