using System;
using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EXUI
{
    public sealed class XUIManager
    {
        // UI场景
        private XUICanvasLoader _canvasLoader;
        // 运行主机
        private XUIHost _host;
        public XUIHost Host => _host;
        
        private BindingServiceBundle _bundle;
        private float _secondCount;
        private readonly Dictionary<string, ViewModelCommon> _vms;
        private readonly Dictionary<string, BaseView> _windows;
        private readonly Dictionary<string, string> _vmTypeToDefaultNameMap;
        
        /// <summary>
        ///   视图预制体路径Map
        /// </summary>
        private Dictionary<Type, string> _viewPrefabPathMap;

        private Func<string, GameObject> PrefabLoadHandle;
        
        public XUIManager()
        {
            _windows = new Dictionary<string, BaseView>();
            _vms = new Dictionary<string, ViewModelCommon>();
            _vmTypeToDefaultNameMap = new Dictionary<string, string>();
        }

        public void Init()
        {
            LaunchBindingService();
            CreateUIScene();
            _host = new GameObject("EXUIHost").AddComponent<XUIHost>();
            _host.transform.SetParent(_canvasLoader.UIRoot.transform);
            _host.Init(this);
        }
        
        public void RegisterViewPrefabPath(Dictionary<Type, string> config,Func<string, GameObject> prefabLoadHandle)
        {
            _viewPrefabPathMap = new Dictionary<Type, string>(config);
            PrefabLoadHandle = prefabLoadHandle;
        }
        
        public void LaunchBindingService()
        {
            var context = Context.GetApplicationContext();
            var container = context.GetContainer();
            _bundle = new BindingServiceBundle(container);
            _bundle.Start();
        }

        public void CreateUIScene()
        {
            _canvasLoader = new XUICanvasLoader();
            _canvasLoader.Create();
        }

        private TView CreateWindow<TView>(string name) where TView : BaseView
        {
            var path = _viewPrefabPathMap[typeof(TView)];
            var prefab = PrefabLoadHandle(path);
            var instance = Object.Instantiate(prefab, _canvasLoader.UIRoot.transform, true);
            instance.name = name;
            TView view;
            if (instance.TryGetComponent(typeof(TView),out var com))
                view = com as TView;
            else
                view = instance.AddComponent<TView>();
            view?.Init(name);
            
            return view;
        }
        
        public TView LoadWindow<TView>(string name = null) where TView : BaseView
        {
            var w = Windows<TView>(name);
            return w;
        }

        public void UnloadWindow(string name)
        {
            if (!_windows.ContainsKey(name)) return;
            _windows[name].ViewModel.OnUnload();
            _vms.Remove(name);
            
            _windows[name].DestroySelf();
            _windows.Remove(name);
        }
        
        public void UnloadWindow<TView>() where TView : BaseView
        {
            var w = typeof(TView).Name;
            UnloadWindow(w);
        }
        
        public TView OpenWindow<TView>(string name = null) where TView : BaseView
        {
            var w = LoadWindow<TView>(name);
            w.Show();
            return w;
        }

        public TViewModel VM<TViewModel>(string name = null) where TViewModel : ViewModelCommon
        {
            var viewName = name ?? _vmTypeToDefaultNameMap[typeof(TViewModel).Name];

            if (_vms.TryGetValue(viewName, value: out var vm)) 
                return vm as TViewModel;
            
            Debug.LogError($"[EXUI] View Model:{typeof(TViewModel)} has not been loaded! Please LOAD it before CALLING.");
            return null;

        }

        public TView Windows<TView>(string name=null,bool ifNullLoadIt = true) where TView : BaseView
        {
            name ??= typeof(TView).Name;
            if (!_windows.ContainsKey(name))
            {
                if (ifNullLoadIt)
                {
                    var w = CreateWindow<TView>(name);
                    _windows.Add(name, w);
                    var vm = _windows[name].ViewModel;
                    _vms.Add(name, vm);
                    _vmTypeToDefaultNameMap.Add(vm.GetType().Name,typeof(TView).Name);
                    vm.OnLoaded();
                }
                else
                {
                    return null;
                }
            }

            return _windows[name] as TView;
        }

        public void UITick()
        {
            _secondCount += Time.deltaTime;
            var isSecondUpdate = _secondCount > 1;
            if (_secondCount > 1) _secondCount = 0;
            foreach (var w in _windows.Values)
                if (w.IsShowing)
                {
                    w.ViewModel.Update_f();
                    if (isSecondUpdate) 
                        w.ViewModel.Update_s();
                }
        }

        public void OnDispose()
        {
            UnloadAllWindows();
            _bundle.Stop();
            
            Object.Destroy(_canvasLoader.UIRoot);
            _host = null;
            _canvasLoader = null;
        }

        private void UnloadAllWindows()
        {
            var names = _windows.Keys.ToList();
            foreach (var name in names)
                UnloadWindow(name);
        }
    }
}