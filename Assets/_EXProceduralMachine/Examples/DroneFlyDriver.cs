using Sirenix.OdinInspector;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    /// 飞行无人机逻辑驱动。
    /// 职责：目标运动生成（巡航/手动、转向、升降）、空气阻力衰减、Banking 倾斜姿态、
    /// 悬停高度保持、Scene 调试 Gizmos。
    /// 视觉层（Body 上的 SecondOrderDynamicsComponent）通过替身滞后跟随，产生惯性/回弹。
    /// </summary>
    public class DroneFlyDriver : MonoBehaviour
    {
        [Title("飞行控制")]
        [LabelText("自动巡航")]
        [Tooltip("开启后无人机自动环形巡航并演示次级运动；关闭后 WASD/Space 手动控制")]
        public bool autoFly = true;

        [LabelText("巡航速度")]
        public float cruiseSpeed = 2.5f;

        [LabelText("巡航转向(度/秒)")]
        public float autoTurnRate = 30f;

        [LabelText("手动速度")]
        public float moveSpeed = 3f;

        [LabelText("手动转向(度/秒)")]
        public float turnSpeed = 120f;

        [LabelText("升降速度")]
        public float verticalSpeed = 1.5f;

        [Title("空气阻力")]
        [LabelText("阻力系数(1/s)")]
        [Tooltip("越大速度越快趋近目标速度/越早停下，模拟空气摩擦阻力")]
        public float drag = 1.8f;

        [Title("悬停")]
        [LabelText("悬停高度")]
        public float hoverHeight = 2f;

        [LabelText("高度恢复速度")]
        public float heightRestoreSpeed = 2f;

        [LabelText("启用漂浮浮动")]
        [Tooltip("悬停漂浮时是否上下轻微起伏")]
        public bool hoverBobOn = true;

        [LabelText("漂浮浮动幅度(米)")]
        [Tooltip("悬停漂浮时上下轻微浮动的幅度（直接叠加在高度上）")]
        public float hoverBobAmplitude = 0.08f;

        [LabelText("漂浮浮动频率(Hz)")]
        [Tooltip("悬停漂浮时上下浮动的频率")]
        public float hoverBobFrequency = 1.2f;

        [Title("Banking 姿态")]
        [LabelText("前倾幅度(度)")]
        public float bankPitch = 18f;

        [LabelText("侧倾幅度(度)")]
        public float bankRoll = 14f;

        [LabelText("离心侧倾权重")]
        [Tooltip("转向越急，额外侧倾越明显（压弯效果）")]
        public float turnBankWeight = 0.6f;

        [Title("替身")]
        [LabelText("姿态替身(隐藏子物体)")]
        [Tooltip("每帧写入期望欧拉角(pitch,0,roll)，由 Body 的次级运动实例跟随")]
        public Transform attitude;

        [Title("调试")]
        [LabelText("绘制 Gizmos")]
        public bool gizmosOn = true;

        /// <summary>当前实测速度（含空气阻力后的实际速度）</summary>
        public Vector3 Velocity => _velocity;

        private Vector3 _velocity;
        private float _yaw;
        private float _heightTarget;
        private float _yawRate;

        private void Awake()
        {
            _heightTarget = hoverHeight;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // 1. 目标速度（速度指令）
            Vector3 desiredVelocity = Vector3.zero;
            _yawRate = 0f;

            if (autoFly)
            {
                // 环形巡航：持续转向 + 轻微速度脉冲（制造加速/减速，突出惯性回弹表现）
                float pulse = 1f + Mathf.Sin(Time.time * 1.2f) * 0.2f;
                desiredVelocity = transform.forward * (cruiseSpeed * pulse);
                _yawRate = autoTurnRate;
            }
            else
            {
                if (Input.GetKey(KeyCode.W)) desiredVelocity += transform.forward * moveSpeed;
                if (Input.GetKey(KeyCode.S)) desiredVelocity -= transform.forward * moveSpeed;
                if (Input.GetKey(KeyCode.A)) _yawRate = -turnSpeed;
                if (Input.GetKey(KeyCode.D)) _yawRate = turnSpeed;
                if (Input.GetKey(KeyCode.Space)) _heightTarget += verticalSpeed * dt;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C)) _heightTarget -= verticalSpeed * dt;
                _heightTarget = Mathf.Clamp(_heightTarget, 0.2f, 50f);
            }

            // 2. 空气阻力：速度以指数速率趋近目标速度（阻力越大越"粘稠"，撤油门后滑行减速）
            float approach = 1f - Mathf.Exp(-drag * dt);
            _velocity = Vector3.Lerp(_velocity, desiredVelocity, approach);

            // 3. 位置积分
            Vector3 newPos = transform.position + _velocity * dt;

            // 4. 高度保持（柔和恢复悬停高度）+ 漂浮浮动（直接叠加在高度上，仅经二级系统柔化）
            float bob = 0f;
            if (hoverBobOn)
                bob = Mathf.Sin(Time.time * hoverBobFrequency * Mathf.PI * 2f) * hoverBobAmplitude;
            newPos.y = Mathf.Lerp(newPos.y, _heightTarget, 1f - Mathf.Exp(-heightRestoreSpeed * dt)) + bob;
            transform.position = newPos;

            // 5. 偏航
            _yaw += _yawRate * dt;
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

            // 6. Banking 目标姿态（相对 root 的局部欧拉角）
            if (attitude != null)
            {
                float fwd = Vector3.Dot(_velocity, transform.forward);
                float side = Vector3.Dot(_velocity, transform.right);
                float norm = Mathf.Max(moveSpeed, 0.1f);

                // 前进低头 / 后退抬头
                float pitch = -Mathf.Clamp(fwd / norm, -1f, 1f) * bankPitch;
                // 侧移侧倾 + 转向离心侧倾（压弯）
                float roll = Mathf.Clamp(side / norm, -1f, 1f) * bankRoll;
                roll += Mathf.Clamp(_yawRate / 120f, -1f, 1f) * bankRoll * turnBankWeight;

                attitude.localEulerAngles = new Vector3(pitch, 0f, roll);
            }
        }

        private void OnDrawGizmos()
        {
            if (!gizmosOn) return;

            // 逻辑位置（root）
            Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.9f);
            Gizmos.DrawWireCube(transform.position, new Vector3(0.4f, 0.4f, 0.4f));

            // 速度向量
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.9f);
            Gizmos.DrawLine(transform.position, transform.position + _velocity);

            // 目标高度（含漂浮浮动）
            Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
            float bob = hoverBobOn ? Mathf.Sin(Time.time * hoverBobFrequency * Mathf.PI * 2f) * hoverBobAmplitude : 0f;
            Vector3 hp = new Vector3(transform.position.x, _heightTarget + bob, transform.position.z);
            Gizmos.DrawLine(transform.position, hp);
            Gizmos.DrawWireSphere(hp, 0.15f);

            // 姿态方向
            if (attitude != null)
            {
                Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.9f);
                Gizmos.DrawLine(attitude.position, attitude.position + attitude.forward * 0.8f);
            }
        }
    }
}
