using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using UnityEngine;
using System.Collections.Generic;  
using System.Linq.Expressions;  
using System.Reflection;  
using Loxodon.Framework.Binding.Builder;

namespace EXUI
{
    public abstract class BaseView : UIView
    {
        protected IBindingContext _bindingContext;

        // 子类通过 override 声明自己所在层级  
        public virtual UILayer Layer => UILayer.Normal;

        /// <summary>
        ///     是否全屏
        /// </summary>
        public virtual bool IsFullScreen => false; // 子类 override 声明  

        /// <summary>
        ///     是否为模态窗口
        /// </summary>
        public virtual bool IsModal => Layer == UILayer.Modal; // 子类 override 声明

        protected string _name;

        protected string _prefabPath;

        public string Name => _name;
        public string PrefabPath => _prefabPath;

        protected ViewModelCommon _viewModel;
        public ViewModelCommon ViewModel => _viewModel;

        protected IBindingContext BindingContext
        {
            get { return _bindingContext ??= this.BindingContext(); }
        }

        public virtual void Init(string viewName)
        {
            _name = viewName;
        }

        protected virtual void OnShow()
        {
            _viewModel.OnShow();
        }

        protected virtual void OnHide()
        {
            _viewModel.OnHide();
        }


        // BaseView.cs  
        private bool _isShowing = false;
        public bool IsShowing => _isShowing;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            _isShowing = true; // ✅ 立即标记为 showing  
            PlayShowAnim();
            OnShow();
        }

        public virtual void Hide()
        {
            _isShowing = false; // ✅ 立即标记为 not showing，UITick 停止驱动  
            PlayHideAnim();
            OnHide();
        }

        protected virtual void OnHideAnimEnd()
        {
            gameObject.SetActive(false); // GameObject 延迟禁用，不影响 IsShowing 判断  
        }

        public virtual void PlayShowAnim()
        {
        }

        public virtual void PlayHideAnim()
        {
            OnHideAnimEnd();
        }

        protected virtual void OnReceiveMessage(object sender, InteractionEventArgs args)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (args.Context != null)
                Debug.Log($"{GetType()} receive message.   args.Context = {args.Context}");
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DestroySelf()
        {
            Destroy(gameObject);
        }
        
        protected virtual void BindData() { }  
        protected virtual void InitViewComponents() { }

