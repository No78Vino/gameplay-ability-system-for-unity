#if PLAYMAKER
using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine;

[Tooltip("Shakes the camera position along its horizontal and vertical axes with the given values")]
public class PC2DShakeWithValues : FsmStateActionProCamera2DBase
{
	[RequiredField]
	[Tooltip("The camera with the ProCamera2D component, most probably the MainCamera")]
	public FsmGameObject MainCamera;

	[Tooltip("The shake strength on each axis")]
	public FsmVector2 Strength;

	[Tooltip("The duration of the shake")]
	public FsmFloat Duration = 1;

	[Tooltip("Indicates how much will the shake vibrate. Don't use values lower than 1 or higher than 100 for better results")]
	public FsmInt Vibrato = 10;

	[Tooltip("Indicates how much random the shake will be")]
	[HasFloatSlider(0, 1)]
	public FsmFloat Randomness = .1f;

	[Tooltip("The initial angle of the shake. Use -1 if you want it to be random.")]
	[HasFloatSlider(-1, 360)]
	public FsmInt InitialAngle = 10;

	[Tooltip("The maximum rotation the camera can reach during shake")]
	public FsmVector3 Rotation;

	[Tooltip("How smooth the shake should be, 0 being instant")]
	[HasFloatSlider(0, .5f)]
	public FsmFloat Smoothness;

	public override void OnEnter()
	{
		var shake = MainCamera.Value.GetComponent<ProCamera2DShake>();

		if (shake == null)
			Debug.LogError("The ProCamera2D component needs to have the Shake plugin enabled.");

		if (ProCamera2D.Instance != null && shake != null)
			shake.Shake(
				Duration.Value,
				Strength.Value,
				Vibrato.Value,
				Randomness.Value,
				InitialAngle.Value,
				Rotation.Value,
				Smoothness.Value);

		Finish();
	}
}
#endif