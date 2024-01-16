using System;
using System.Collections;
using Cysharp.Threading.Tasks.Triggers;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace Demo.Script
{
    public class Launcher : MonoBehaviour
    {
        private InputListener _inputListener;
        private Player _player;
        [SerializeField] string prefixOfFguiPackagePath = "Assets/Game/FairyGUI/";
        
        private IEnumerator Start()
        {
            XUI.Launch(prefixOfFguiPackagePath,LoadResource);
            XUI.M.OpenWindow<MainUI>();

            yield return new WaitForSeconds(1f);
            RegisterInputAction();
        }

        private object LoadResource(string path, Type type)
        {
            var split = path.Split('/');
            int indexOfResources = Array.IndexOf(split, "Resources");
            string resourcePath = "";
            for (int i = indexOfResources + 1; i < split.Length; i++)
            {
                resourcePath += split[i];
                if (i != split.Length - 1)
                {
                    resourcePath += "/";
                }
            }
            resourcePath = resourcePath.Substring( 0, resourcePath.LastIndexOf('.'));
            var obj = Resources.Load(resourcePath, type);
            return obj;
        }

        private void RegisterInputAction()
        {
            _player = FindObjectOfType<Player>();
            _inputListener = FindObjectOfType<InputListener>();

            _inputListener.RegisterOnMove(_player.OnMove);
            _inputListener.RegisterOnMoveEnd(_player.OnMoveEnd);
            _inputListener.RegisterOnPressQ(_player.OnPressQ);
            _inputListener.RegisterOnPressE(_player.OnPressE);
            _inputListener.RegisterOnPressR(_player.OnPressR);
            _inputListener.RegisterOnPressMouseLeft(_player.OnPressMouseLeft);
            _inputListener.RegisterOnMousePosition(_player.OnMousePosition);
        }
    }
}