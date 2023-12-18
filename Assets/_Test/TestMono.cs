using System;
using EXMaid;
using UnityEngine;

namespace _Test
{
    public class TestMono : MonoBehaviour
    {
        private void FixedUpdate()
        {
            EXLog.Log($"Frame Count:{Time.frameCount} \n" +
                      $"Time:{Time.time}");
        }
    }
}