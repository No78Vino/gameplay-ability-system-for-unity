using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-shake/")]
    public class ProCamera2DShake : BasePC2D
    {
        public static string ExtensionName = "Shake";

        static ProCamera2DShake _instance;

        public static ProCamera2DShake Instance
        {
            get
            {
                if (Equals(_instance, null))
                {
                    _instance = FindObjectOfType(typeof(ProCamera2DShake)) as ProCamera2DShake;

                    if (Equals(_instance, null))
                        throw new UnityException("ProCamera2D does not have a Shake extension.");
                }

                return _instance;
            }
        }

        /// <summary>Property to know if there's a ProCamera2DShake present</summary>
        public static bool Exists { get { return _instance != null; } }

        public System.Action OnShakeCompleted;

        public List<ShakePreset> ShakePresets = new List<ShakePreset>();

        public List<ConstantShakePreset> ConstantShakePresets = new List<ConstantShakePreset>();

        public ConstantShakePreset StartConstantShakePreset;

        /// <summary>Used internally by the editor</summary>
        public ConstantShakePreset CurrentConstantShakePreset;

        Transform _shakeParent;

        List<Coroutine> _applyInfluencesCoroutines = new List<Coroutine>();
        List<Coroutine> _shakeTimedCoroutines = new List<Coroutine>();
        Coroutine _shakeCoroutine;

        Vector3 _shakeVelocity;
        List<Vector3> _shakePositions = new List<Vector3>();

        Quaternion _rotationTarget;
        Quaternion _originalRotation;
        float _rotationTime;
        float _rotationVelocity;

        List<Vector3> _influences = new List<Vector3>();
        Vector3 _influencesSum = Vector3.zero;

        Vector3[] _constantShakePositions;
        Vector3 _constantShakePosition;
        bool _isConstantShaking;

        override protected void Awake()
        {
            base.Awake();

            _instance = this;

            if (ProCamera2D.transform.parent != null)
            {
                _shakeParent = new GameObject("ProCamera2DShakeContainer").transform;
                _shakeParent.SetParent(ProCamera2D.transform.parent);
                _shakeParent.localPosition = Vector3.zero;
                ProCamera2D.transform.SetParent(_shakeParent);
            }
            else
            {
                var parent = new GameObject("ProCamera2DShakeContainer").transform;
                ProCamera2D.transform.SetParent(parent);
                _shakeParent = parent;
            }

            _originalRotation = _transform.localRotation;
        }

        void Start()
        {
            if (StartConstantShakePreset != null)
                ConstantShake(StartConstantShakePreset);
        }

        void Update()
        {
            _influencesSum = Vector3.zero;
            if (_influences.Count > 0)
            {
                _influencesSum = Utils.GetVectorsSum(_influences);
                _influences.Clear();

                _shakeParent.localPosition = _influencesSum;
            }
        }

        /// <summary>Shakes the camera with the given values.</summary>
        /// <param name="duration">The duration of the shake</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much random the shake will be</param>
        /// <param name="initialAngle">The initial angle of the shake. Use -1 if you want it to be random.</param>
        /// <param name="rotation">The maximum rotation the camera can reach during shake</param>
        /// <param name="smoothness">How smooth the shake should be, 0 being instant</param>
        /// <param name="ignoreTimeScale">If true, the shake will occur even if the timeScale is 0</param>
        public void Shake(
            float duration,
            Vector2 strength,
            int vibrato = 10,
            float randomness = .1f,
            float initialAngle = -1f,
            Vector3 rotation = default(Vector3),
            float smoothness = .1f,
            bool ignoreTimeScale = false)
        {
            if (!enabled)
                return;

            vibrato++;
            if (vibrato < 2)
                vibrato = 2;

            // Calculate steps durations
            float[] durations = new float[vibrato];
            float sum = 0;
            for (int i = 0; i < vibrato; ++i)
            {
                float iterationPerc = (i + 1) / (float)vibrato;
                float tDuration = duration * iterationPerc;
                sum += tDuration;
                durations[i] = tDuration;
            }
            float tDurationMultiplier = duration / sum;
            for (int i = 0; i < vibrato; ++i)
                durations[i] = durations[i] * tDurationMultiplier;

            float shakeMagnitude = strength.magnitude;
            float magnitudeDecay = shakeMagnitude / vibrato;

            float ang = initialAngle != -1f ? initialAngle : Random.Range(0f, 360f);
            var positions = new Vector2[vibrato];
            positions[vibrato - 1] = Vector2.zero;
            var rotations = new Quaternion[vibrato];
            rotations[vibrato - 1] = _originalRotation;
            var rotationQtn = Quaternion.Euler(rotation);
            for (int i = 0; i < vibrato - 1; ++i)
            {
                // Position
                if (i > 0)
                    ang = ang - 180 + Random.Range(-90, 90) * randomness;

                Quaternion rndQuaternion = Quaternion.AngleAxis(Random.Range(-90, 90) * randomness, Vector3.up);

                float radians = ang * Mathf.Deg2Rad;
                var dir = new Vector3(shakeMagnitude * Mathf.Cos(radians), shakeMagnitude * Mathf.Sin(radians), 0);

                Vector2 position = rndQuaternion * dir;
                position.x = Vector2.ClampMagnitude(position, strength.x).x;
                position.y = Vector2.ClampMagnitude(position, strength.y).y;
                positions[i] = position;

                shakeMagnitude -= magnitudeDecay;
                strength = Vector2.ClampMagnitude(strength, shakeMagnitude);

                // Rotation
                var sign = i % 2 == 0 ? 1 : -1;
                var percent = (float)i / (vibrato - 1);
                rotations[i] = sign == 1 ? Quaternion.Lerp(rotationQtn, Quaternion.identity, percent) * _originalRotation : Quaternion.Inverse(Quaternion.Lerp(rotationQtn, Quaternion.identity, percent)) * _originalRotation;
            }

            _applyInfluencesCoroutines.Add(ApplyShakesTimed(positions, rotations, durations, smoothness, ignoreTimeScale));
        }

        /// <summary>Shakes the camera using the values defined on the provided preset</summary>
        /// <param name="presetIndex">The index of the preset</param>
        public void Shake(int presetIndex)
        {
            if (presetIndex <= ShakePresets.Count - 1)
            {
                Shake(
                    ShakePresets[presetIndex].Duration,
                    ShakePresets[presetIndex].Strength,
                    ShakePresets[presetIndex].Vibrato,
                    ShakePresets[presetIndex].Randomness,
                    ShakePresets[presetIndex].UseRandomInitialAngle ? -1 : ShakePresets[presetIndex].InitialAngle,
                    ShakePresets[presetIndex].Rotation,
                    ShakePresets[presetIndex].Smoothness,
                    ShakePresets[presetIndex].IgnoreTimeScale);
            }
            else
            {
                Debug.LogWarning("Could not find a shake preset with the index: " + presetIndex);
            }
        }

        /// <summary>Shakes the camera using the values defined on the provided preset</summary>
        /// <param name="presetName">The name of the preset</param>
        public void Shake(string presetName)
        {
            for (int i = 0; i < ShakePresets.Count; i++)
            {
                if (ShakePresets[i].name == presetName)
                {
                    Shake(i);

                    return;
                }
            }

            Debug.LogWarning("Could not find a shake preset with the name: " + presetName);
        }

        /// <summary>Shakes the camera using the values defined on the provided preset</summary>
        /// <param name="preset">The shake preset</param>
        public void Shake(ShakePreset preset)
        {
            Shake(preset.Duration,
                  preset.Strength,
                  preset.Vibrato,
                  preset.Randomness,
                  preset.UseRandomInitialAngle ? -1 : preset.InitialAngle,
                  preset.Rotation,
                  preset.Smoothness,
                  preset.IgnoreTimeScale);
        }

        /// <summary>Stops all current shakes</summary>
        public void StopShaking()
        {
            for (int i = 0; i < _applyInfluencesCoroutines.Count; i++)
            {
                StopCoroutine(_applyInfluencesCoroutines[i]);
            }
            
            for (int i = 0; i < _shakeTimedCoroutines.Count; i++)
            {
                StopCoroutine(_shakeTimedCoroutines[i]);
            }
            
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }
            
            _shakePositions.Clear();

            _shakeVelocity = Vector3.zero;

            ShakeCompleted();
        }

        /// <summary>
        /// Constantly shakes the camera until it's explicitly told to stop
        /// </summary>
        /// <param name="preset">The preset that contains all the constant shake parameters</param>
        public void ConstantShake(ConstantShakePreset preset)
        {
            if (CurrentConstantShakePreset != null)
                StopConstantShaking(0);
            
            CurrentConstantShakePreset = preset;

            _isConstantShaking = true;

            _constantShakePositions = new Vector3[preset.Layers.Count];
            
            for (int i = 0; i < preset.Layers.Count; i++)
            {
                StartCoroutine(CalculateConstantShakePosition(
                    i,
                    preset.Layers[i].Frequency.x,
                    preset.Layers[i].Frequency.y,
                    preset.Layers[i].AmplitudeHorizontal,
                    preset.Layers[i].AmplitudeVertical,
                    preset.Layers[i].AmplitudeDepth));
            }

            StartCoroutine(ConstantShakeRoutine(preset.Intensity));
        }

        /// <summary>
        /// Constantly shakes the camera until it's explicitly told to stop
        /// </summary>
        /// <param name="presetName">The name of the preset. It must be part of the ConstantShakePresets list.</param>
        public void ConstantShake(string presetName)
        {
            for (int i = 0; i < ConstantShakePresets.Count; i++)
            {
                if (ConstantShakePresets[i].name == presetName)
                {
                    ConstantShake(ConstantShakePresets[i]);

                    return;
                }
            }

            Debug.LogWarning("Could not find a ConstantShakePreset with the name: " + presetName + ". Remember you need to add it to the ConstantShakePresets list first.");
        }

        /// <summary>
        /// Constantly shakes the camera until it's explicitly told to stop
        /// </summary>
        /// <param name="presetIndex">The index of the preset. It must be part of the ConstantShakePresets list.</param>
        public void ConstantShake(int presetIndex)
        {
            if (presetIndex <= ConstantShakePresets.Count - 1)
            {
                ConstantShake(ConstantShakePresets[presetIndex]);
            }
            else
            {
                Debug.LogWarning("Could not find a ConstantShakePreset with the index: " + presetIndex + ". Remember you need to add it to the ConstantShakePresets list first.");
            }
        }

        /// <summary>
        /// Stops constant shakes
        /// </summary>
        /// <param name="duration">How long it takes to stop the constant shake</param>
        public void StopConstantShaking(float duration = .3f)
        {
            CurrentConstantShakePreset = null;

            _isConstantShaking = false;

            if (duration > 0f)
                StartCoroutine(StopConstantShakeRoutine(duration));
            else
            {
                StopAllCoroutines();
                _constantShakePosition = Vector3.zero;
                _influences.Clear();
                _influences.Add(_constantShakePosition);
            }
        }

        /// <summary>Apply the given influences to the camera during the corresponding durations.</summary>
        /// <param name="shakes">An array of the vectors representing the shakes to be applied</param>
        /// <param name="rotations">An array of the rotations to be applied</param>
        /// <param name="durations">An array with the durations of the influences to be applied</param>
        /// <param name="smoothness">How smooth the shake should be, 0 being instant</param>
        /// <param name="ignoreTimeScale">If true, the shake will occur even if the timeScale is 0</param>
        public Coroutine ApplyShakesTimed(
            Vector2[] shakes,
            Vector3[] rotations,
            float[] durations,
            float smoothness = .1f,
            bool ignoreTimeScale = false)
        {
            if (!enabled)
                return null;

            var rotationsQtn = new Quaternion[rotations.Length];
            for (int i = 0; i < rotations.Length; i++)
                rotationsQtn[i] = Quaternion.Euler(rotations[i]) * _originalRotation;

            return ApplyShakesTimed(shakes, rotationsQtn, durations);
        }

        /// <summary>Apply the given influence to the camera during this frame, while ignoring all camera boundaries</summary>
        /// <param name="influence">The vector representing the influence to be applied</param>
        public void ApplyInfluenceIgnoringBoundaries(Vector2 influence)
        {
            if (Time.deltaTime < .0001f || float.IsNaN(influence.x) || float.IsNaN(influence.y))
                return;

            _influences.Add(VectorHV(influence.x, influence.y));
        }

        Coroutine ApplyShakesTimed(
            Vector2[] shakes,
            Quaternion[] rotations,
            float[] durations,
            float smoothness = .1f,
            bool ignoreTimeScale = false)
        {
            var coroutine = StartCoroutine(ApplyShakesTimedRoutine(shakes, rotations, durations, ignoreTimeScale));

            if (_shakeCoroutine == null)
                _shakeCoroutine = StartCoroutine(ShakeRoutine(smoothness, ignoreTimeScale));

            return coroutine;
        }

        IEnumerator ShakeRoutine(float smoothness, bool ignoreTimeScale = false)
        {
            while (_shakePositions.Count > 0 ||
                   Vector3.Distance(_shakeParent.localPosition, _influencesSum) > .01f ||
                   Quaternion.Angle(_transform.localRotation, _originalRotation) > .01f)
            {
                var newShakePosition = Utils.GetVectorsSum(_shakePositions) + _influencesSum;

                var newShakePositionSmoothed = Vector3.zero;
                if (ignoreTimeScale)
                    newShakePositionSmoothed = Vector3.SmoothDamp(_shakeParent.localPosition, newShakePosition, ref _shakeVelocity, smoothness, float.MaxValue, Time.unscaledDeltaTime);
                else if (ProCamera2D.DeltaTime > 0f)
                    newShakePositionSmoothed = Vector3.SmoothDamp(_shakeParent.localPosition, newShakePosition, ref _shakeVelocity, smoothness);

                _shakeParent.localPosition = newShakePositionSmoothed;
                _shakePositions.Clear();

                if (ignoreTimeScale)
                    _rotationTime = Mathf.SmoothDamp(_rotationTime, 1f, ref _rotationVelocity, smoothness, float.MaxValue, ProCamera2D.UpdateType == UpdateType.LateUpdate ? Time.unscaledDeltaTime : Time.fixedUnscaledDeltaTime);
                else if (ProCamera2D.DeltaTime > 0)
                    _rotationTime = Mathf.SmoothDamp(_rotationTime, 1f, ref _rotationVelocity, smoothness, float.MaxValue, ProCamera2D.DeltaTime);

                var rotationTargetSmoothed = Quaternion.Slerp(_transform.localRotation, _rotationTarget, _rotationTime);

                _transform.localRotation = rotationTargetSmoothed;
                _rotationTarget = _originalRotation;

                yield return ProCamera2D.GetYield();
            }

            ShakeCompleted();
        }

        void ShakeCompleted()
        {
            _shakeParent.localPosition = _influencesSum;
            _transform.localRotation = _originalRotation;
            _shakeCoroutine = null;

            if (OnShakeCompleted != null)
                OnShakeCompleted();
        }

        IEnumerator ApplyShakesTimedRoutine(IList<Vector2> shakes, IList<Quaternion> rotations, float[] durations, bool ignoreTimeScale = false)
        {
            _shakeTimedCoroutines = new List<Coroutine>();
            
            var count = -1;
            while (count < durations.Length - 1)
            {
                count++;
                var duration = durations[count];
                
                var coroutine = StartCoroutine(ApplyShakeTimedRoutine(shakes[count], rotations[count], duration, ignoreTimeScale));
                _shakeTimedCoroutines.Add(coroutine);

                yield return coroutine;
            }
        }

        IEnumerator ApplyShakeTimedRoutine(Vector2 shake, Quaternion rotation, float duration, bool ignoreTimeScale = false)
        {
            _rotationTime = 0;
            _rotationVelocity = 0;
            while (duration > 0)
            {
                if (ignoreTimeScale)
                {
                    if(ProCamera2D.UpdateType == UpdateType.LateUpdate)
                        duration -= Time.unscaledDeltaTime;
                    else if(ProCamera2D.UpdateType == UpdateType.FixedUpdate)
                        duration -= Time.fixedUnscaledDeltaTime;
                }
                else
                    duration -= ProCamera2D.DeltaTime;

                _shakePositions.Add(VectorHV(shake.x, shake.y));

                _rotationTarget = rotation;

                yield return ProCamera2D.GetYield();
            }
        }

        IEnumerator StopConstantShakeRoutine(float duration)
        {
            var velocity = Vector3.zero;
            _influences.Clear();
            while (duration >= 0)
            {
                duration -= ProCamera2D.DeltaTime;

                _constantShakePosition = Vector3.SmoothDamp(_constantShakePosition, Vector3.zero, ref velocity, duration, float.MaxValue);

                _influences.Add(_constantShakePosition);

                yield return ProCamera2D.GetYield();
            }
        }

        IEnumerator CalculateConstantShakePosition(int index, float frequencyMin, float frequencyMax, float amplitudeX, float amplitudeY, float amplitudeZ)
        {
            while (_isConstantShaking)
            {
                var randomFrequency = Random.Range(frequencyMin, frequencyMax);
                var unitSphere = Random.insideUnitSphere;
                var randomAmplitudeX = unitSphere.x * amplitudeX;
                var randomAmplitudeY = unitSphere.y * amplitudeY;
                var randomAmplitudeZ = unitSphere.z * amplitudeZ;

                if(index < _constantShakePositions.Length)
                    _constantShakePositions[index] = VectorHVD(randomAmplitudeX, randomAmplitudeY, randomAmplitudeZ);

                //Debug.DrawLine(transform.localPosition, transform.localPosition + VectorHVD(randomAmplitudeX, randomAmplitudeY, randomAmplitudeZ), Color.green, randomFrequency);

                if(ProCamera2D.IgnoreTimeScale)
                    yield return new WaitForSecondsRealtime(randomFrequency);
                else
                    yield return new WaitForSeconds(randomFrequency);
            }
        }

        IEnumerator ConstantShakeRoutine(float intensity)
        {
            while (_isConstantShaking)
            {
                if (ProCamera2D.DeltaTime > 0)
                {
                    var result = Utils.GetVectorsSum(_constantShakePositions) / _constantShakePositions.Length;
                    _constantShakePosition.Set(Utils.SmoothApproach(_constantShakePosition.x, result.x, result.x, intensity, ProCamera2D.DeltaTime),
                                               Utils.SmoothApproach(_constantShakePosition.y, result.y, result.y, intensity, ProCamera2D.DeltaTime),
                                               Utils.SmoothApproach(_constantShakePosition.z, result.z, result.z, intensity, ProCamera2D.DeltaTime));

                    _influences.Add(_constantShakePosition);
                }

                yield return ProCamera2D.GetYield();
            }
        }

#if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(ProCamera2D.GameCamera, Mathf.Abs(Vector3D(transform.localPosition)));

            if (Application.isPlaying && _shakeParent.localPosition != Vector3.zero)
            {
                Gizmos.color = EditorPrefsX.GetColor(PrefsData.ShakeInfluenceColorKey, PrefsData.ShakeInfluenceColorValue);
                Utils.DrawArrowForGizmo(ProCamera2D.TargetsMidPoint, _shakeParent.localPosition, .04f * cameraDimensions.y);
            }
        }
#endif
    }
}