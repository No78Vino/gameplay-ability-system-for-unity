using UnityEngine;

namespace Cinemachine.Utility
{
    /// <summary>
    /// This is a utility class to implement position predicting.
    /// </summary>
    public class PositionPredictor
    {
        Vector3 m_Velocity;
        Vector3 m_SmoothDampVelocity;
        Vector3 m_Pos;
        bool m_HavePos;

        /// <summary>
        /// How much to smooth the predicted result.  Must be >= 0, roughly coresponds to smoothing time.
        /// </summary>
        public float Smoothing;

        /// <summary>
        /// Have any positions been logged for smoothing?
        /// </summary>
        /// <returns>True if no positions have yet been logged, in which case smoothing is impossible</returns>
        public bool IsEmpty() { return !m_HavePos; }

        /// <summary>
        /// Apply a delta to the target's position, which will be ignored for 
        /// smoothing purposes.  Use this whent he target's position gets warped.
        /// </summary>
        /// <param name="positionDelta">The position change of the target object</param>
        public void ApplyTransformDelta(Vector3 positionDelta) { m_Pos += positionDelta; }

        /// <summary>Reset the lookahead data, clear all the buffers.</summary>
        public void Reset() 
        { 
            m_HavePos = false; 
            m_SmoothDampVelocity = Vector3.zero; 
            m_Velocity = Vector3.zero;
        }

        /// <summary>Add a new target position to the history buffer</summary>
        /// <param name="pos">The new target position</param>
        /// <param name="deltaTime">deltaTime since the last target position was added</param>
        /// <param name="lookaheadTime">Current lookahead time setting (unused)</param>
        public void AddPosition(Vector3 pos, float deltaTime, float lookaheadTime)
        {
            if (deltaTime < 0)
                Reset();
            if (m_HavePos && deltaTime > UnityVectorExtensions.Epsilon)
            {
                var vel = (pos - m_Pos) / deltaTime;
                bool slowing = vel.sqrMagnitude < m_Velocity.sqrMagnitude;
                m_Velocity = Vector3.SmoothDamp(
                    m_Velocity, vel, ref m_SmoothDampVelocity, Smoothing / (slowing ? 30 : 10), 
                    float.PositiveInfinity, deltaTime);
            }
            m_Pos = pos;
            m_HavePos = true;
        }

        /// <summary>Predict the target's position change over a given time from now</summary>
        /// <param name="lookaheadTime">How far ahead in time to predict</param>
        /// <returns>The predicted position change (current velocity * lokahead time)</returns>
        public Vector3 PredictPositionDelta(float lookaheadTime)
        {
            return m_Velocity * lookaheadTime;
        }

        /// <summary>Predict the target's position a given time from now</summary>
        /// <param name="lookaheadTime">How far ahead in time to predict</param>
        /// <returns>The predicted position</returns>
        public Vector3 PredictPosition(float lookaheadTime)
        {
            return m_Pos + PredictPositionDelta(lookaheadTime);
        }
    }

    /// <summary>Utility to perform realistic damping of float or Vector3 values.
    /// The algorithm is based on exponentially decaying the delta until only
    /// a negligible amount remains.</summary>
    public static class Damper
    {
        const float Epsilon = UnityVectorExtensions.Epsilon;

        // Get the decay constant that would leave a given residual after a given time
        static float DecayConstant(float time, float residual)
        {
            return Mathf.Log(1f / residual) / time;
        }

        // Exponential decay: decay a given quantity opver a period of time
        static float DecayedRemainder(float initial, float decayConstant, float deltaTime)
        {
            return initial / Mathf.Exp(decayConstant * deltaTime);
        }

