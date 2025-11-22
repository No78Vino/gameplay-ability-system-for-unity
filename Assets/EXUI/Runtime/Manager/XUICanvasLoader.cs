using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EXUI
{
    public class XUICanvasLoader
    {
        private GameObject _uiRoot;
        public GameObject UIRoot => _uiRoot;
        private GameObject _uiCanvasObj;
        private Canvas _uiCanvas;
        private GameObject _eventSystemObj;
        private EventSystem _eventSystem;
        private GraphicRaycaster _graphicRaycaster;
        
        public void Create()
        {
            _uiRoot = new GameObject("UIRoot");
            Object.DontDestroyOnLoad(_uiRoot);
            // 生成UGUI核心组件（Canvas + EventSystem）
            CreateCanvas();
            CreateEventSystem();
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("UICanvas");
            canvasObj.transform.SetParent(_uiRoot.transform);
            _uiCanvasObj = canvasObj;
            _uiCanvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            _graphicRaycaster = canvasObj.AddComponent<GraphicRaycaster>();

            // 设置canvasObj 不随场景切换而销毁
            Object.DontDestroyOnLoad(canvasObj);

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObj.GetComponent<CanvasScaler>();

            // 配置Canvas核心参数（适合全屏菜单场景）
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // 屏幕覆盖模式（无需相机）
            canvas.sortingOrder = 100; // UI层级（确保在其他UI之上）

            // 配置CanvasScaler（屏幕适配关键）
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(2340, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f; // 宽高自适应平衡
            canvasScaler.scaleFactor = 1f;
        }

        /// <summary>
        /// 代码生成EventSystem（UI交互必备）
        /// </summary>
        private void CreateEventSystem()
        {
            GameObject eventSystemObj = new GameObject("UIEventSystem");
            eventSystemObj.transform.SetParent(_uiRoot.transform);
            _eventSystemObj = eventSystemObj;
            // 添加必备组件（无这些组件，按钮、滑动条等交互失效）
            _eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>(); // 桌面端输入模块

            // 设置 eventSystemObj 不随场景切换而销毁
            Object.DontDestroyOnLoad(eventSystemObj);

            // 配置输入模块参数（可选）
            StandaloneInputModule inputModule = eventSystemObj.GetComponent<StandaloneInputModule>();
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
        }
    }
}