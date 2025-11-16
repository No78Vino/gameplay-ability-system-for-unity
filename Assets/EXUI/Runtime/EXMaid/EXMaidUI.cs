using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI.Extension;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace EXMaidForUI.Runtime.EXMaid
{
    public interface IEXMaidUI
    {
        void UITick();

        void OnDispose();

        void LaunchBindingService(string prefix,FairyGUIPackageExtension.OnLoadResource onLoadResourceHandler);

        T LoadWindow<T>() where T : BaseWindow;

        void UnloadWindow<T>() where T : BaseWindow;

        T OpenWindow<T>() where T : BaseWindow;

        T VM<T>() where T : ViewModelCommon;

        BaseWindow Windows(Type type);

        BaseWindow WindowsWithoutLoad(Type type);

        void AddWorldSpaceUI(GObject obj);

        void RefreshSceneUICanvas(float cameraSize);
    }

    public sealed class EXMaidUI : IEXMaidUI
    {
        private BindingServiceBundle _bundle;
        private FairyGUIBindingServiceBundle _fairyGUIBindingServiceBundle;
        private float _secondCount;
        private readonly Dictionary<Type, ViewModelCommon> _vms;
        private readonly Dictionary<Type, BaseWindow> _windows;
        private GComponent _worldSpaceUICanvas;
        private Window _worldSpaceUIWindow;

        public EXMaidUI()
        {
            _windows = new Dictionary<Type, BaseWindow>();
            _vms = new Dictionary<Type, ViewModelCommon>();
            CreateWorldSpaceUICanvas();
        }

        public void LaunchBindingService(string prefix,FairyGUIPackageExtension.OnLoadResource onLoadResourceHandler = null)
        {
            var context = Context.GetApplicationContext();
            var container = context.GetContainer();
            /* Initialize the data binding service */
            _bundle = new BindingServiceBundle(container);
            _bundle.Start();

            _fairyGUIBindingServiceBundle = new FairyGUIBindingServiceBundle(container);
            _fairyGUIBindingServiceBundle.Start();

            FairyGUIPackageExtension.RegisterOnLoadResourceHandler(onLoadResourceHandler);
            FairyGUIPackageExtension.InitFileNamePrefix(prefix);
        }

        public T LoadWindow<T>() where T : BaseWindow
        {
            var t = typeof(T);
            var w = Windows(t);
            return w as T;
        }

        public void UnloadWindow<T>() where T : BaseWindow
        {
            var w = typeof(T);
            if (!_windows.ContainsKey(w)) return;
            _windows[w].VM.OnUnload();
            _vms.Remove(_windows[w].VM.GetType());

            _windows[w].OnDispose();
            _windows[w].Dispose();
            _windows.Remove(w);
        }

        public T OpenWindow<T>() where T : BaseWindow
        {
            var w = LoadWindow<T>();
            w.Show();
            return w;
        }

        public T VM<T>() where T : ViewModelCommon
        {
            var t = typeof(T);
            if (!_vms.ContainsKey(t))
            {
                Debug.LogWarning($"[EXMaid] View Model:{t} has not been loaded! Please LOAD it before CALLING.");
                return null;
            }

            var vm = _vms[t];
            return vm as T;
        }

        public BaseWindow Windows(Type type)
        {
            if (!_windows.ContainsKey(type))
            {
                _windows.Add(type, Activator.CreateInstance(type) as BaseWindow);
                var vm = _windows[type].VM;
                _vms.Add(vm.GetType(), vm);
                vm.OnLoaded();
            }

            return _windows[type];
        }

        public BaseWindow WindowsWithoutLoad(Type type)
        {
            if (!_windows.ContainsKey(type)) return null;
            return _windows[type];
        }

        public void AddWorldSpaceUI(GObject obj)
        {
            _worldSpaceUICanvas.AddChild(obj);
        }

        /// <summary>
        ///     世界UI画布位置更新
        /// </summary>
        public void RefreshSceneUICanvas(float cameraSize)
        {
            //缩放
            var uiScale = cameraSize / Camera.main.orthographicSize;
            _worldSpaceUICanvas.SetScale(uiScale, uiScale);
            // 移动
            var gRoot = GRoot.inst;
            var size = gRoot.viewHeight * 0.5f / Camera.main.orthographicSize;
            var localPosition = Camera.main.transform.localPosition;
            _worldSpaceUICanvas.x = gRoot.viewWidth * 0.5f - localPosition.x * size;
            _worldSpaceUICanvas.y = gRoot.viewHeight * 0.5f + localPosition.y * size;
        }

        public void UITick()
        {
            _secondCount += Time.deltaTime;
            var isSecondUpdate = _secondCount > 1;
            if (_secondCount > 1) _secondCount = 0;
            foreach (var w in _windows.Values)
                if (w.isShowing)
                {
                    w.VM.Update_f();
                    if (isSecondUpdate) w.VM.Update_s();
                }
        }

        public void OnDispose()
        {
            UnloadAllWindows();
            _worldSpaceUIWindow.Dispose();

            _bundle.Stop();
            _fairyGUIBindingServiceBundle.Stop();
        }

        ///创建地图UI画布
        private void CreateWorldSpaceUICanvas()
        {
            _worldSpaceUIWindow = new Window();
            _worldSpaceUIWindow.contentPane = new GComponent();
            _worldSpaceUIWindow.contentPane.touchable = true;
            _worldSpaceUIWindow.bringToFontOnClick = false;
            _worldSpaceUIWindow.MakeFullScreen();
            _worldSpaceUIWindow.Show();

            _worldSpaceUICanvas = new GComponent();
            _worldSpaceUICanvas.touchable = true;
            _worldSpaceUICanvas.name = "World FairyGUI Canvas";
            _worldSpaceUIWindow.contentPane.AddChild(_worldSpaceUICanvas);
        }

        ///世界坐标系 -> 地图UI画布坐标系
        public static Vector3 WorldSpaceToUISpace(Vector3 worldPos, float cameraSize)
        {
            var gRoot = GRoot.inst;
            var size = gRoot.viewHeight * 0.5f / cameraSize;
            return new Vector3(worldPos.x * size + gRoot.x, -worldPos.y * size - gRoot.y, 0);
        }

        private void UnloadAllWindows()
        {
            var listCopy = _windows.Values.ToList();
            foreach (var win in listCopy)
            {
                var w = win.GetType();
                if (!_windows.ContainsKey(w)) return;
                _windows[w].VM.OnUnload();
                _vms.Remove(_windows[w].VM.GetType());

                _windows[w].OnDispose();
                _windows[w].Dispose();
                _windows.Remove(w);
            }
        }
    }
}