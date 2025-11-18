using UnityEngine;
using System;
using Cinemachine.Utility;
using UnityEngine.Serialization;

namespace Cinemachine
{
    /// <summary>
    /// Axis state for defining how to react to player input.
    /// The settings here control the responsiveness of the axis to player input.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct AxisState
    {
        /// <summary>The current value of the axis</summary>
        [NoSaveDuringPlay]
        [Tooltip("The current value of the axis.")]
        public float Value;

        /// <summary>How to interpret the Max Speed setting.</summary>
        public enum SpeedMode
        {
            /// <summary>
            /// The Max Speed setting will be interpreted as a maximum axis speed, in units/second
            /// </summary>
            MaxSpeed,

            /// <summary>
            /// The Max Speed setting will be interpreted as a direct multiplier on the input value
            /// </summary>
            InputValueGain
        };

        /// <summary>How to interpret the Max Speed setting.</summary>
        [Tooltip("How to interpret the Max Speed setting: in units/second, or as a "
            + "direct input value multiplier")]
        public SpeedMode m_SpeedMode;

        /// <summary>How fast the axis value can travel.  Increasing this number
        /// makes the behaviour more responsive to joystick input</summary>
        [Tooltip("The maximum speed of this axis in units/second, or the input value "
            + "multiplier, depending on the Speed Mode")]
        public float m_MaxSpeed;

        /// <summary>The amount of time in seconds it takes to accelerate to
        /// MaxSpeed with the supplied Axis at its maximum value</summary>
        [Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed "
            + "with the supplied Axis at its maximum value")]
        public float m_AccelTime;

        /// <summary>The amount of time in seconds it takes to decelerate
        /// the axis to zero if the supplied axis is in a neutral position</summary>
        [Tooltip("The amount of time in seconds it takes to decelerate the axis to "
            + "zero if the supplied axis is in a neutral position")]
        public float m_DecelTime;

        /// <summary>The name of this axis as specified in Unity Input manager.
        /// Setting to an empty string will disable the automatic updating of this axis</summary>
        [FormerlySerializedAs("m_AxisName")]
        [Tooltip("The name of this axis as specified in Unity Input manager. "
            + "Setting to an empty string will disable the automatic updating of this axis")]
        public string m_InputAxisName;

        /// <summary>The value of the input axis.  A value of 0 means no input
        /// You can drive this directly from a
        /// custom input system, or you can set the Axis Name and have the value
        /// driven by the internal Input Manager</summary>
        [NoSaveDuringPlay]
        [Tooltip("The value of the input axis.  A value of 0 means no input.  "
            + "You can drive this directly from a custom input system, or you can set "
            + "the Axis Name and have the value driven by the internal Input Manager")]
        public float m_InputAxisValue;

        /// <summary>If checked, then the raw value of the input axis will be inverted
        /// before it is used.</summary>
        [FormerlySerializedAs("m_InvertAxis")]
        [Tooltip("If checked, then the raw value of the input axis will be inverted "
            + "before it is used")]
        public bool m_InvertInput;

        /// <summary>The minimum value for the axis</summary>
        [Tooltip("The minimum value for the axis")]
        public float m_MinValue;

        /// <summary>The maximum value for the axis</summary>
        [Tooltip("The maximum value for the axis")]
        public float m_MaxValue;

        /// <summary>If checked, then the axis will wrap around at the 
        /// min/max values, forming a loop</summary>
        [Tooltip("If checked, then the axis will wrap around at the min/max values, "
            + "forming a loop")]
        public bool m_Wrap;

        /// <summary>Automatic recentering.  Valid only if HasRecentering is true</summary>
        [Tooltip("Automatic recentering to at-rest position")]
        public Recentering m_Recentering;

        float m_CurrentSpeed;
        float m_LastUpdateTime;
        int m_LastUpdateFrame;

        /// <summary>Constructor with specific values</summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="wrap"></param>
        /// <param name="rangeLocked"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="accelTime"></param>
        /// <param name="decelTime"></param>
        /// <param name="name"></param>
        /// <param name="invert"></param>
        public AxisState(
            float minValue, float maxValue, bool wrap, bool rangeLocked,
            float maxSpeed, float accelTime, float decelTime,
            string name, bool invert)
        {
            m_MinValue = minValue;
            m_MaxValue = maxValue;
            m_Wrap = wrap;
            ValueRangeLocked = rangeLocked;

            HasRecentering = false;
            m_Recentering = new Recentering(false, 1, 2);

            m_SpeedMode = SpeedMode.MaxSpeed;
            m_MaxSpeed = maxSpeed;
            m_AccelTime = accelTime;
            m_DecelTime = decelTime;
            Value = (minValue + maxValue) / 2;
            m_InputAxisName = name;
            m_InputAxisValue = 0;
            m_InvertInput = invert;

            m_CurrentSpeed = 0f;
            m_InputAxisProvider = null;
            m_InputAxisIndex = 0;
            m_LastUpdateTime = 0;
            m_LastUpdateFrame = 0;
        }

