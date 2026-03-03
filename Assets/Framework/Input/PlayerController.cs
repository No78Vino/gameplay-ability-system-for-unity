using Framework.Unit;  
using UnityEngine;  
  
namespace Framework  
{  
    /// <summary>  
    /// 玩家输入控制器。  
    /// - 捕获键盘/轴输入，调用 PlayerUnit 的语义方法  
    /// - 不直接访问 ASC，所有逻辑通过 PlayerUnit 接口分发  
    /// - SetBanInput(true) 可屏蔽全部输入（过场动画、引导步骤等）  
    /// 对应现有：EasyInputController  
    /// </summary>  
    public class PlayerController : MonoBehaviour  
    {  
        [SerializeField] private PlayerUnit _player;  
        [SerializeField] [Range(0.1f, 3f)] private float _acceleration = 0.5f;  
  
        private Camera _mainCamera;  
        private Vector3 _movement;  
        private float _currentSpeed;  
        private bool _banInput;  
  
        private void Awake()  
        {  
            _mainCamera = Camera.main;  
        }  
  
        private void Update()  
        {  
            HandleInput();  
            UpdateMovement();  
        }  
  
        // ── 外部控制接口 ──  
  
        /// <summary>屏蔽/恢复输入（过场动画、UI 弹出时调用）</summary>  
        public void SetBanInput(bool ban)  
        {  
            _banInput = ban;  
            if (ban) _movement = Vector3.zero;  
        }  
  
        /// <summary>供动画系统查询当前移动向量</summary>  
        public Vector3 GetMovementVector() => _movement;  
  
        public bool IsMoving()  => _movement.magnitude > 0.1f;  
  
        // ── 内部输入处理 ──  
  
        private void HandleInput()  
        {  
            if (_banInput)  
            {  
                _movement = Vector3.zero;  
                return;  
            }  
            HandleMove();  
            HandleRun();  
            HandleAttack();  
        }  
  
        private void UpdateMovement()  
        {  
            if (_player == null) return;  
  
            // 速度插值（对应 EasyInputController._currentSpeed Lerp）  
            _currentSpeed = Mathf.Lerp(_currentSpeed, _player.GetSpeed(), _acceleration * Time.deltaTime);  
  
            var h = Mathf.Abs(Input.GetAxis("Horizontal"));  
            var v = Mathf.Abs(Input.GetAxis("Vertical"));  
  
            if (h + v > 0.2f)  
                _player.Move(_movement);  
            else  
                _player.StopMove();  
        }  
  
        private void HandleMove()  
        {  
            // 以摄像机方向为基准计算移动向量（与 EasyInputController.HandleMove 一致）  
            var horizontal = Input.GetAxis("Horizontal");  
            var vertical   = Input.GetAxis("Vertical");  
            var camFwd  = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;  
            var camRight = Vector3.Cross(Vector3.up, camFwd).normalized;  
            _movement = (camFwd * vertical + camRight * horizontal).normalized;  
        }  
  
        private void HandleRun()  
        {  
            if (Input.GetKeyDown(KeyCode.LeftShift))  
                _player.StartRun();  
            else if (Input.GetKeyUp(KeyCode.LeftShift))  
                _player.StopRun();  
        }  
  
        private void HandleAttack()  
        {  
            if (Input.GetKeyDown(KeyCode.E))  
                _player.Attack();  
        }  
    }  
}