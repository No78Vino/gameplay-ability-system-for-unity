using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

        private Func<string, GameObject> _prefabLoadHandle;
        
        private Func<string, Task<GameObject>> _asyncPrefabLoadHandle;  
        
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
        
        
        
        public void RegisterViewPrefabPath(  
            Dictionary<Type, string> config,  
            Func<string, GameObject> syncLoadHandle,  
            Func<string, Task<GameObject>> asyncLoadHandle = null)  // ✅ 可选异步版  
        {  
            _viewPrefabPathMap = new Dictionary<Type, string>(config);  
            _prefabLoadHandle = syncLoadHandle;  
            _asyncPrefabLoadHandle = asyncLoadHandle;  
        }  

        private void LaunchBindingService()
        {
            var context = Context.GetApplicationContext();
            var container = context.GetContainer();
            _bundle = new BindingServiceBundle(container);
            _bundle.Start();
        }

        private void CreateUIScene()
        {
            _canvasLoader = new XUICanvasLoader();
            _canvasLoader.Create();
        }

        private TView CreateWindow<TView>(string name) where TView : BaseView  
        {  
            // 1. 路径查找（不变）  
            if (!_viewPrefabPathMap.TryGetValue(typeof(TView), out var path))  
                throw new InvalidOperationException($"[EXUI] View type {typeof(TView).Name} is not registered in ViewPrefabPathMap.");  
      
            // 2. 加载 Prefab（不变）  
            var prefab = _prefabLoadHandle(path);  
            if (prefab == null)  
                throw new InvalidOperationException($"[EXUI] Failed to load prefab at path: {path}");  
  
            // 3. ✅ 先实例化一个临时对象，拿到 View 组件，读取其 Layer 属性  
            //    注意：这里先实例化到 UIRoot，稍后再 SetParent 到正确层级  
            var instance = Object.Instantiate(prefab);  
            instance.name = name;  
  
            TView view;  
            if (instance.TryGetComponent(typeof(TView), out var com))  
                view = com as TView;  
            else  
                view = instance.AddComponent<TView>();  
  
            // 4. ✅ 根据 view.Layer 获取对应层级容器，再重新设置父级  
            var layerRoot = _canvasLoader.GetLayerRoot(view.Layer);  
            instance.transform.SetParent(layerRoot.transform, false);
  
            // 5. 初始化 View（Init 内部会依次调用 CreateVM → InitViewComponents → BindData）  
            view.Init(name);  
  
            return view;  
        }
        
        private TView CreateWindow<TView, TViewModel>(string name, TViewModel vm)  
            where TView : BaseView<TViewModel>  
            where TViewModel : ViewModelCommon  
        {  
            var path = _viewPrefabPathMap[typeof(TView)];  
            var prefab = _prefabLoadHandle(path);  
            var instance = Object.Instantiate(prefab, _canvasLoader.UIRoot.transform, true);  
            instance.name = name;  
            TView view = instance.TryGetComponent(out TView existing) ? existing : instance.AddComponent<TView>();  
            view.InitWithViewModel(name, vm);  // ✅ 注入外部 VM  
            return view;  
        }
        
        // 新增异步创建窗口  
        private async Task<TView> CreateWindowAsync<TView>(string name) where TView : BaseView  
        {  
            if (_asyncPrefabLoadHandle == null)  
                throw new InvalidOperationException("[EXUI] Async load handle is not registered.");  
  
            var path = _viewPrefabPathMap[typeof(TView)];  
            var prefab = await _asyncPrefabLoadHandle(path);               // ✅ 异步加载，不阻塞主线程  
            if (prefab == null)  
                throw new InvalidOperationException($"[EXUI] Failed to async load prefab: {path}");  
  
            var instance = Object.Instantiate(prefab);  
            instance.name = name;  
            TView view = instance.TryGetComponent(out TView existing) ? existing : instance.AddComponent<TView>();  
            var layerRoot = _canvasLoader.GetLayerRoot(view.Layer);  
            instance.transform.SetParent(layerRoot.transform, false);  
            view.Init(name);  
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
            
            // ✅ 如果是全屏窗口，隐藏同层中其他已显示的窗口  
            if (w.IsFullScreen)  
            {  
                foreach (var other in _windows.Values)  
                    if (other != w && other.IsShowing && other.Layer == w.Layer)  
                        other.Hide();  
            }  
            
            w.Show();
            return w;
        }

        public async Task<TView> OpenWindowAsync<TView>(string name = null) where TView : BaseView  
        {  
            name ??= typeof(TView).Name;  
            if (!_windows.ContainsKey(name))  
            {  
                var w = await CreateWindowAsync<TView>(name);  
                _windows.Add(name, w);  
                var vm = w.ViewModel;  
                _vms.Add(name, vm);  
                _vmTypeToDefaultNameMap.Add(vm.GetType().Name, typeof(TView).Name);  
                vm.OnLoaded();  
            }  
            var window = _windows[name] as TView;  
            window?.Show();  
            return window;  
        }
        
        public TViewModel VM<TViewModel>(string name = null) where TViewModel : ViewModelCommon  
        {  
            if (!_vmTypeToDefaultNameMap.TryGetValue(typeof(TViewModel).Name, out var viewName))  
                viewName = name;  
  
            if (viewName != null && _vms.TryGetValue(viewName, value: out var vm))  
                return vm as TViewModel;  
  
            // ✅ 改为抛出异常，让调用栈明确指向问题根源  
            throw new InvalidOperationException(  
                $"[EXUI] ViewModel<{typeof(TViewModel).Name}> has not been loaded. Call OpenWindow or LoadWindow first.");  
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
                    
                    // ✅ 新增：如果是模态窗口，在其层级下方插入遮罩  
                    if (w.IsModal)  
                        InsertModalBackdrop(w);  
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
        
        private void InsertModalBackdrop(BaseView modalView)  
        {  
            var backdropObj = new GameObject("ModalBackdrop");  
            backdropObj.transform.SetParent(modalView.transform.parent, false);  
            backdropObj.transform.SetSiblingIndex(modalView.transform.GetSiblingIndex()); // 插到 modal 正下方  
  
            var image = backdropObj.AddComponent<Image>();  
            image.color = new Color(0, 0, 0, 0.5f);  
            image.raycastTarget = true; // ✅ 阻断下层点击  
  
            var rt = backdropObj.GetComponent<RectTransform>();  
            rt.anchorMin = Vector2.zero;  
            rt.anchorMax = Vector2.one;  
            rt.offsetMin = rt.offsetMax = Vector2.zero;  
        }
    }
}