        /// <summary>Call from OnValidate: Make sure the fields are sensible</summary>
        public void Validate()
        {
            if (m_SpeedMode == SpeedMode.MaxSpeed)
                m_MaxSpeed = Mathf.Max(0, m_MaxSpeed);
            m_AccelTime = Mathf.Max(0, m_AccelTime);
            m_DecelTime = Mathf.Max(0, m_DecelTime);
            m_MaxValue = Mathf.Clamp(m_MaxValue, m_MinValue, m_MaxValue);
        }

        const float Epsilon = UnityVectorExtensions.Epsilon;

        /// <summary>
        /// Cancel current input state and reset input to 0
        /// </summary>
        public void Reset()
        {
            m_InputAxisValue = 0;
            m_CurrentSpeed = 0;
            m_LastUpdateTime = 0;
            m_LastUpdateFrame = 0;
        }

        /// <summary>
        /// This is an interface to override default querying of Unity's legacy Input system.
        /// If a befaviour implementing this interface is attached to a Cinemachine virtual camera that 
        /// requires input, that interface will be polled for input instead of the standard Input system.
        /// </summary>
        public interface IInputAxisProvider
        {
            /// <summary>Get the value of the input axis</summary>
            /// <param name="axis">Which axis to query: 0, 1, or 2.  These represent, respectively, the X, Y, and Z axes</param>
            /// <returns>The input value of the axis queried</returns>
            float GetAxisValue(int axis);
        }
        IInputAxisProvider m_InputAxisProvider;
        int m_InputAxisIndex;

        /// <summary>
        /// Set an input provider for this axis.  If an input provider is set, the 
        /// provider will be queried when user input is needed, and the Input Axis Name 
        /// field will be ignored.  If no provider is set, then the legacy Input system 
        /// will be queried, using the Input Axis Name.
        /// </summary>
        /// <param name="axis">Which axis will be queried for input</param>
        /// <param name="provider">The input provider</param>
        public void SetInputAxisProvider(int axis, IInputAxisProvider provider)
        {
            m_InputAxisIndex = axis;
            m_InputAxisProvider = provider;
        }

        /// <summary>Returns true if this axis has an InputAxisProvider, in which case 
        /// we ignore the input axis name</summary>
        public bool HasInputProvider { get => m_InputAxisProvider != null; }

        /// <summary>
        /// Updates the state of this axis based on the Input axis defined
        /// by AxisState.m_AxisName
        /// </summary>
        /// <param name="deltaTime">Delta time in seconds</param>
        /// <returns>Returns <b>true</b> if this axis's input was non-zero this Update,
        /// <b>false</b> otherwise</returns>
        public bool Update(float deltaTime)
        {
            // Update only once per frame
            if (Time.frameCount == m_LastUpdateFrame)
                return false;
            m_LastUpdateFrame = Time.frameCount;

            // Cheating: we want the render frame time, not the fixed frame time
            if (deltaTime >= 0 && m_LastUpdateTime != 0) 
                deltaTime = Time.realtimeSinceStartup - m_LastUpdateTime;
            
            m_LastUpdateTime = Time.realtimeSinceStartup;
            
            if (m_InputAxisProvider != null)
                m_InputAxisValue = m_InputAxisProvider.GetAxisValue(m_InputAxisIndex);
            else if (!string.IsNullOrEmpty(m_InputAxisName))
            {
                try { m_InputAxisValue = CinemachineCore.GetInputAxis(m_InputAxisName); }
                catch (ArgumentException e) { Debug.LogError(e.ToString()); }
            }

            float input = m_InputAxisValue;
            if (m_InvertInput)
                input *= -1f;

            if (m_SpeedMode == SpeedMode.MaxSpeed)
                return MaxSpeedUpdate(input, deltaTime); // legacy mode

            // Direct mode update: maxSpeed interpreted as multiplier
            input *= m_MaxSpeed;
            if (deltaTime < Epsilon)
                m_CurrentSpeed = 0;
            else
            {
                float speed = input / deltaTime;
                float dampTime = Mathf.Abs(speed) < Mathf.Abs(m_CurrentSpeed) ? m_DecelTime : m_AccelTime;
                speed = m_CurrentSpeed + Damper.Damp(speed - m_CurrentSpeed, dampTime, deltaTime);
                m_CurrentSpeed = speed;

                // Decelerate to the end points of the range if not wrapping
                float range = m_MaxValue - m_MinValue;
                if (!m_Wrap && m_DecelTime > Epsilon && range > Epsilon)
                {
                    float v0 = ClampValue(Value);
                    float v = ClampValue(v0 + speed * deltaTime);
                    float d = (speed > 0) ? m_MaxValue - v : v - m_MinValue;
                    if (d < (0.1f * range) && Mathf.Abs(speed) > Epsilon)
                        speed = Damper.Damp(v - v0, m_DecelTime, deltaTime) / deltaTime;
                }
                input = speed * deltaTime;
            }
            Value = ClampValue(Value + input);
            return Mathf.Abs(input) > Epsilon;
        }

