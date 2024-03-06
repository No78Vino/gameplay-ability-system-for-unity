using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCore : MonoBehaviour
{
    public float rotateSpeed = 180;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotateAngle = rotateSpeed * Time.deltaTime;
        var localEulerAngles = transform.localEulerAngles;
        localEulerAngles.z += rotateAngle;
        localEulerAngles.z %= 360;
        transform.localEulerAngles = localEulerAngles;
    }
}
