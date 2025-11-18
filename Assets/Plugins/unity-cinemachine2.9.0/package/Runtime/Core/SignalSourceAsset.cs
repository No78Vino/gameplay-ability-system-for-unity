using UnityEngine;

namespace Cinemachine
{
    /// <summary>Interface for raw signal provider</summary>
    public interface ISignalSource6D
    {
        /// <summary>
        /// Returns the length on seconds of the signal.  
        /// Returns 0 for signals of indeterminate length.
        /// </summary>
        float SignalDuration { get; }

        /// <summary>Get the signal value at a given time relative to signal start</summary>
        /// <param name="timeSinceSignalStart">Time since signal start.  Always >= 0</param>
        /// <param name="pos">output for position component of the signal</param>
        /// <param name="rot">output for rotation component of the signal.  Use Quaternion.identity if none.</param>
        void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot);
    }

    /// <summary>
    /// This is an asset that defines a 6D signal that can be retrieved in a random-access fashion.
    /// This is used by the Cinemachine Impulse module.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.API)]
    public abstract class SignalSourceAsset : ScriptableObject, ISignalSource6D
    {
        /// <summary>
        /// Returns the length on seconds of the signal.  
        /// Returns 0 for signals of indeterminate length.
        /// </summary>
        public abstract float SignalDuration { get; }

        /// <summary>Get the signal value at a given time relative to signal start</summary>
        /// <param name="timeSinceSignalStart">Time since signal start.  Always >= 0</param>
        /// <param name="pos">output for position component of the signal</param>
        /// <param name="rot">output for rotation component of the signal.  Use Quaternion.identity if none.</param>
        public abstract void GetSignal(
            float timeSinceSignalStart, out Vector3 pos, out Quaternion rot);    
    }

}