        /// <summary>Standard residual</summary>
        public const float kNegligibleResidual = 0.01f;
        const float kLogNegligibleResidual = -4.605170186f; // == math.Log(kNegligibleResidual=0.01f);

        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static float Damp(float initial, float dampTime, float deltaTime)
        {
            if (dampTime < Epsilon || Mathf.Abs(initial) < Epsilon)
                return initial;
            if (deltaTime < Epsilon)
                return 0;
            float k = -kLogNegligibleResidual / dampTime; //DecayConstant(dampTime, kNegligibleResidual);

#if CINEMACHINE_EXPERIMENTAL_DAMPING
            // Try to reduce damage caused by frametime variability
            float step = Time.fixedDeltaTime;
            if (deltaTime != step)
                step /= 5;
            int numSteps = Mathf.FloorToInt(deltaTime / step);
            float vel = initial * step / deltaTime;
            float decayConstant = Mathf.Exp(-k * step);
            float r = 0;
            for (int i = 0; i < numSteps; ++i)
                r = (r + vel) * decayConstant;
            float d = deltaTime - (step * numSteps);
            if (d > Epsilon)
                r = Mathf.Lerp(r, (r + vel) * decayConstant, d / step);
            return initial - r;
#else
            return initial * (1 - Mathf.Exp(-k * deltaTime));
#endif
        }

        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static Vector3 Damp(Vector3 initial, Vector3 dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime[i], deltaTime);
            return initial;
        }

        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static Vector3 Damp(Vector3 initial, float dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime, deltaTime);
            return initial;
        }
    }

    /// <summary>Tracks an object's velocity with a filter to determine a reasonably
    /// steady direction for the object's current trajectory.</summary>
    public class HeadingTracker
    {
        struct Item
        {
            public Vector3 velocity;
            public float weight;
            public float time;
        };
        Item[] mHistory;
        int mTop;
        int mBottom;
        int mCount;

        Vector3 mHeadingSum;
        float mWeightSum = 0;
        float mWeightTime = 0;

        Vector3 mLastGoodHeading = Vector3.zero;

        /// <summary>Construct a heading tracker with a given filter size</summary>
        /// <param name="filterSize">The size of the filter.  The larger the filter, the
        /// more constanct (and laggy) is the heading.  30 is pretty big.</param>
        public HeadingTracker(int filterSize)
        {
            mHistory = new Item[filterSize];
            float historyHalfLife = filterSize / 5f; // somewhat arbitrarily
            mDecayExponent = -Mathf.Log(2f) / historyHalfLife;
            ClearHistory();
        }

        /// <summary>Get the current filter size</summary>
        public int FilterSize { get { return mHistory.Length; } }

        void ClearHistory()
        {
            mTop = mBottom = mCount = 0;
            mWeightSum = 0;
            mHeadingSum = Vector3.zero;
        }

        static float mDecayExponent;
        static float Decay(float time) { return Mathf.Exp(time * mDecayExponent); }

        /// <summary>Add a new velocity frame.  This should be called once per frame,
        /// unless the velocity is zero</summary>
        /// <param name="velocity">The object's velocity this frame</param>
        public void Add(Vector3 velocity)
        {
            if (FilterSize == 0)
            {
                mLastGoodHeading = velocity;
                return;
            }
            float weight = velocity.magnitude;
            if (weight > UnityVectorExtensions.Epsilon)
            {
                Item item = new Item();
                item.velocity = velocity;
                item.weight = weight;
                item.time = CinemachineCore.CurrentTime;
                if (mCount == FilterSize)
                    PopBottom();
                ++mCount;
                mHistory[mTop] = item;
                if (++mTop == FilterSize)
                    mTop = 0;

                mWeightSum *= Decay(item.time - mWeightTime);
                mWeightTime = item.time;
                mWeightSum += weight;
                mHeadingSum += item.velocity;
            }
        }

        void PopBottom()
        {
            if (mCount > 0)
            {
                float time = CinemachineCore.CurrentTime;
                Item item = mHistory[mBottom];
                if (++mBottom == FilterSize)
                    mBottom = 0;
                --mCount;

                float decay = Decay(time - item.time);
                mWeightSum -= item.weight * decay;
                mHeadingSum -= item.velocity * decay;
                if (mWeightSum <= UnityVectorExtensions.Epsilon || mCount == 0)
                    ClearHistory();
            }
        }

        /// <summary>Decay the history.  This should be called every frame.</summary>
        public void DecayHistory()
        {
            float time = CinemachineCore.CurrentTime;
            float decay = Decay(time - mWeightTime);
            mWeightSum *= decay;
            mWeightTime = time;
            if (mWeightSum < UnityVectorExtensions.Epsilon)
                ClearHistory();
            else
                mHeadingSum = mHeadingSum * decay;
        }

        /// <summary>Get the filtered heading.</summary>
        /// <returns>The filtered direction of motion</returns>
        public Vector3 GetReliableHeading()
        {
            // Update Last Good Heading
            if (mWeightSum > UnityVectorExtensions.Epsilon
                && (mCount == mHistory.Length || mLastGoodHeading.AlmostZero()))
            {
                Vector3  h = mHeadingSum / mWeightSum;
                if (!h.AlmostZero())
                    mLastGoodHeading = h.normalized;
            }
            return mLastGoodHeading;
        }
    }
}
