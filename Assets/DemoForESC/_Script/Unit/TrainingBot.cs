using System;
using EXUI;
using UI.View;
using UnityEngine;

namespace DemoForESC._Script
{
    /// <summary>
    ///  训练机器人
    /// </summary>
    public class TrainingBot : MonoBehaviour
    {
        private BaseUnit _unit;

        private void Awake()
        {
            _unit = GetComponent<BaseUnit>();
        }

        private void Start()
        {
            var w = XUI.M.OpenWindow<BossWindow>();
            w.VM.BindTargetHp(_unit);
        }
    }
}