using System;
using UnityEngine;

namespace GAS.General
{
    public class GASTimer
    {
        /// <summary>
        /// 帧间隔
        /// </summary>
        public static float TimeDelta { get; set; }
        /// <summary>
        /// 时间速度
        /// </summary>
        public static float TimeSpeed { get; set; } = 1;
        /// <summary>
        /// 当前时间(秒)
        /// </summary>
        public static float CurrentTimeSeconds
        {
            get
            {
                if (_isPaused)
                {
                    return _pauseTimeSeconds;
                }
                else
                {
                    return Time.time;
                }
            }
        }
        
        // TODO 矫正时间差(服务器客户端时间差/暂停游戏导致的时间差)
        static int _deltaTime;
        private static int _frameRate = 60;
        public static int FrameRate => _frameRate;
        
        private static long _startTimestamp;
        public static long StartTimestamp => _startTimestamp;
        public static long Timestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _deltaTime;
        public static long TimestampSeconds() => Timestamp() / 1000;
        private static int _currentFrameCount;
        public static int CurrentFrameCount => _currentFrameCount;
        private static long _pauseTimestamp;
        private static bool _isPaused = false;
        private static float _pauseTimeSeconds;
        
        public static void InitStartTimestamp()
        {
            _startTimestamp = Timestamp();
        }
        public static void UpdateCurrentFrameCount()
        {
            _currentFrameCount = Mathf.FloorToInt((Timestamp() - _startTimestamp) / 1000f * FrameRate);
            TimeDelta = Time.deltaTime * TimeSpeed;
        }
        
        public static void Pause()
        {
            _isPaused = true;
            _pauseTimestamp = Timestamp();
            _pauseTimeSeconds = Time.time;
        }
        
        public static void Unpause()
        {
            _isPaused = false;
            _deltaTime -= (int)(Timestamp() - _pauseTimestamp);
        }
    }
}