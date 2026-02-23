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
        switch (EventName)
        {
            case "GuideMove":
                EventCenter.Trigger(EventName);
                break;
            case "GuideRun":
                if (DemoPlayer.Player().AbilitySystemComponent.HasTag(XTag.State_Buff_SpeedUp))
                {
                    EventCenter.Trigger(EventName);
                }
                break;
            default:
                EventCenter.Trigger(EventName);
                break;
        }
    }
}
