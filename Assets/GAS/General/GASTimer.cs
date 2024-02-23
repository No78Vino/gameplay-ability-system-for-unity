using System;

namespace GAS.General
{
    public class GASTimer
    {
        /// <summary>
        ///  The delta time between the server and the client.
        /// </summary>
        static long _deltaTimeWithServer;

        /// <summary>
        /// Notice: The time unit of this timestamp is milliseconds.
        /// Therefore, the time unit of the delta time is also milliseconds.
        /// </summary>
        /// <returns></returns>
        public static long Timestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _deltaTimeWithServer;

        public static long TimestampSeconds() => Timestamp() / 1000;

        
        private static int _frameCount;
        public static int FrameCount => _frameCount;


        private static long _startTimestamp;
        public static float Time;

        private static int _frameRate = 60;
        public static int FrameRate => _frameRate;
        
        public static void SetFrameRate(int rate)
        {
            _frameRate = rate;
        }
    }
}