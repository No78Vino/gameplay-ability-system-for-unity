using System.Collections.Generic;

using UnityEngine;

namespace EXToyLib
{
    public enum GroundDetectionMethod
    {
        Default, // 使用 CharacterController.isGrounded
        SphereCheck
    }

    public class GravityForCharacterController
    {
        private static GravityForCharacterController _instance;

        private readonly Dictionary<CharacterController, float> _gravityRate = new();

        private bool _enabled = true; // 是否启用重力
        private float _gravity = -9f; // 地球重力约 -9.81 m/s², 可调整

        private GroundDetectionMethod
            _groundDetectionMethod = GroundDetectionMethod.Default; // 默认使用 CharacterController.isGrounded

        private float _groundDistance = 0.4f; // 检测距离
        private LayerMask _groundMask = LayerMask.GetMask("Default"); // 指定哪些层代表地面
        public static GravityForCharacterController Instance => _instance ??= new GravityForCharacterController();

        private GravityForCharacterControllerHost _host;
        
        private GravityForCharacterController()
        {
            _host = Object.FindObjectOfType<GravityForCharacterControllerHost>();
            if (_host != null) return;
            var go = new GameObject("GravityForCharacterControllerHost");
            Object.DontDestroyOnLoad(go);
            _host = go.AddComponent<GravityForCharacterControllerHost>();
        }
        
        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        public void SetGravity(float gravity)
        {
            _gravity = gravity;
        }

        public float GetGravity()
        {
            return _gravity;
        }

        public void SetGroundDetectionMethod(GroundDetectionMethod method)
        {
            _groundDetectionMethod = method;
        }

        public GroundDetectionMethod GetGroundDetectionMethod()
        {
            return _groundDetectionMethod;
        }

        public void SetGroundDistance(float distance)
        {
            _groundDistance = distance;
        }

        public void SetGroundMask(LayerMask mask)
        {
            _groundMask = mask;
        }

        public void Register(CharacterController controller, float rate = 1f)
        {
            if (_gravityRate.ContainsKey(controller))
                _gravityRate[controller] = rate;
            else
                _gravityRate.Add(controller, rate);
        }

        public void Unregister(CharacterController controller)
        {
            if (_gravityRate.ContainsKey(controller)) _gravityRate.Remove(controller);
        }

        public void UpdateGravity()
        {
            if (!_enabled) return;
            
            foreach (var kvp in _gravityRate)
            {
                var controller = kvp.Key;
                var rate = kvp.Value;

                if (_groundDetectionMethod == GroundDetectionMethod.Default)
                {
                    if (controller != null && controller.isGrounded == false)
                    {
                        // 1. 检测地面 - 在角色脚底位置创建一个小的球形检测区域
                        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                        var isGrounded = controller.isGrounded;

                        // 2. 重置落地速度（确保角色站稳，防止微小弹跳）
                        var velocity = controller.velocity;
                        if (isGrounded && velocity.y < 0) velocity.y = -2f; // 一个小的负值，比0更好，确保角色紧贴地面

                        // 3. 应用重力 (无论是否跳跃，只要不在地面就持续加速下落)
                        velocity.y += _gravity * rate * Time.fixedDeltaTime;

                        // 4. 应用垂直速度 (重力或跳跃)
                        controller.Move(velocity * Time.fixedDeltaTime); // 注意：这里再次调用Move，应用Y轴速度
                    }
                }
                else if (_groundDetectionMethod == GroundDetectionMethod.SphereCheck)
                {
                    // 1. 检测地面 - 在角色脚底位置创建一个小的球形检测区域
                    var position = controller.transform.position +
                                   Vector3.down * (controller.height / 2 + _groundDistance);
                    var isGrounded = Physics.CheckSphere(position, _groundDistance, _groundMask);
                    if (controller != null && !isGrounded)
                    {
                        // 2. 重置落地速度（确保角色站稳，防止微小弹跳）
                        var velocity = controller.velocity;

                        // 3. 应用重力 (无论是否跳跃，只要不在地面就持续加速下落)
                        velocity.y += _gravity * rate * Time.fixedDeltaTime;

                        // 4. 应用垂直速度 (重力或跳跃)
                        controller.Move(velocity * Time.fixedDeltaTime); // 注意：这里再次调用Move，应用Y轴速度
                    }
                }
            }
        }
    }
}