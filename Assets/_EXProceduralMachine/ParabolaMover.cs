using UnityEngine;

/// <summary>
/// 抛物线移动组件（基于Y轴的相对高度）
/// 支持中途打断并顺滑衔接新的抛物线移动
/// </summary>
[DisallowMultipleComponent]
public class ParabolaMover : MonoBehaviour
{
    // 缓存自身Transform，避免频繁获取
    private Transform _selfTrans;
    
    // 移动状态变量
    private bool _isMoving;          // 是否正在移动
    private float _currentTime;      // 当前已移动时间
    private float _totalDuration;    // 本次移动总时长
    private float _maxRelativeHeight;// 本次移动的最高相对高度（相对于起点到终点的连线）
    private Vector3 _startPos;       // 本次移动的起始位置
    private Vector3 _targetPos;      // 本次移动的目标位置

    private void Awake()
    {
        _selfTrans = transform;
    }

    private void Update()
    {
        if (!_isMoving) return;

        // 累加移动时间
        _currentTime += Time.deltaTime;
        // 归一化时间（0~1），超过1则设为1（到达终点）
        float timeNormalized = Mathf.Clamp01(_currentTime / _totalDuration);

        // 计算当前位置
        Vector3 currentPos = CalculateParabolaPosition(timeNormalized);
        _selfTrans.position = currentPos;

        // 检查是否到达终点
        if (timeNormalized >= 1f)
        {
            _isMoving = false;
            // 确保最终位置精准匹配目标点（避免浮点误差）
            _selfTrans.position = _targetPos;
        }
    }

    /// <summary>
    /// 启动/打断抛物线移动（核心方法，外部调用）
    /// </summary>
    /// <param name="target">目标位置</param>
    /// <param name="duration">移动总时长（秒）</param>
    /// <param name="maxRelativeHeight">抛物线最高相对高度（相对于起点到终点连线的Y轴高度）</param>
    public void MoveToParabola(Vector3 target, float duration, float maxRelativeHeight)
    {
        // 处理非法参数（避免除零/无效高度）
        if (duration <= 0)
        {
            _selfTrans.position = target;
            _isMoving = false;
            return;
        }

        // 重置移动状态（打断原有移动，顺滑衔接）
        _startPos = _selfTrans.position;    // 起始点设为当前位置
        _targetPos = target;
        _totalDuration = duration;
        _maxRelativeHeight = maxRelativeHeight;
        _currentTime = 0f;
        _isMoving = true;
    }

    /// <summary>
    /// 计算指定归一化时间的抛物线位置
    /// </summary>
    /// <param name="timeNormalized">归一化时间（0~1）</param>
    /// <returns>当前位置</returns>
    private Vector3 CalculateParabolaPosition(float timeNormalized)
    {
        // 1. 水平方向（X/Z）线性插值（匀速）
        float x = Mathf.Lerp(_startPos.x, _targetPos.x, timeNormalized);
        float z = Mathf.Lerp(_startPos.z, _targetPos.z, timeNormalized);

        // 2. 垂直方向（Y）抛物线插值（核心公式）
        float deltaY = _targetPos.y - _startPos.y;
        // 抛物线公式推导：y = -4h*t² + (Δy+4h)*t + y0 （h为相对高度）
        float y = -4 * _maxRelativeHeight * Mathf.Pow(timeNormalized, 2) 
                  + (deltaY + 4 * _maxRelativeHeight) * timeNormalized 
                  + _startPos.y;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 强制停止移动
    /// </summary>
    public void StopMove()
    {
        _isMoving = false;
    }

    /// <summary>
    /// 获取当前是否正在移动
    /// </summary>
    public bool IsMoving => _isMoving;
}