        float ClampValue(float v)
        {
            float r = m_MaxValue - m_MinValue;
            if (m_Wrap && r > Epsilon)
            {
                v = (v - m_MinValue) % r;
                v += m_MinValue + ((v < 0) ? r : 0);
            }
            return Mathf.Clamp(v, m_MinValue, m_MaxValue);
        }

        bool MaxSpeedUpdate(float input, float deltaTime)
        {
            if (m_MaxSpeed > Epsilon)
            {
                float targetSpeed = input * m_MaxSpeed;
                if (Mathf.Abs(targetSpeed) < Epsilon
                    || (Mathf.Sign(m_CurrentSpeed) == Mathf.Sign(targetSpeed)
                        && Mathf.Abs(targetSpeed) <  Mathf.Abs(m_CurrentSpeed)))
                {
                    // Need to decelerate
                    float a = Mathf.Abs(targetSpeed - m_CurrentSpeed) / Mathf.Max(Epsilon, m_DecelTime);
                    float delta = Mathf.Min(a * deltaTime, Mathf.Abs(m_CurrentSpeed));
                    m_CurrentSpeed -= Mathf.Sign(m_CurrentSpeed) * delta;
                }
                else
                {
                    // Accelerate to the target speed
                    float a = Mathf.Abs(targetSpeed - m_CurrentSpeed) / Mathf.Max(Epsilon, m_AccelTime);
                    m_CurrentSpeed += Mathf.Sign(targetSpeed) * a * deltaTime;
                    if (Mathf.Sign(m_CurrentSpeed) == Mathf.Sign(targetSpeed)
                        && Mathf.Abs(m_CurrentSpeed) > Mathf.Abs(targetSpeed))
                    {
                        m_CurrentSpeed = targetSpeed;
                    }
                }
            }

            // Clamp our max speeds so we don't go crazy
            float maxSpeed = GetMaxSpeed();
            m_CurrentSpeed = Mathf.Clamp(m_CurrentSpeed, -maxSpeed, maxSpeed);

            Value += m_CurrentSpeed * deltaTime;
            bool isOutOfRange = (Value > m_MaxValue) || (Value < m_MinValue);
            if (isOutOfRange)
            {
                if (m_Wrap)
                {
                    if (Value > m_MaxValue)
                        Value = m_MinValue + (Value - m_MaxValue);
                    else
                        Value = m_MaxValue + (Value - m_MinValue);
                }
                else
                {
                    Value = Mathf.Clamp(Value, m_MinValue, m_MaxValue);
                    m_CurrentSpeed = 0f;
                }
            }
            return Mathf.Abs(input) > Epsilon;
        }

        // MaxSpeed may be limited as we approach the range ends, in order
        // to prevent a hard bump
        float GetMaxSpeed()
        {
            float range = m_MaxValue - m_MinValue;
            if (!m_Wrap && range > 0)
            {
                float threshold = range / 10f;
                if (m_CurrentSpeed > 0 && (m_MaxValue - Value) < threshold)
                {
                    float t = (m_MaxValue - Value) / threshold;
                    return Mathf.Lerp(0, m_MaxSpeed, t);
                }
                else if (m_CurrentSpeed < 0 && (Value - m_MinValue) < threshold)
                {
                    float t = (Value - m_MinValue) / threshold;
                    return Mathf.Lerp(0, m_MaxSpeed, t);
                }
            }
            return m_MaxSpeed;
        }

        /// <summary>Value range is locked, i.e. not adjustable by the user (used by editor)</summary>
        public bool ValueRangeLocked { get; set; }

        /// <summary>True if the Recentering member is valid (bcak-compatibility support:
        /// old versions had recentering in a separate structure)</summary>
        public bool HasRecentering { get; set; }

