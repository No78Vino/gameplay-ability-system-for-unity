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

        void LaunchBindingService();

        T LoadWindow<T>() where T : BaseView;

        void UnloadWindow<T>() where T : BaseView;

        T OpenWindow<T>() where T : BaseView;

        T VM<T>() where T : ViewModelCommon;

        BaseView Windows(Type type);

        BaseView WindowsWithoutLoad(Type type);
    }

    public sealed class EXMaidUI : IEXMaidUI
    {
        private BindingServiceBundle _bundle;
        private float _secondCount;
        private readonly Dictionary<string, ViewModelCommon> _vms;
        private readonly Dictionary<string, BaseView> _windows;

        public EXMaidUI()
        {
            _windows = new Dictionary<string, BaseView>();
            _vms = new Dictionary<string, ViewModelCommon>();
        }

        public void LaunchBindingService()
        {
            var context = Context.GetApplicationContext();
            var container = context.GetContainer();
            _bundle = new BindingServiceBundle(container);
            _bundle.Start();
        }

        public T LoadWindow<T>() where T : BaseView
        {
            var t = typeof(T);
            var w = Windows(t);
            return w as T;
        }

        public void UnloadWindow<T>(string name = null) where T : BaseView
        {
            var w = name ?? typeof(T).Name;
            if (!_windows.ContainsKey(w)) return;
            _windows[w].VM.OnUnload();
            _vms.Remove(w);

            _windows[w].OnDispose();
            _windows[w].Dispose();
            _windows.Remove(w);
        }

        public T OpenWindow<T>() where T : BaseView
        {
            var w = LoadWindow<T>();
            w.Show();
            return w;
        }

        public T VM<T>() where T : ViewModelCommon
        {
            if (_vms.TryGetValue(typeof(T), value: out var vm)) 
                return vm as T;
            
            Debug.LogError($"[EXUI] View Model:{typeof(T)} has not been loaded! Please LOAD it before CALLING.");
            return null;

        }

        public BaseView Windows(Type type,bool ifNullLoadIt = true)
        {
            if (!_windows.ContainsKey(type))
            {
                if (ifNullLoadIt)
                {
                    _windows.Add(type, Activator.CreateInstance(type) as BaseView);
                    var vm = _windows[type].VM;
                    _vms.Add(vm.GetType(), vm);
                    vm.OnLoaded();
                }
                else
                {
                    return null;
                }
            }

            return _windows[type];
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
            _bundle.Stop();
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