        protected TCom GetComponentByNode<TCom>(string path) where TCom : Component
        {
            var node = transform.Node(path);
            if (node != null) return transform.Node(path).GetComponent<TCom>();

            Debug.LogError($"Can't Find Node {path}");
            return null;
        }
    }

    public abstract class BaseView<T> : BaseView where T : ViewModelCommon
    {
        protected T _vm;
        public T VM => _vm;

        // ✅ 静态缓存：每种 View 类型只反射一次  
        // 泛型静态字段在不同的 T 之间是独立的，互不共享  
        private static List<BindingDescriptor> _cachedDescriptors;

        public override void Init(string viewName)
        {
            base.Init(viewName);
            _vm = Activator.CreateInstance<T>();
            _viewModel = _vm;
            BindingContext.DataContext = _vm;
            InitViewComponents();

            // ✅ 首次调用时反射扫描，之后直接命中缓存  
            if (_cachedDescriptors == null)
                _cachedDescriptors = BuildDescriptorsViaReflection(GetType());

            // BaseView<T>.Init() 中，在 ApplyAttributeBindings 之前增加：  
            AutoInitFieldsFromAttributes(_cachedDescriptors);
            ApplyAttributeBindings(_cachedDescriptors);

            // ✅ 手写的 BindData() 继续执行，处理事件绑定等复杂场景  
            BindData();
        }

        public void InitWithViewModel(string viewName, T vm)  
        {  
            base.Init(viewName);  
            _vm = vm;  
            _viewModel = _vm;  
            BindingContext.DataContext = _vm;  
            InitViewComponents();  
      
            _cachedDescriptors ??= BuildDescriptorsViaReflection(GetType());  
            ApplyAttributeBindings(_cachedDescriptors);  // ← 补上  
      
            BindData();  
        }
        
        // ====== 反射扫描：仅首次执行 ======    
        private static List<BindingDescriptor> BuildDescriptorsViaReflection(Type viewType)  
        {  
            var result = new List<BindingDescriptor>();  
            var fields = viewType.GetFields(  
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);  
  
            foreach (var field in fields)  
            {  
                // 处理 [BindOneWay]    
                var oneWay = field.GetCustomAttribute<BindOneWayAttribute>();  
                if (oneWay != null)  
                {  
                    result.Add(new BindingDescriptor  
                    {  
                        FieldInfo = field,  
                        ComponentType = field.FieldType,        // 新增  
                        NodePath = oneWay.NodePath,  
                        VMPropertyName = oneWay.VMPropertyName,  
                        UIPropertyName = oneWay.UIPropertyName,  
                        Mode = BindingMode.OneWay,  
                        FieldGetter = BuildFieldGetter(viewType, field)  
                    });  
                }  
  
                // 处理 [BindTwoWay]    
                var twoWay = field.GetCustomAttribute<BindTwoWayAttribute>();  
                if (twoWay != null)  
                {  
                    result.Add(new BindingDescriptor  
                    {  
                        FieldInfo = field,  
                        ComponentType = field.FieldType,        // 新增  
                        NodePath = twoWay.NodePath,  
                        VMPropertyName = twoWay.VMPropertyName,  
                        UIPropertyName = twoWay.UIPropertyName,  
                        Mode = BindingMode.TwoWay,  
                        FieldGetter = BuildFieldGetter(viewType, field)  
                    });  
                }  
            }  
  
            return result;  
        }

        // ====== 将 FieldInfo 编译为强类型委托，彻底消除运行时反射 ======  
        private static Func<object, UnityEngine.Component> BuildFieldGetter(Type viewType, FieldInfo field)
        {
            var param = Expression.Parameter(typeof(object), "instance");
            var cast = Expression.Convert(param, viewType);
            var access = Expression.Field(cast, field);
            var convertToComponent = Expression.Convert(access, typeof(UnityEngine.Component));
            return Expression.Lambda<Func<object, UnityEngine.Component>>(convertToComponent, param).Compile();
        }

        // ====== 用缓存的描述符建立绑定，无反射 ======    
        private void ApplyAttributeBindings(List<BindingDescriptor> descriptors)  
        {  
            if (descriptors == null || descriptors.Count == 0) return;  
  
            // ✅ 核心修复：使用非泛型 BindingSet  
            // 泛型 BindingSet<BaseView<T>, T> 会导致 BindingBuilder 将 TargetType   
            // 设为 typeof(BaseView<T>)，代理工厂在该类型上找不到 "text" 等属性。  
            // 非泛型 BindingSet 的 BindingBuilder 会用 target.GetType() 获取运行时类型  
            // （如 Text），从而正确创建属性代理。  
            var bindingSet = new BindingSet(_bindingContext, this);  
            foreach (var desc in descriptors)  
            {  
                // 用缓存的委托获取组件引用，无 FieldInfo.GetValue 反射    
                var component = desc.FieldGetter(this);  
  
                // ✅ 新增：字段为 null 时，利用 NodePath 自动查找并初始化组件  
                if (component == null && !string.IsNullOrEmpty(desc.NodePath))  
                {  
                    var node = transform.Node(desc.NodePath);  
                    if (node != null)  
                    {  
                        component = node.GetComponent(desc.ComponentType) as Component;  
                        if (component != null)  
                        {  
                            // 回写字段，使 BindData() 等后续代码也能访问  
                            desc.FieldInfo.SetValue(this, component);  
                        }  
                    }  
                    else  
                    {  
                        Debug.LogError(  
                            $"[EXUI] ApplyAttributeBindings: Can't find node '{desc.NodePath}' " +  
                            $"for field '{desc.FieldInfo.Name}' in {GetType().Name}");  
                    }  
                }  
  
                if (component == null) continue;  
  
                if (desc.Mode == BindingMode.OneWay)  
                    bindingSet.Bind(component)  
                        .For(desc.UIPropertyName)  
                        .To($"{desc.VMPropertyName}.Value")  
                        .OneWay();  
                else if (desc.Mode == BindingMode.TwoWay)  
                    bindingSet.Bind(component)  
                        .For(desc.UIPropertyName)  
                        .To($"{desc.VMPropertyName}.Value")  
                        .TwoWay();  
            }  
  
            bindingSet.Build();  
        }
        
        private void AutoInitFieldsFromAttributes(List<BindingDescriptor> descriptors)  
        {  
            foreach (var desc in descriptors)  
            {  
                if (string.IsNullOrEmpty(desc.NodePath)) continue;  
                var node = transform.Node(desc.NodePath);  
                if (node == null)  
                {  
                    Debug.LogError($"Can't Find Node {desc.NodePath}");  
                    continue;  
                }  
                var component = node.GetComponent(desc.FieldInfo.FieldType);  
                if (component != null)  
                    desc.FieldInfo.SetValue(this, component);  
            }  
        }
    }

    // ====== 绑定描述符：纯数据结构，反射完成后不再需要 ======    
    internal struct BindingDescriptor  
    {  
        public FieldInfo FieldInfo;  
        public Type ComponentType;              // 新增：字段声明类型（如 typeof(Text)）  
        public string NodePath;  
        public string VMPropertyName;  
        public string UIPropertyName;  
        public BindingMode Mode;  
        public Func<object, UnityEngine.Component> FieldGetter; // 编译好的委托    
    }
}