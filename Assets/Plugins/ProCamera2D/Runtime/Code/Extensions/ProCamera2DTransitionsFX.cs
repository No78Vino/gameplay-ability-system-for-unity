using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum TransitionsFXShaders
    {
        Fade,
        Circle,
        Shutters,
        Wipe,
        Blinds,
        Texture
    }

    public enum TransitionFXSide
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    }

    public enum TransitionFXDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-transitions-fx/")]
    public class ProCamera2DTransitionsFX : BasePC2D
    {
        public static string ExtensionName = "TransitionsFX";

        /// <summary>Fires whenever a TransitionEnter starts</summary>
        public Action OnTransitionEnterStarted;
        /// <summary>Fires whenever a TransitionEnter ends</summary>
        public Action OnTransitionEnterEnded;

        /// <summary>Fires whenever a TransitionExit starts</summary>
        public Action OnTransitionExitStarted;
        /// <summary>Fires whenever a TransitionExit ends</summary>
        public Action OnTransitionExitEnded;

        /// <summary>Fires whenever a TransitionEnter or a TransitionExit starts</summary>
        public Action OnTransitionStarted;
        /// <summary>Fires whenever a TransitionEnter or a TransitionExit ends</summary>
        public Action OnTransitionEnded;

        static ProCamera2DTransitionsFX _instance;

        public static ProCamera2DTransitionsFX Instance
        {
            get
            {
                if (Equals(_instance, null))
                {
                    _instance = ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>();

                    if (Equals(_instance, null))
                        throw new UnityException("ProCamera2D does not have a TransitionFX extension.");
                }

                return _instance;
            }
        }

        public TransitionsFXShaders TransitionShaderEnter = TransitionsFXShaders.Fade;
        public float DurationEnter = .5f;
        public float DelayEnter = 0f;
        public EaseType EaseTypeEnter = EaseType.EaseOut;
        public Color BackgroundColorEnter = Color.black;
        public TransitionFXSide SideEnter = TransitionFXSide.Left;
        public TransitionFXDirection DirectionEnter = TransitionFXDirection.Horizontal;
        [Range(2, 128)]
        public int BlindsEnter = 16;
        public Texture TextureEnter;
        [Range(0, 1)]
        public float TextureSmoothingEnter = .2f;

        public TransitionsFXShaders TransitionShaderExit = TransitionsFXShaders.Fade;
        public float DurationExit = .5f;
        public float DelayExit = 0f;
        public EaseType EaseTypeExit = EaseType.EaseOut;
        public Color BackgroundColorExit = Color.black;
        public TransitionFXSide SideExit = TransitionFXSide.Left;
        public TransitionFXDirection DirectionExit = TransitionFXDirection.Horizontal;
        [Range(2, 128)]
        public int BlindsExit = 16;
        public Texture TextureExit;
        [Range(0, 1)]
        public float TextureSmoothingExit = .2f;

        public bool StartSceneOnEnterState = true;

        Coroutine _transitionCoroutine;
        float _step;

        Material _transitionEnterMaterial;
        Material _transitionExitMaterial;

        BasicBlit _blit;

        int _material_StepID;
        int _material_BackgroundColorID;

        string _previousEnterShader = "";
        string _previousExitShader = "";

        protected override void Awake()
        {
            base.Awake();

            _instance = this;

            _material_StepID = Shader.PropertyToID("_Step");
            _material_BackgroundColorID = Shader.PropertyToID("_BackgroundColor");

            _blit = gameObject.AddComponent<BasicBlit>();
            _blit.enabled = false;

            UpdateTransitionsShaders();
            UpdateTransitionsProperties();
            UpdateTransitionsColor();

            if (StartSceneOnEnterState)
            {
                _step = 1f;
                _blit.CurrentMaterial = _transitionEnterMaterial;
                _blit.CurrentMaterial.SetFloat(_material_StepID, _step);
                _blit.enabled = true;
            }
        }

        /// <summary>
        /// Transition enter
        /// </summary>
        public void TransitionEnter()
        {
            Transition(_transitionEnterMaterial, DurationEnter, DelayEnter, 1f, 0f, EaseTypeEnter);
        }

        /// <summary>
        /// Transition exit
        /// </summary>
        public void TransitionExit()
        {
            Transition(_transitionExitMaterial, DurationExit, DelayExit, 0f, 1f, EaseTypeExit);
        }

        /// <summary>
        /// Updates the transitions shaders. 
        /// Use only if you change the selected Enter/Exit shaders during runtime.
        /// </summary>
        public void UpdateTransitionsShaders()
        {
            string shaderEnter = TransitionShaderEnter.ToString();
            if (!_previousEnterShader.Equals(shaderEnter))
            {
                _transitionEnterMaterial = new Material(Shader.Find("Hidden/ProCamera2D/TransitionsFX/" + shaderEnter));
                _previousEnterShader = shaderEnter;
            }

            string shaderExit = TransitionShaderExit.ToString();
            if (!_previousExitShader.Equals(shaderExit))
            {
                _transitionExitMaterial = new Material(Shader.Find("Hidden/ProCamera2D/TransitionsFX/" + shaderExit));
                _previousExitShader = shaderExit;
            }
        }

        /// <summary>
        /// Updates the transitions properties.
        /// Use only if you changed the following properties during runtime: Direction, Side, Blinds, Texture, Texture Smoothing.
        /// For updating the background color, use the method UpdateTransitionsColor
        /// </summary>
        public void UpdateTransitionsProperties()
        {
            // Enter
            if (TransitionShaderEnter == TransitionsFXShaders.Wipe || TransitionShaderEnter == TransitionsFXShaders.Blinds)
            {
                _transitionEnterMaterial.SetInt("_Direction", (int)SideEnter);
                _transitionEnterMaterial.SetInt("_Blinds", BlindsEnter);
            }
            else if (TransitionShaderEnter == TransitionsFXShaders.Shutters)
            {
                _transitionEnterMaterial.SetInt("_Direction", (int)DirectionEnter);
            }
            else if (TransitionShaderEnter == TransitionsFXShaders.Texture)
            {
                _transitionEnterMaterial.SetTexture("_TransitionTex", TextureEnter);
                _transitionEnterMaterial.SetFloat("_Smoothing", TextureSmoothingEnter);
            }


            // Exit
            if (TransitionShaderExit == TransitionsFXShaders.Wipe || TransitionShaderExit == TransitionsFXShaders.Blinds)
            {
                _transitionExitMaterial.SetInt("_Direction", (int)SideExit);
                _transitionExitMaterial.SetInt("_Blinds", BlindsExit);
            }
            else if (TransitionShaderExit == TransitionsFXShaders.Shutters)
            {
                _transitionExitMaterial.SetInt("_Direction", (int)DirectionExit);
            }
            else if (TransitionShaderExit == TransitionsFXShaders.Texture)
            {
                _transitionExitMaterial.SetTexture("_TransitionTex", TextureExit);
                _transitionExitMaterial.SetFloat("_Smoothing", TextureSmoothingExit);
            }
        }

        /// <summary>
        /// Updates the transitions color.
        /// Use only if you changed the BackgroundColorEnter or BackgroundColorExit during runtime.
        /// </summary>
        public void UpdateTransitionsColor()
        {
            _transitionEnterMaterial.SetColor(_material_BackgroundColorID, BackgroundColorEnter);
            _transitionExitMaterial.SetColor(_material_BackgroundColorID, BackgroundColorExit);
        }

        /// <summary>
        /// Clears the current transition
        /// </summary>
        public void Clear()
        {
            _blit.enabled = false;
        }

        void Transition(Material material, float duration, float delay, float startValue, float endValue, EaseType easeType)
        {
            if (_transitionEnterMaterial == null)
            {
                Debug.LogWarning("TransitionsFX not initialized yet. You're probably calling TransitionEnter/Exit from an Awake method. Please call it from a Start method instead.");
                return;
            }

            if (_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            _transitionCoroutine = StartCoroutine(TransitionRoutine(material, duration, delay, startValue, endValue, easeType));
        }

        IEnumerator TransitionRoutine(Material material, float duration, float delay, float startValue, float endValue, EaseType easeType)
        {
            _blit.enabled = true;

            _step = startValue;
            _blit.CurrentMaterial = material;
            _blit.CurrentMaterial.SetFloat(_material_StepID, _step);

            if (endValue == 0)
            {
                if (OnTransitionEnterStarted != null)
                    OnTransitionEnterStarted();
            }
            else if (endValue == 1)
            {
                if (OnTransitionExitStarted != null)
                    OnTransitionExitStarted();
            }

            if (OnTransitionStarted != null)
                OnTransitionStarted();

            if (delay > 0)
            {
                if(ProCamera2D.IgnoreTimeScale)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);
            }

            var t = 0f;
            while (t <= 1.0f)
            {
                t += ProCamera2D.DeltaTime / duration;

                _step = Utils.EaseFromTo(startValue, endValue, t, easeType);

                material.SetFloat(_material_StepID, _step);

                yield return null;
            }

            _step = endValue;
            material.SetFloat(_material_StepID, _step);

            if (endValue == 0)
            {
                if (OnTransitionEnterEnded != null)
                    OnTransitionEnterEnded();
            }
            else if (endValue == 1)
            {
                if (OnTransitionExitEnded != null)
                    OnTransitionExitEnded();
            }

            if (OnTransitionEnded != null)
                OnTransitionEnded();

            if(endValue == 0)
                _blit.enabled = false;
        }
    }
}