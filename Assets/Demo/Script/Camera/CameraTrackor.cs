using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTrackor:MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float trackTime = 0.3f;

    private Vector2 trackPoint;
    private Vector2 startPoint;
    private float timeCount;
    private void Update()
    {
        if (target == null) return;

        if (((Vector2)transform.position - (Vector2)target.transform.position).magnitude < 0.1f) return;

        if ((trackPoint - (Vector2)target.transform.position).magnitude > 0.1f)
        {
            trackPoint = target.transform.position;
            startPoint = transform.position;
            timeCount = trackTime;
        }

        timeCount -= Time.deltaTime;
        Vector3 pos = Vector2.Lerp(startPoint, trackPoint, timeCount / trackTime);
        pos.z = -10;
        pos.y = 0;
        transform.position = pos;
    }
}