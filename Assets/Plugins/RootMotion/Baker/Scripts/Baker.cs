using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RootMotion
{
    /// <summary>
    /// Base class for animation bakers, handles timing, keyframing and saving AnimationClips.
    /// </summary>
    [HelpURL("http://www.root-motion.com/finalikdox/html/page3.html")]
    [AddComponentMenu("Scripts/RootMotion/Baker")]
    public abstract class Baker : MonoBehaviour
    {

        // Open the User Manual URL
        [ContextMenu("User Manual")]
        void OpenUserManual()
        {
            Application.OpenURL("http://www.root-motion.com/finalikdox/html/page3.html");
        }

        // Open the Script Reference URL
        [ContextMenu("Scrpt Reference")]
        void OpenScriptReference()
        {
            Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_baker.html");
        }

        // Link to the Final IK Google Group
        [ContextMenu("Support Group")]
        void SupportGroup()
        {
            Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
        }

        // Link to the Final IK Asset Store thread in the Unity Community
        [ContextMenu("Asset Store Thread")]
        void ASThread()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
        }

        [System.Serializable]
        public enum Mode
        {
            AnimationClips = 0,
            AnimationStates = 1,
            PlayableDirector = 2,
            Realtime = 3
        }

        /// <summary>
        /// In AnimationClips, AnimationStates or PlayableDirector mode - the frame rate at which the animation clip will be sampled. In Realtime mode - the frame rate at which the pose will be sampled. With the latter, the frame rate is not guaranteed if the player is not able to reach it.
        /// </summary>
        [Tooltip("In AnimationClips, AnimationStates or PlayableDirector mode - the frame rate at which the animation clip will be sampled. In Realtime mode - the frame rate at which the pose will be sampled. With the latter, the frame rate is not guaranteed if the player is not able to reach it.")]
        [Range(1, 90)] public int frameRate = 30;

        /// <summary>
        /// Maximum allowed error for keyframe reduction.
        /// </summary>
        [Tooltip("Maximum allowed error for keyframe reduction.")]
        [Range(0f, 0.1f)] public float keyReductionError = 0.01f;

        /// <summary>
        /// AnimationClips mode can be used to bake a batch of AnimationClips directly without the need of setting up an AnimatorController. AnimationStates mode is useful for when you need to set up a more complex rig with layers and AvatarMasks in Mecanim. PlayableDirector mode bakes a Timeline. Realtime mode is for continuous baking of gameplay, ragdoll phsysics or PuppetMaster dynamics.
        /// </summary>
        [Tooltip("AnimationClips mode can be used to bake a batch of AnimationClips directly without the need of setting up an AnimatorController. " +
            "AnimationStates mode is useful for when you need to set up a more complex rig with layers and AvatarMasks in Mecanim. " +
            "PlayableDirector mode bakes a Timeline. " +
            "Realtime mode is for continuous baking of gameplay, ragdoll phsysics or PuppetMaster dynamics.")]
        public Mode mode;

        /// <summary>
        /// AnimationClips to bake.
        /// </summary>
        [Tooltip("AnimationClips to bake.")]
        public AnimationClip[] animationClips = new AnimationClip[0];

        /// <summary>
        /// The name of the AnimationStates to bake (must be on the base layer) in the Animator (Right-click on this component header and select 'Find Animation States' to have Baker fill those in automatically, required that state names match with the names of the clips used in them).
        /// </summary>
        [Tooltip("The name of the AnimationStates to bake (must be on the base layer) in the Animator above (Right-click on this component header and select 'Find Animation States' to have Baker fill those in automatically, required that state names match with the names of the clips used in them).")]
        public string[] animationStates = new string[0];

        /// <summary>
        /// The folder to save the baked AnimationClips to.
        /// </summary>
        [Tooltip("The folder to save the baked AnimationClips to.")]
        public string saveToFolder = "Assets";

        /// <summary>
        /// String that will be added to each clip or animation state name for the saved clip. For example if your animation state/clip names were 'Idle' and 'Walk', then with '_Baked' as Append Name, the Baker will create 'Idle_Baked' and 'Walk_Baked' animation clips.
        /// </summary>
        [Tooltip("String that will be added to each clip or animation state name for the saved clip. For example if your animation state/clip names were 'Idle' and 'Walk', then with '_Baked' as Append Name, the Baker will create 'Idle_Baked' and 'Walk_Baked' animation clips.")]
        public string appendName = "_Baked";

        /// <summary>
        /// Name of the created AnimationClip file.
        /// </summary>
        [Tooltip("Name of the created AnimationClip file.")]
        public string saveName = "Baked Clip";

        public bool isBaking { get; private set; }
        public float bakingProgress { get; private set; }

        [HideInInspector] public Animator animator;
        [HideInInspector] public PlayableDirector director;

        public delegate void BakerDelegate(AnimationClip clip, float time);

        public BakerDelegate OnStartClip;
        public BakerDelegate OnUpdateClip;

        [System.Serializable]
        public class ClipSettings
        {
            [System.Serializable]
            public enum BasedUponRotation
            {
                Original = 0,
                BodyOrientation = 1,
            }

            [System.Serializable]
            public enum BasedUponY
            {
                Original = 0,
                CenterOfMass = 1,
                Feet = 2,
            }

            [System.Serializable]
            public enum BasedUponXZ
            {
                Original = 0,
                CenterOfMass = 1,
            }

            public bool loopTime;
            public bool loopBlend;
            public float cycleOffset;
            public bool loopBlendOrientation;
            public BasedUponRotation basedUponRotation;
            public float orientationOffsetY;
            public bool loopBlendPositionY;
            public BasedUponY basedUponY;
            public float level;
            public bool loopBlendPositionXZ;
            public BasedUponXZ basedUponXZ;
            public bool mirror;

#if UNITY_EDITOR

            public void ApplyTo(AnimationClipSettings settings)
            {
                settings.loopTime = loopTime;
                settings.loopBlend = loopBlend;
                settings.cycleOffset = cycleOffset;

                settings.loopBlendOrientation = loopBlendOrientation;
                settings.keepOriginalOrientation = basedUponRotation == BasedUponRotation.Original;
                settings.orientationOffsetY = orientationOffsetY;

                settings.loopBlendPositionY = loopBlendPositionY;
                settings.keepOriginalPositionY = basedUponY == BasedUponY.Original;
                settings.heightFromFeet = basedUponY == BasedUponY.Feet;
                settings.level = level;

                settings.loopBlendPositionXZ = loopBlendPositionXZ;
                settings.keepOriginalPositionXZ = basedUponXZ == BasedUponXZ.Original;

                settings.mirror = mirror;
            }

#endif
        }

        /// <summary>
        /// If enabled, baked clips will have the same AnimationClipSettings as the clips used for baking. If disabled, clip settings from below will be applied to all the baked clips.
        /// </summary>
        [Tooltip("If enabled, baked clips will have the same AnimationClipSettings as the clips used for baking. If disabled, clip settings from below will be applied to all the baked clips.")] public bool inheritClipSettings;

        /// <summary>
        /// AnimationClipSettings applied to the baked animation clip.
        /// </summary>
        [Tooltip("AnimationClipSettings applied to the baked animation clip.")] public ClipSettings clipSettings;

        protected abstract Transform GetCharacterRoot();
        protected abstract void OnStartBaking();
        protected abstract void OnSetLoopFrame(float time);
        protected abstract void OnSetCurves(ref AnimationClip clip);
        protected abstract void OnSetKeyframes(float time, bool lastFrame);
        protected float clipLength { get; private set; }
        protected bool addLoopFrame;

#if UNITY_EDITOR
        private AnimationClip[] bakedClips = new AnimationClip[0];

        private AnimatorStateInfo currentClipStateInfo;
        private int currentClipIndex;

        private float startBakingTime;
        private float nextKeyframeTime;
        private bool firstFrame;
        private int clipFrames;
        private int clipFrameNo;
        private bool setKeyframes;

        private float currentClipTime;
        private float clipFrameInterval;
#endif

        // Start baking an animation state, clip or timeline, also called for each next clip in the baking array
        public void BakeClip()
        {
#if UNITY_EDITOR
            if (mode == Mode.AnimationClips && inheritClipSettings)
            {
                AnimationClipSettings originalSettings = AnimationUtility.GetAnimationClipSettings(animationClips[currentClipIndex]);
                addLoopFrame = originalSettings.loopTime;
            }
            else
            {
                if (mode == Mode.Realtime) addLoopFrame = clipSettings.loopTime && clipSettings.loopBlend;
                else addLoopFrame = clipSettings.loopTime;
            }

            StartBaking();
#endif
        }

        public void StartBaking()
        {
#if UNITY_EDITOR

            isBaking = true;
            nextKeyframeTime = 0f;

            bakingProgress = 0f;
            clipFrameNo = 0;

            if (bakedClips.Length == 0)
            {
                switch (mode)
                {
                    case Mode.AnimationClips:
                        bakedClips = new AnimationClip[animationClips.Length];
                        break;
                    case Mode.AnimationStates:
                        bakedClips = new AnimationClip[animationStates.Length];
                        break;
                    default:
                        bakedClips = new AnimationClip[1];
                        break;
                }
            }

            OnStartBaking();

            firstFrame = true;

#endif
        }

        public void StopBaking()
        {
#if UNITY_EDITOR

            if (!isBaking) return;

            if (addLoopFrame)
            {
                OnSetLoopFrame(clipLength);
            }

            bakedClips[currentClipIndex] = new AnimationClip();
            bakedClips[currentClipIndex].frameRate = frameRate;

            OnSetCurves(ref bakedClips[currentClipIndex]);

            bakedClips[currentClipIndex].EnsureQuaternionContinuity();

            if (mode == Mode.Realtime)
            {
                isBaking = false;
                SaveClips();
            }

#endif
        }

#if UNITY_EDITOR

        [ContextMenu("Find Animation States")]
        public void FindAnimationStates()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator found on Baker GameObject. Can not find animation states.");
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator does not have a valid Controller. Can not find animation states.");
                return;
            }

            var clips = animator.runtimeAnimatorController.animationClips;
            animationStates = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                animationStates[i] = clips[i].name;
            }
        }

        void Update()
        {
            // Baking procedure
            if (isBaking)
            {
                if (mode != Mode.Realtime)
                {
                    if (firstFrame)
                    {
                        transform.position = Vector3.zero;
                        transform.rotation = Quaternion.identity;
                        GetCharacterRoot().position = Vector3.zero;
                        GetCharacterRoot().rotation = Quaternion.identity;

                        StartAnimationUpdate();

                        currentClipTime = 0f;
                        firstFrame = false;
                    }

                    switch (mode)
                    {
                        case Mode.AnimationClips:
                            clipLength = animationClips[currentClipIndex].length;
                            bakingProgress = currentClipTime / clipLength;
                            break;
                        case Mode.AnimationStates:
                            currentClipStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                            bakingProgress = currentClipStateInfo.normalizedTime;

                            if (currentClipStateInfo.speed <= 0f)
                            {
                                Debug.LogError("Baker can not bake a clip with 0 speed.");
                                return;
                            }

                            clipLength = currentClipStateInfo.length / currentClipStateInfo.speed / animator.speed;
                            break;
                        case Mode.PlayableDirector:
                            clipLength = (float)director.duration;
                            bakingProgress = (float)director.time / clipLength;
                            break;
                    }

                    clipFrames = (int)(clipLength * (frameRate));
                    clipFrameInterval = clipLength / (float)(clipFrames);
                    setKeyframes = true;

                    // Stop clip baking if the clip is finished, start baking the next clip if possible
                    if (clipFrameNo > clipFrames)
                    {
                        StopBaking();

                        currentClipIndex++;
                        if (currentClipIndex >= bakedClips.Length)
                        {
                            currentClipIndex = 0;
                            StopAnimationUpdate();
                            isBaking = false;

                            SaveClips();

                            return;
                        }
                        else
                        {
                            BakeClip();
                            setKeyframes = false;
                        }
                    }

                    if (!firstFrame) AnimationUpdate();
                }
            }
        }

        void LateUpdate()
        {
            if (!isBaking) return;

            if (mode != Mode.Realtime)
            {
                if (setKeyframes)
                {
                    OnSetKeyframes(clipFrameNo * clipFrameInterval, clipFrameNo >= clipFrames);

                    clipFrameNo++;
                    setKeyframes = false;
                }
            }
            else
            {
                if (firstFrame)
                {
                    startBakingTime = Time.time;
                    firstFrame = false;
                }

                if (Time.time < nextKeyframeTime) return;

                OnSetKeyframes(Time.time - startBakingTime, false);

                nextKeyframeTime = Time.time + (1f / (float)frameRate);
            }

        }

        private void StartAnimationUpdate()
        {
            switch (mode)
            {
                case Mode.AnimationClips:
                    if (!AnimationMode.InAnimationMode()) AnimationMode.StartAnimationMode();
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(gameObject, animationClips[currentClipIndex], 0f);
                    AnimationMode.EndSampling();
                    if (OnStartClip != null) OnStartClip(animationClips[currentClipIndex], 0f);
                    break;
                case Mode.AnimationStates:
                    animator.enabled = false;
                    animator.Play(animationStates[currentClipIndex], 0, 0f);
                    break;
                case Mode.PlayableDirector:
                    director.enabled = false;
                    director.time = 0f;
                    director.Evaluate();
                    break;
            }
        }

        private void StopAnimationUpdate()
        {
            switch (mode)
            {
                case Mode.AnimationClips:
                    if (AnimationMode.InAnimationMode()) AnimationMode.StopAnimationMode();
                    break;
                case Mode.AnimationStates:
                    animator.enabled = true;
                    break;
                case Mode.PlayableDirector:
                    director.enabled = true;
                    break;
            }
        }

        private void AnimationUpdate()
        {
            switch (mode)
            {
                case Mode.AnimationClips:
                    if (!AnimationMode.InAnimationMode()) AnimationMode.StartAnimationMode();
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(gameObject, animationClips[currentClipIndex], currentClipTime);
                    AnimationMode.EndSampling();
                    if (OnUpdateClip != null) OnUpdateClip(animationClips[currentClipIndex], currentClipTime / animationClips[currentClipIndex].length);
                    currentClipTime += clipFrameInterval;
                    break;
                case Mode.AnimationStates:
                    animator.Update(clipFrameInterval);
                    break;
                case Mode.PlayableDirector:
                    director.time = currentClipTime;
                    director.Evaluate();
                    currentClipTime += clipFrameInterval;
                    break;
            }
        }

        public void SaveClips()
        {
            var clips = GetBakedClips();
            AnimationClip savedClip = null;

            for (int i = 0; i < clips.Length; i++)
            {
                string path = GetFullPath(i);

                if (mode == Mode.AnimationClips && inheritClipSettings)
                {
                    AnimationClipSettings inheritedSettings = AnimationUtility.GetAnimationClipSettings(animationClips[i]);
                    SetClipSettings(clips[i], inheritedSettings);
                    AnimationUtility.SetAnimationClipSettings(clips[i], inheritedSettings);
                }
                else
                {
                    AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clips[i]);
                    clipSettings.ApplyTo(settings);
                    SetClipSettings(clips[i], settings);
                    AnimationUtility.SetAnimationClipSettings(clips[i], settings);
                }

                var existing = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                if (existing != null)
                {
                    // Overwrite
                    EditorUtility.CopySerialized(clips[i], existing);
                }
                else
                {
                    // Create new asset
                    AssetDatabase.CreateAsset(clips[i], path);
                }

                /* v2.0
                switch (mode)
                {
                    case Baker.Mode.Realtime: break;
                    case Baker.Mode.AnimationClips:
                        AnimationClipSettings originalSettings = AnimationUtility.GetAnimationClipSettings(animationClips[i]);
                        SetClipSettings(clips[i], originalSettings);
                        AnimationUtility.SetAnimationClipSettings(clips[i], originalSettings);
                        break;
                    default:
                        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clips[i]);
                        settings.loopTime = loop;
                        SetClipSettings(clips[i], settings);
                        AnimationUtility.SetAnimationClipSettings(clips[i], settings);
                        break;
                }

                var existing = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                if (existing != null)
                {
                    // Overwrite with existing settings
                    AnimationClipSettings existingSettings = AnimationUtility.GetAnimationClipSettings(existing);
                    existingSettings.stopTime = clips[i].length;
                    AnimationUtility.SetAnimationClipSettings(clips[i], existingSettings);
                    EditorUtility.CopySerialized(clips[i], existing);
                }
                else
                {
                    // Create new asset
                    AssetDatabase.CreateAsset(clips[i], path);
                }
                */

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                savedClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;

                Debug.Log(path + " successfully baked.");
            }

            Selection.activeObject = savedClip;

            ClearBakedClips();
        }

        protected virtual void SetClipSettings(AnimationClip clip, AnimationClipSettings settings) { }

        private string GetFullPath(int clipIndex)
        {
            switch (mode)
            {
                case Baker.Mode.AnimationClips:
                    return saveToFolder + "/" + animationClips[clipIndex].name + appendName + ".anim";
                case Baker.Mode.AnimationStates:
                    return saveToFolder + "/" + animationStates[clipIndex] + appendName + ".anim";
                case Baker.Mode.PlayableDirector:
                    return saveToFolder + "/" + saveName + ".anim";
                default:
                    return saveToFolder + "/" + saveName + ".anim";
            }
        }

        private AnimationClip[] GetBakedClips()
        {
            return bakedClips;
        }

        private void ClearBakedClips()
        {
            bakedClips = new AnimationClip[0];
        }

#endif

    }
}
