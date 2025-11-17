using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace FairyGUI.Extension
{
    public abstract class BaseView<T> : UIView  where T : ViewModelCommon
    {
        protected string _name;
        public string Name => _name;
        
        protected string _prefabPath;
        public string PrefabPath => _prefabPath;

        /// <summary>
        ///     是否全屏
        /// </summary>
        protected bool _isFullScreen;

        /// <summary>
        /// 是否为模态窗口
        /// </summary>
        protected bool _isModal;

        protected IBindingContext _bindingContext;

        // // UI控件引用
        // // 静态：只执行一次绑定和同步
        // public Text title;
        //
        // // 动态：执行 双向/单向/逆向 绑定和同步
        // public Text username;

        protected T _vm;
        public T VM => _vm;

        private IBindingContext BindingContext
        {
            get { return _bindingContext ??= this.BindingContext(); }
        }

        public virtual void Init<TView>(string viewName, T vm) 
            where TView:BaseView<T>
        {
            _name = viewName;
            _vm = vm;
            //将视图模型赋值到DataContext
            BindingContext.DataContext = _vm;

            // 动态绑定
            BindingSet<TView, T> dynamicBindingSet = this.CreateBindingSet<TView, T>();
            BindDynamic(dynamicBindingSet);
            dynamicBindingSet.Build();
            
            // 静态绑定
            BindingSet<TView> staticBindingSet = this.CreateBindingSet<TView>();
            BindStatic(staticBindingSet);
            staticBindingSet.Build();
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
        
        protected virtual void OnShow()
        {
            _vm.OnShow();
        }

        protected virtual void OnHide()
        {
            _vm.OnHide();
        }

        protected virtual void OnHideAnimEnd()
        {
            gameObject.SetActive(false);    
        }
        
        public virtual void OnDispose()
        {
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
        /// 绑定动态同步的视图属性和模型数据
        ///  bindingSet.Bind(xxx).For(v => v.attr).To(vm => vm.data).OneWay();
        ///  bindingSet.Bind(xxx).For(v => v.attr).To(vm => vm.data).TwoWay();
        ///  bindingSet.Bind(xxx).For(v => v.attr).To(vm => vm.data).OneWayToSource();
        /// </summary>
        protected abstract void BindDynamic(BindingSet<TView, T> bindingSet) where TView:BaseView<T>;
        
        /// <summary>
        /// 绑定静态的视图属性和模型数据（只初始化一次的显示属性）
        ///  bindingSet.Bind(xxx).For(v => v.attr).To(vm => vm.data).OneTime();
        /// </summary>
        protected abstract void BindStatic(BindingSet<TView> bindingSet) where TView:BaseView<T>;
    }
}