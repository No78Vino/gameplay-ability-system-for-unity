using System;
using System.Globalization;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace FairyGUI.Extension
{
    public abstract class BaseView : UIView
    {
        protected string _name;
        public string Name => _name;

        /// <summary>
        ///     是否全屏
        /// </summary>
        protected bool _isFullScreen;

        protected bool _isModal;

        protected IBindingContext _bindingContext;
        
        // UI控件引用
        // 静态：只执行一次绑定和同步
        public Text title;

        // 动态：执行 双向/单向/逆向 绑定和同步
        public Text username;

        protected ViewModelCommon _vm;
        public ViewModelCommon VM => _vm;

        private IBindingContext BindingContext
        {
            get { return _bindingContext ??= this.BindingContext(); }
        }
        
        protected override void Awake()
        {
            //获得应用上下文
            ApplicationContext context = Context.GetApplicationContext();
            //启动数据绑定服务
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }

        public virtual void Init(ViewModelCommon vm)
        {
            _vm = vm;
            //将视图模型赋值到DataContext
            BindingContext.DataContext = _vm;
            
            // 动态绑定
            BindingSet<BaseView, DatabindingViewModel> bindingSet =
                this.CreateBindingSet<BaseView, DatabindingViewModel>();
            //bindingSet.Bind(xxx).For(v => v.text).To(vm => vm.Account.Username).OneWay();
            // 静态绑定
            BindingSet<BaseView> staticBindingSet = this.CreateBindingSet<BaseView>();
            //staticBindingSet.Bind(yyy).For(v => v.text).To(() => Res.databinding_tutorials_title).OneTime();

            bindingSet.Build();
            staticBindingSet.Build();
        }
    }


    public abstract class BaseWindow : Window
    {
        private IBindingContext _bindingContext;

        /// <summary>
        ///     是否全屏
        /// </summary>
        protected bool _isFullScreen;

        protected bool _isModal;

        private GButton _modalButton;

        protected string _pkgName;
        protected ViewModelCommon _vm;
        protected string _windowPathName;

        public ViewModelCommon VM => _vm;

        protected IBindingContext bindingContext
        {
            get { return _bindingContext ??= contentPane.displayObject.gameObject.BindingContext(); }
        }

        protected void CreateContentPane(ViewModelCommon vm, string pkgName, string windowName, bool isFullScreen)
        {
            _vm = vm;
            _pkgName = pkgName;
            _windowPathName = windowName;
            _isFullScreen = isFullScreen;
            FairyGUIPackageExtension.LoadPackage(_pkgName);
            contentPane = UIPackage.CreateObject(_pkgName, _windowPathName).asCom;
            if (_isFullScreen) MakeFullScreen();
            bindingContext.DataContext = _vm;

            // 点击不显示顺序影响
            bringToFontOnClick = false;
        }

        protected GObject _ui(string path)
        {
            var arr = path.Split('.');
            var cnt = arr.Length;
            var gcom = contentPane;
            GObject obj = null;
            for (var i = 0; i < cnt; ++i)
            {
                if (arr[i].EndsWith("]"))
                {
                    var listName = arr[i].Substring(0, arr[i].IndexOf('['));
                    obj = gcom.GetChild(listName);
                    if (obj is GList list)
                    {
                        var index = arr[i].Substring(arr[i].IndexOf('[') + 1, arr[i].Length - 2 - listName.Length);
                        if (index == "last")
                        {
                            var actualIdx = list.ItemIndexToChildIndex(list.numItems - 1); // 如果是GList,注意元素索引和子项索引的转换关系
                            obj = actualIdx >= 0 ? list.GetChildAt(actualIdx) : null;
                        }
                        else
                        {
                            if (int.TryParse(index, out var idx))
                            {
                                var actualIdx = list.ItemIndexToChildIndex(idx); // 如果是GList,注意元素索引和子项索引的转换关系
                                obj = actualIdx >= 0 ? list.GetChildAt(actualIdx) : null;
                            }
                            else
                            {
                                obj = null;
                            }
                        }
                    }
                    else
                    {
                        obj = null;
                    }
                }
                else
                {
                    obj = gcom.GetChild(arr[i]);
                }

                if (obj == null) break;
                if (i == cnt - 1) continue;
                if (!(obj is GComponent))
                {
                    obj = null;
                    break;
                }

                gcom = (GComponent)obj;
            }


            if (obj == null)
                Debug.LogError($"[FairyGUI] No Component Path:{path} In WindowComponent:{_windowPathName}.");
            return obj;
        }

        public GObject GetUI(string path)
        {
            return _ui(path);
        }

        protected virtual void Msg_Common(object sender, InteractionEventArgs args)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (args.Context != null)
                Debug.Log($"{GetType()} Msg_Common args.Context = {args.Context}");
#endif
        }

        protected virtual void Msg_Transition(object sender, InteractionEventArgs args)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (args.Context != null)
                Debug.Log($"{GetType()} Msg_Transition args.Context = {args.Context}");
#endif
        }

        protected override void OnShown()
        {
            base.OnShown();
            _vm.OnOpen();
        }

        public virtual void OnDispose()
        {
        }
    }
}