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

        /// <summary>
        ///     是否全屏
        /// </summary>
        protected bool _isFullScreen;

        /// <summary>
        ///     是否为模态窗口
        /// </summary>
        protected bool _isModal;

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

        public virtual void Show()
        {
            gameObject.SetActive(true);
            PlayShowAnim();
        }

        public virtual void Hide()
        {
            PlayHideAnim();
        }

        protected virtual void OnHideAnimEnd()
        {
            gameObject.SetActive(false);
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

        public bool IsShowing => gameObject.activeSelf;
        
        public virtual void DestroySelf()
        {
            Destroy(gameObject);
        }

        /// <summary>
        ///     BehaviourBindingExtension
        ///     // 动态绑定
        ///     BindingSet
        ///     <TView, T>
        ///         dynamicBindingSet = this.CreateBindingSet
        ///         <TView, T>
        ///             ();
        ///             BindDynamic(dynamicBindingSet);
        ///             dynamicBindingSet.Build();
        ///             // 静态绑定
        ///             BindingSet
        ///             <TView>
        ///                 staticBindingSet = this.CreateBindingSet
        ///                 <TView>
        ///                     ();
        ///                     BindStatic(staticBindingSet);
        ///                     staticBindingSet.Build();
        /// </summary>
        protected abstract void BindData();
    }

    public abstract class BaseView<T> : BaseView where T : ViewModelCommon
    {
        protected T _vm;
        public T VM => _vm;
        
        protected virtual void OnShow()
        {
            _vm.OnShow();
        }

        protected virtual void OnHide()
        {
            _vm.OnHide();
        }
        
        public virtual void Init(string viewName, T vm)
        {
            _name = viewName;
            _vm = vm;
            //将视图模型赋值到DataContext
            BindingContext.DataContext = _vm;
            // 绑定
            BindData();
        }
    }
}