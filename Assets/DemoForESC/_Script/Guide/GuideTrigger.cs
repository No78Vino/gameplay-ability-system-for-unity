using System;
using System.Collections;
using System.Collections.Generic;
using DemoForESC._Script;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GuideTrigger : MonoBehaviour
{
    public string EventName;
    
    private void OnTriggerEnter(Collider other)
    {
        EventCenter.Trigger(EventName);
    }
}
