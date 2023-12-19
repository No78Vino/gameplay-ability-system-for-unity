using System;
using System.Collections;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using UnityEngine;
using YooAsset;

namespace Demo.Script
{
    public class Launcher : MonoBehaviour
    {
        private InputListener _inputListener;
        private Player _player;

        private IEnumerator Start()
        {
            YooAssets.Initialize();
            var packageName = "DefaultPackage";
            var package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);

            var initParameters = new EditorSimulateModeParameters();
            initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
            yield return package.InitializeAsync(initParameters);

            XUI.Launch(LoadResource);
            XUI.M.OpenWindow<MainUI>();
        }

        private object LoadResource(string path, Type type)
        {
            var obj = YooAssets.LoadAssetSync(path, type).AssetObject;
            return obj;
        }

        private void RegisterInputAction()
        {
            _player = FindObjectOfType<Player>();
            _inputListener = FindObjectOfType<InputListener>();

            _inputListener.RegisterOnMove(_player.OnMove);
            _inputListener.RegisterOnPressQ(_player.OnPressQ);
            _inputListener.RegisterOnPressE(_player.OnPressE);
            _inputListener.RegisterOnPressR(_player.OnPressR);
            _inputListener.RegisterOnPressMouseLeft(_player.OnPressMouseLeft);
            _inputListener.RegisterOnMousePosition(_player.OnMousePosition);
        }
    }
}