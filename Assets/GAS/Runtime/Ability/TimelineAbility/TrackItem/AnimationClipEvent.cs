using System;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [Serializable]
    public class AnimationClipEvent : ClipEventBase
    {
        public AnimationClip Clip;
        public float TransitionTime;
        //public int DurationFrame;
    }

    
    public class TestClassForGAS
    {
        public int x = 0;
        public string Y = "Hello";

        public void Test()
        {
            Debug.Log("TestClassForGAS");
        }
    }
    
    public class TestClassForGASSon: TestClassForGAS
    {
        public int z = 0;
        public string W = "World";
    }
    
    public class TestClassForGASSonB: TestClassForGAS
    {
        public int bSon = 0;
        public string bSonW = "World B";
    }
}