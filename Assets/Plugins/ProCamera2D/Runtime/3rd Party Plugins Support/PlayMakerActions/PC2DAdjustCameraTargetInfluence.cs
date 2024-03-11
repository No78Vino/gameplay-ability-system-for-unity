#if PLAYMAKER
using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

[Tooltip("Adjusts a target influence")]
public class PC2DAdjustCameraTargetInfluence : FsmStateActionProCamera2DBase
{
	[RequiredField]
	[Tooltip("The Transform of the target")]
	public FsmGameObject target;

	[HasFloatSlider(0, 1)]
	[Tooltip("The influence this target horizontal position should have when calculating the average position of all the targets")]
	public FsmFloat targetInfluenceH = 1;

	[HasFloatSlider(0, 1)]
	[Tooltip("The influence this target vertical position should have when calculating the average position of all the targets")]
	public FsmFloat targetInfluenceV = 1;

	[Tooltip("The time it takes for this target to reach it's influence")]
	public FsmFloat duration = 0;

	public override void OnEnter()
	{
		if (ProCamera2D.Instance != null && target.Value)
			ProCamera2D.Instance.AdjustCameraTargetInfluence(target.Value.transform, targetInfluenceH.Value, targetInfluenceV.Value, duration.Value);

		Finish();
	}
}
#endif