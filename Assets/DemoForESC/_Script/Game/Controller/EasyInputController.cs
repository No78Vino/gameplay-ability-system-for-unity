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
            if(_banInput) return;
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
            if (_movement.magnitude > 0.1f)
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
            if (GuideManager.I.GuideInfo!=null
                && (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None))
                return;
            
            // if (GuideManager.I.GuideInfo!=null
            //     && (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None
            //     || GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.Run
            //     || GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.Move))
            //     return;

            // 获取原始输入（保持原始值用于动画混合）
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // 转换为基于相机的移动方向
            _cameraForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            var cameraRight = Vector3.Cross(Vector3.up, _cameraForward).normalized;
            _movement = (_cameraForward * vertical + cameraRight * horizontal).normalized;
        }

        private void HandleRun()
        {
            if (GuideManager.I.GuideInfo!=null
                && (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None
                    || GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.Run))
                return;
            
            // 奔跑开关
            if (Input.GetKeyDown(KeyCode.LeftShift))
                demoPlayer.StartRun();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                demoPlayer.StopRun();
        }

        private void HandleAttack()
        {
            if (GuideManager.I.GuideInfo!=null
                && (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None
                    || GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.MeleeAttack))
                return;
            
            // 调试GE1
            if (Input.GetKeyDown(KeyCode.E)) 
                demoPlayer.StartDebugGE1();
            else if (Input.GetKeyUp(KeyCode.E))
                demoPlayer.StopDebugGE1();
        }

        private void HandleDodge()
        {
            if (GuideManager.I.GuideInfo!=null
                && (GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.None
                    || GuideManager.I.GuideInfo.LearningKey == GuideLearningKey.Dodge))
                return;
            
            // 调试GE2
            if (Input.GetKeyDown(KeyCode.F))
                demoPlayer.StartDebugGE2();
            else if (Input.GetKeyUp(KeyCode.F))
                demoPlayer.StopDebugGE2();
        }

        private void HandleGuideClick()
        {
            if (!Input.anyKeyDown) return;
            if (!GuideManager.I.IsInGuide) return;
            var guideInfo = GuideManager.I.GuideInfo;
            if (guideInfo.LearningKey == GuideLearningKey.None)
                GuideManager.I.ContinueGuide();
        }
        #endregion
    }
}