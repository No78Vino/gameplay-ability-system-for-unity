using Cinemachine;
using UnityEngine;

/// <summary>
/// This is an add-on behaviour that globally maps the touch control 
/// to standard input channels, such as mouse X and mouse Y.
/// Drop it on any game object in your scene.
/// </summary>
public class CinemachineTouchInputMapper : MonoBehaviour
{
    /// <summary>Sensitivity multiplier for x-axis</summary>
    [Tooltip("Sensitivity multiplier for x-axis")]
    public float TouchSensitivityX = 10f;

    /// <summary>Sensitivity multiplier for y-axis</summary>
    [Tooltip("Sensitivity multiplier for y-axis")]
    public float TouchSensitivityY = 10f;

    /// <summary>Input channel to spoof for X axis</summary>
    [Tooltip("Input channel to spoof for X axis")]
    public string TouchXInputMapTo = "Mouse X";

    /// <summary>Input channel to spoof for Y axis</summary>
    [Tooltip("Input channel to spoof for Y axis")]
    public string TouchYInputMapTo = "Mouse Y";

    void Start()
    {
        CinemachineCore.GetInputAxis = GetInputAxis;
    }

    private float GetInputAxis(string axisName)
    {
        if (Input.touchCount > 0)
        {
            if (axisName == TouchXInputMapTo)
                return Input.touches[0].deltaPosition.x / TouchSensitivityX;
            if (axisName == TouchYInputMapTo)
                return Input.touches[0].deltaPosition.y / TouchSensitivityY;
        }
        return Input.GetAxis(axisName);
    }
}
