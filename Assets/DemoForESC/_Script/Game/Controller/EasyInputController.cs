using UnityEngine;

namespace DemoForESC._Script.Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class EasyInputController : MonoBehaviour
    {
        private static EasyInputController _inst;
        public static EasyInputController Inst()
        {
            if (_inst == null)
                _inst = FindObjectOfType<EasyInputController>();
            
            return _inst;
        }
        
        private float Speed => demoPlayer.GetSpeed(); // 行走速度

        [SerializeField] [Range(0.1f, 3f)] private float acceleration = 0.5f; // 加速时间
        
        [SerializeField]
        private DemoPlayer demoPlayer;


        private Vector3 _cameraForward;
        
        private float _currentSpeed;
        private bool _isRunning;
        private UnityEngine.Camera _mainCamera;
        private Vector3 _movement;
        private bool _banInput;

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            HandleInput();
            UpdateMovement();
        }

        private void HandleInput()
        {
            if (_banInput)
            {
                _movement = Vector3.zero;
                return;
            }
            HandleGuideClick();
            HandleMove();
            HandleRun();
            HandleAttack();
            HandleDodge();
        }

        private void UpdateMovement()
        {
            // 速度控制
            _currentSpeed = Mathf.Lerp(_currentSpeed, Speed, acceleration * Time.deltaTime);

            // 应用移动
            var horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
            var vertical = Mathf.Abs(Input.GetAxis("Vertical"));
            if (horizontal+vertical > 0.2f)
            {
                if (demoPlayer != null)
                {
                    demoPlayer.Move(_movement);
                }
            }
            else
            {
                if (demoPlayer != null)
                {
                    demoPlayer.StopMove();
                }
            }
            
        }

        // 供动画系统调用的参数
        public Vector3 GetMovementVector()
        {
            return _movement;
        }

        public bool IsMoving()
        {
            return _movement.magnitude > 0.1f;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public void SetBanInput(bool ban)
        {
            _banInput = ban;
        }
        
        #region InputHandle

        private void HandleMove()  
        {  
            var horizontal = Input.GetAxis("Horizontal");  
            var vertical = Input.GetAxis("Vertical");  
            _cameraForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;  
            var cameraRight = Vector3.Cross(Vector3.up, _cameraForward).normalized;  
            _movement = (_cameraForward * vertical + cameraRight * horizontal).normalized;  
        }  
  
        private void HandleRun()  
        {  
            if (Input.GetKeyDown(KeyCode.LeftShift))  
                demoPlayer.StartRun();  
            else if (Input.GetKeyUp(KeyCode.LeftShift))  
                demoPlayer.StopRun();  
        }  
  
        private void HandleAttack()  
        {  
            if (Input.GetKeyDown(KeyCode.E))  
                demoPlayer.Attack();  
        }  
  
        private void HandleDodge()  
        {  
            if (Input.GetKeyDown(KeyCode.F))  
                demoPlayer.Dodge();  
        }  
  
        private void HandleGuideClick()  
        {  
            if (!Input.anyKeyDown) return;  
            if (!GuideManager.I.IsInGuide) return;  
            // 只有纯文字步骤（None）才响应任意键推进引导  
            if (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None)  
                GuideManager.I.ContinueGuide();  
        }
        #endregion
    }
}