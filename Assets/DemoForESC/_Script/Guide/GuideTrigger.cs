using System;
using System.Collections;
using System.Collections.Generic;
using DemoForESC._Script;
using GAS.Runtime;
using UnityEngine;

[RequireComponent(typeof(Collider))]  
public class GuideTrigger : MonoBehaviour  
{  
    public string EventName;  
  
    private void OnTriggerEnter(Collider other)  
    {  
        if (!string.IsNullOrEmpty(EventName))  
            EventCenter.Trigger(EventName);  
    }  
}