using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class DoorKey : MonoBehaviour
    {
        public Door Door;
        public string PickupTag = "Player";
        public ProCamera2DCinematics Cinematics;

        void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag(PickupTag) && !Door.IsOpen)
            {
                Door.OpenDoor();

                if (Cinematics != null)
                    Cinematics.Play();

                Destroy(this.gameObject);
            }
        }
    }
}