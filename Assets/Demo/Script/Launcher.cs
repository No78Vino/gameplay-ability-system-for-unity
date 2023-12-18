using System;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using UnityEditor;
using UnityEngine;

namespace Demo.Script
{
    public class Launcher:MonoBehaviour
    {
        private void Awake()
        {
            XUI.Launch(LoadResource);
        }

        private void Start()
        {
            XUI.M.OpenWindow<MainUI>();
        }

        object LoadResource(string path, Type type)
        {
            var obj = AssetDatabase.LoadAssetAtPath(path, type);
            return obj;
        }
    }
}