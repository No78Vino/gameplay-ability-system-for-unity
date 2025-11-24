using UnityEngine;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{
    // Moving the demo VR character controller.
    public class VRController : MonoBehaviour
    {

        [System.Serializable]
        public enum InputMode
        {
            Input = 0,
            WASDOnly = 1,
        }

        public InputMode inputMode;
        public VRIK ik;
        public Transform centerEyeAnchor;

        // Match these values to velocities in the locomotion animation blend tree for better looking results (avoids half-blends)
        public float walkSpeed = 1f;
        public float runSpeed = 3f;
        public float walkForwardSpeedMlp = 1f;
        public float runForwardSpeedMlp = 1f;

        private Vector3 smoothInput, smoothInputV;

        private void Update()
        {
            // Get input
            Vector3 input = GetInput();
            input *= ik.solver.scale;

            float fDot = Vector3.Dot(input, Vector3.forward);
            bool f = fDot > 0f;

            // Locomotion speed
            float s = walkSpeed;
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                s = runSpeed;
                if (f) s *= runForwardSpeedMlp; // Walk faster/slower when moving forward
            } else
            {
                if (f) s *= walkForwardSpeedMlp; // Run faster/slower when moving forward
            }

            // Input smoothing
            smoothInput = Vector3.SmoothDamp(smoothInput, input * s, ref smoothInputV, 0.1f);

            // Rotate input to avatar space
            Vector3 forward = centerEyeAnchor.forward;
            forward.y = 0f;
            Quaternion avatarSpace = Quaternion.LookRotation(forward);

            // Apply
            transform.position += avatarSpace * smoothInput * Time.deltaTime;
        }

        // Returns keyboard/thumbstick input vector
        private Vector3 GetInput()
        {
            switch (inputMode)
            {
                case InputMode.Input:
                    Vector3 v = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
                    if (v.sqrMagnitude < 0.3f) return Vector3.zero;
                    return v.normalized;
                case InputMode.WASDOnly:
                    Vector3 input = Vector3.zero;
                    if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
                    if (Input.GetKey(KeyCode.S)) input += Vector3.back;
                    if (Input.GetKey(KeyCode.A)) input += Vector3.left;
                    if (Input.GetKey(KeyCode.D)) input += Vector3.right;
                    return input.normalized;
                default: return Vector3.zero;
            }
        }
    }
}
