using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public enum DoorDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public class Door : MonoBehaviour
    {
        public bool IsOpen { get { return _isOpen; } }

        bool _isOpen;


        public DoorDirection DoorDirection = DoorDirection.Left;
        public float MovementRange = 5f;
        public float AnimDuration = 1f;
        public float OpenDelay = 0f;
        public float CloseDelay = 0f;

        Vector3 _origPos;

        Coroutine _moveCoroutine;


        void Awake()
        {
            _origPos = this.transform.position;
        }

        public void OpenDoor(float openDelay = -1f)
        {
            if (openDelay == -1f)
                openDelay = OpenDelay;

            _isOpen = true;
            switch (DoorDirection)
            {
                case DoorDirection.Up:
                    Move(_origPos + new Vector3(0, 0, MovementRange), AnimDuration, openDelay);
                    break;

                case DoorDirection.Down:
                    Move(_origPos - new Vector3(0, 0, MovementRange), AnimDuration, openDelay);
                    break;

                case DoorDirection.Left:
                    Move(_origPos - new Vector3(MovementRange, 0, 0), AnimDuration, openDelay);
                    break;
            
                case DoorDirection.Right:
                    Move(_origPos + new Vector3(MovementRange, 0, 0), AnimDuration, openDelay);
                    break;
            }
        }

        public void CloseDoor()
        {
            _isOpen = false;
            Move(_origPos, AnimDuration, CloseDelay);
        }

        void Move(Vector3 newPos, float duration, float delay)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        
            _moveCoroutine = StartCoroutine(MoveRoutine(newPos, duration, delay));
        }

        IEnumerator MoveRoutine(Vector3 newPos, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);

            var origPos = transform.position;

            var t = 0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / duration;
                transform.position = new Vector3(
                    Utils.EaseFromTo(origPos.x, newPos.x, t, EaseType.EaseOut),
                    Utils.EaseFromTo(origPos.y, newPos.y, t, EaseType.EaseOut),
                    Utils.EaseFromTo(origPos.z, newPos.z, t, EaseType.EaseOut));

                yield return null;
            }
        }
    }
}