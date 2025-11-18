using UnityEngine;

namespace Cinemachine.Examples
{
    public static class InputSystemHelper
    {
        static float s_LastMessageTime = -10;
        // Logs warning every 5 seconds
        public static void EnableBackendsWarningMessage()
        {
            if (Time.realtimeSinceStartup - s_LastMessageTime > 5)
            {
                Debug.Log(
                    "Old input backends are disabled. Cinemachine examples use Unity’s classic input system. " +
                    "To enable classic input: Edit > Project Settings > Player, " +
                    "set Active Input Handling to Both!\n" +
                    "Note: This is probably because the Input System Package has been added to the project. To use " +
                    "Cinemachine Virtual Cameras with the Input System Package, add CinemachineInputProvider component to them.");
                s_LastMessageTime = Time.realtimeSinceStartup;
            }
        }
    }
}
