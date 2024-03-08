using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaVisualization : MonoBehaviour
{
    [SerializeField] private Transform _circleArea;
    [SerializeField] private Transform _circleProgress;

    private float _progress;
    private float _size;
    
    public void SetAreaSize(float size)
    {
        _size = size;
        _circleArea.localScale = new Vector3(size, size, 1);
        SetProgress(0);
    }
    
    /// <summary>
    /// progress 0-1
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(float progress)
    {
        _progress = progress;
        _circleProgress.localScale = new Vector3(_size, _size, 1) * progress;
    }
}
