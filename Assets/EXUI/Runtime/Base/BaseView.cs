using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using UnityEngine;

namespace EXUI
{
    public abstract class BaseView: UIView
    {
        protected IBindingContext _bindingContext;
        
        // 子类通过 override 声明自己所在层级  
        public virtual UILayer Layer => UILayer.Normal;  

        /// <summary>
        ///     是否全屏
        /// </summary>
        public virtual bool IsFullScreen => false;  // 子类 override 声明  
        
        /// <summary>
        ///     是否为模态窗口
        /// </summary>
        public virtual bool IsModal => false;       // 子类 override 声明
        
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
            _isShowing = true;      // ✅ 立即标记为 showing  
            PlayShowAnim();  
            OnShow();  
        }  
  
        public virtual void Hide()  
        {  
            _isShowing = false;     // ✅ 立即标记为 not showing，UITick 停止驱动  
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
        
        /// <summary>
        /// 使用格式如下：
        /// var bindingDynamic = new BindingSet XXXWindow, VMXXXWindow (_bindingContext, this);
        /// bindingDynamic.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
        /// bindingDynamic.Bind(titleText).For(v => v.text).To(vm => vm.title.Value).OneWay();
        /// bindingDynamic.Bind(btnStartText).For(v => v.text).To(vm => vm.btnStartText.Value).OneWay();
        /// bindingDynamic.Bind(btnStart).For(v => v.onClick).To(vm => vm.GameStart);
        /// bindingDynamic.Build();
        /// 
        /// var bindingStatic = new BindingSet XXXWindow, VMXXXWindow (_bindingContext, this);
        /// bindingDynamic.Bind(titleText).For(v => v.text).To(vm => vm.title.Value).OneWay();
        /// bindingStatic.Build();
        /// </summary>
        protected abstract void BindData();
        
        protected abstract void InitViewComponents();
        
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
  
        public override void Init(string viewName)  
        {  
            base.Init(viewName);  
            _vm = Activator.CreateInstance<T>();  // 默认行为：无参构造  
            SetupViewModel();  
        }  
  
        // ✅ 新增：允许外部注入已构造好的 VM  
        public void InitWithViewModel(string viewName, T vm)  
        {  
            base.Init(viewName);  
            _vm = vm;  
            SetupViewModel();  
        }  
  
        private void SetupViewModel()  
        {  
            _viewModel = _vm;  
            BindingContext.DataContext = _vm;  
            InitViewComponents();  
            BindData();  
        }  
    }
}