        /// <summary>Helper for automatic axis recentering</summary>
        [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
        [Serializable]
        public struct Recentering
        {
            /// <summary>If checked, will enable automatic recentering of the
            /// axis. If FALSE, recenting is disabled.</summary>
            [Tooltip("If checked, will enable automatic recentering of the axis. If unchecked, recenting is disabled.")]
            public bool m_enabled;

            /// <summary>If no input has been detected, the camera will wait
            /// this long in seconds before moving its heading to the default heading.</summary>
            [Tooltip("If no user input has been detected on the axis, the axis will wait this long in seconds before recentering.")]
            public float m_WaitTime;

            /// <summary>How long it takes to reach destination once recentering has started</summary>
            [Tooltip("How long it takes to reach destination once recentering has started.")]
            public float m_RecenteringTime;

            float m_LastUpdateTime;

            /// <summary>Constructor with specific field values</summary>
            /// <param name="enabled"></param>
            /// <param name="waitTime"></param>
            /// <param name="recenteringTime"></param>
            public Recentering(bool enabled, float waitTime,  float recenteringTime)
            {
                m_enabled = enabled;
                m_WaitTime = waitTime;
                m_RecenteringTime = recenteringTime;
                mLastAxisInputTime = 0;
                mRecenteringVelocity = 0;
                m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
                m_LastUpdateTime = 0;
            }

            /// <summary>Call this from OnValidate()</summary>
            public void Validate()
            {
                m_WaitTime = Mathf.Max(0, m_WaitTime);
                m_RecenteringTime = Mathf.Max(0, m_RecenteringTime);
            }

            // Internal state
            float mLastAxisInputTime;
            float mRecenteringVelocity;

            /// <summary>
            /// Copy Recentering state from another Recentering component.
            /// </summary>
            /// <param name="other"></param>
            public void CopyStateFrom(ref Recentering other)
            {
                if (mLastAxisInputTime != other.mLastAxisInputTime)
                    other.mRecenteringVelocity = 0;
                mLastAxisInputTime = other.mLastAxisInputTime;
            }

            /// <summary>Cancel any recenetering in progress.</summary>
            public void CancelRecentering()
            {
                mLastAxisInputTime = Time.realtimeSinceStartup;
                mRecenteringVelocity = 0;
            }

            /// <summary>Skip the wait time and start recentering now (only if enabled).</summary>
            public void RecenterNow()
            {
                mLastAxisInputTime = 0;
            }

            /// <summary>Bring the axis back to the centered state (only if enabled).</summary>
            /// <param name="axis">The axis to recenter</param>
            /// <param name="deltaTime">Current effective deltaTime</param>
            /// <param name="recenterTarget">The value that is considered to be centered</param>
            public void DoRecentering(ref AxisState axis, float deltaTime, float recenterTarget)
            {
                // Cheating: we want the render frame time, not the fixed frame time
                if (deltaTime >= 0)
                    deltaTime = Time.realtimeSinceStartup - m_LastUpdateTime;
                
                m_LastUpdateTime = Time.realtimeSinceStartup;
                
                if (!m_enabled && deltaTime >= 0)
                    return;

                recenterTarget = axis.ClampValue(recenterTarget);
                if (deltaTime < 0)
                {
                    CancelRecentering();
                    if (m_enabled)
                        axis.Value = recenterTarget;
                    return;
                }

                float v = axis.ClampValue(axis.Value);
                float delta = recenterTarget - v;
                if (delta == 0)
                    return;

                if (Time.realtimeSinceStartup < (mLastAxisInputTime + m_WaitTime))
                    return;

                // Determine the direction
                float r = axis.m_MaxValue - axis.m_MinValue;
                if (axis.m_Wrap && Mathf.Abs(delta) > r * 0.5f)
                    v += Mathf.Sign(recenterTarget - v) * r;

                // Damp our way there
                if (m_RecenteringTime < 0.001f)
                    v = recenterTarget;
                else
                    v = Mathf.SmoothDamp(
                        v, recenterTarget, ref mRecenteringVelocity,
                        m_RecenteringTime, 9999, deltaTime);
                axis.Value = axis.ClampValue(v);
            }

            // Legacy support
            [SerializeField] [HideInInspector] [FormerlySerializedAs("m_HeadingDefinition")]
            int m_LegacyHeadingDefinition;
            [SerializeField] [HideInInspector] [FormerlySerializedAs("m_VelocityFilterStrength")]
            int m_LegacyVelocityFilterStrength;
            internal bool LegacyUpgrade(ref int heading, ref int velocityFilter)
            {
                if (m_LegacyHeadingDefinition != -1 && m_LegacyVelocityFilterStrength != -1)
                {
                    heading = m_LegacyHeadingDefinition;
                    velocityFilter = m_LegacyVelocityFilterStrength;
                    m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
                    return true;
                }
                return false;
            }
        }
    }
}
