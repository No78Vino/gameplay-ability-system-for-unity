using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     蜘蛛模板调试驱动器：挂到 Spider 根节点后自动驱动其行走，
    ///     便于在 Scene 中直接观察程序化动画效果，无需手动移动。
    ///     <para>控制：W/S 前进后退，A/D 转向（绕 Y 轴），Space 暂停。</para>
    /// </summary>
    [AddComponentMenu("EXProceduralMachine/Debug/SpiderWalkDriver")]
    public class SpiderWalkDriver : MonoBehaviour
    {
        [Tooltip("前进速度 (m/s)")]
        public float moveSpeed = 1.2f;

        [Tooltip("转向速度 (度/秒)")]
        public float turnSpeed = 90f;

        [Tooltip("按 W/S 手动控制时是否同时转向输入")]
        public bool allowManualControl = true;

        [Tooltip("未按键时是否自动前进（方便直接观察）")]
        public bool autoWalk = true;

        private bool _paused;
        private float _runtimeDistance;

        public float RuntimeDistance => _runtimeDistance;
        public bool IsPaused => _paused;

        private void Update()
        {
            // 空格暂停/继续
            if (Input.GetKeyDown(KeyCode.Space))
                _paused = !_paused;

            if (_paused)
                return;

            var move = 0f;
            if (allowManualControl)
            {
                if (Input.GetKey(KeyCode.W)) move += 1f;
                if (Input.GetKey(KeyCode.S)) move -= 1f;
            }

            if (move == 0f && autoWalk)
                move = 1f;

            if (move != 0f)
            {
                transform.Translate(Vector3.forward * (move * moveSpeed * Time.deltaTime));
                _runtimeDistance += Mathf.Abs(move) * moveSpeed * Time.deltaTime;
            }

            if (allowManualControl)
            {
                var turn = 0f;
                if (Input.GetKey(KeyCode.A)) turn -= 1f;
                if (Input.GetKey(KeyCode.D)) turn += 1f;
                if (turn != 0f)
                    transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
            }
        }
    }
}