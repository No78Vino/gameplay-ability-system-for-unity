using System;
using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// Alternative to AxisState - a simplified structure to hold the definition of an input axis
    /// </summary>
    [Serializable]
    public struct AxisBase
    {
        /// <summary>The current value of the axis</summary>
        [NoSaveDuringPlay]
        [Tooltip("The current value of the axis.")]
        public float m_Value;

        /// <summary>The minimum value for the axis</summary>
        [Tooltip("The minimum value for the axis")]
        public float m_MinValue;

        /// <summary>The maximum value for the axis</summary>
        [Tooltip("The maximum value for the axis")]
        public float m_MaxValue;

        /// <summary>If checked, then the axis will wrap around at the min/max values, forming a loop</summary>
        [Tooltip("If checked, then the axis will wrap around at the min/max values, forming a loop")]
        public bool m_Wrap;

        /// <summary>
        /// Call this from OnValidate() to validate the fields of this structure (applies clamps, etc).
        /// </summary>
        public void Validate()
        {
            m_MaxValue = Mathf.Clamp(m_MaxValue, m_MinValue, m_MaxValue);
        }
    }

    /// <summary>
    /// A helper class to drive an input axis, as an alternative to the standard Cinemachine.AxisState.
    /// Behaviour is a simple direct scaling of the input channel, with no max speed.
    /// </summary>
    [Serializable]
    public struct CinemachineInputAxisDriver
    {
        /// <summary>Multiply the input by this amount prior to processing.  Controls the input power</summary>
        [Tooltip("Multiply the input by this amount prior to processing.  Controls the input power.")]
        public float multiplier;

        /// <summary>The amount of time in seconds it takes to accelerate to a higher speed</summary>
        [Tooltip("The amount of time in seconds it takes to accelerate to a higher speed")]
        public float accelTime;

        /// <summary>The amount of time in seconds it takes to decelerate to a lower speed</summary>
        [Tooltip("The amount of time in seconds it takes to decelerate to a lower speed")]
        public float decelTime;

        /// <summary>The name of this axis as specified in Unity Input manager. 
        /// Setting to an empty string will disable the automatic updating of this axis</summary>
        [Tooltip("The name of this axis as specified in Unity Input manager. "
            + "Setting to an empty string will disable the automatic updating of this axis")]
        public string name;

        /// <summary>The value of the input axis.  A value of 0 means no input.  You can drive 
        /// "this directly from a custom input system, or you can set the Axis Name and 
        /// have the value driven by the internal Input Manager</summary>
        [NoSaveDuringPlay]
        [Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive "
            + "this directly from a custom input system, or you can set the Axis Name and "
            + "have the value driven by the internal Input Manager")]
        public float inputValue;

        /// Internal state
        private float mCurrentSpeed;
        const float Epsilon =  UnityVectorExtensions.Epsilon;

        /// <summary>Call from OnValidate: Make sure the fields are sensible</summary>
        public void Validate()
        {
            accelTime = Mathf.Max(0, accelTime);
            decelTime = Mathf.Max(0, decelTime);
        }

        /// <summary>Update the axis</summary>
        /// <param name="deltaTime">current deltaTime</param>
        /// <param name="axis">The AxisState to update</param>
        /// <returns>True if the axis value changed due to user input, false otherwise</returns>
        public bool Update(float deltaTime, ref AxisBase axis)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try { inputValue = CinemachineCore.GetInputAxis(name); }
                catch (ArgumentException) {}
                //catch (ArgumentException e) { Debug.LogError(e.ToString()); }
            }

            float input = inputValue * multiplier;
            if (deltaTime < Epsilon)
                mCurrentSpeed = 0;
            else
            {
                float speed = input / deltaTime;
                float dampTime = Mathf.Abs(speed) < Mathf.Abs(mCurrentSpeed) ? decelTime : accelTime;
                speed = mCurrentSpeed + Damper.Damp(speed - mCurrentSpeed, dampTime, deltaTime);
                mCurrentSpeed = speed;

                // Decelerate to the end points of the range if not wrapping
                float range = axis.m_MaxValue - axis.m_MinValue;
                if (!axis.m_Wrap && decelTime > Epsilon && range > Epsilon)
                {
                    float v0 = ClampValue(ref axis, axis.m_Value);
                    float v = ClampValue(ref axis, v0 + speed * deltaTime);
                    float d = (speed > 0) ? axis.m_MaxValue - v : v - axis.m_MinValue;
                    if (d < (0.1f * range) && Mathf.Abs(speed) > Epsilon)
                        speed = Damper.Damp(v - v0, decelTime, deltaTime) / deltaTime;
                }
                input = speed * deltaTime;
            }

            axis.m_Value = ClampValue(ref axis, axis.m_Value + input);
            return Mathf.Abs(inputValue) > Epsilon;
        }


        /// <summary>Support for legacy AxisState struct: update the axis</summary>
        /// <param name="deltaTime">current deltaTime</param>
        /// <param name="axis">The AxisState to update</param>
        /// <returns>True if the axis value changed due to user input, false otherwise</returns>
        public bool Update(float deltaTime, ref AxisState axis)
        {
            var a = new AxisBase
            {
                m_Value = axis.Value,
                m_MinValue = axis.m_MinValue,
                m_MaxValue = axis.m_MaxValue,
                m_Wrap = axis.m_Wrap
            };
            bool changed = Update(deltaTime, ref a);
            axis.Value = a.m_Value;
            return changed;
        }
        
        float ClampValue(ref AxisBase axis, float v)
        {
            float r = axis.m_MaxValue - axis.m_MinValue;
            if (axis.m_Wrap && r > Epsilon)
            {
                v = (v - axis.m_MinValue) % r;
                v += axis.m_MinValue + ((v < 0) ? r : 0);
            }
            return Mathf.Clamp(v, axis.m_MinValue, axis.m_MaxValue);
        }
    }
}
