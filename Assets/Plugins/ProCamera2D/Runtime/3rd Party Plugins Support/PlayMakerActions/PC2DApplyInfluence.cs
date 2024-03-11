#if PLAYMAKER
using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

[Tooltip("Apply the given influence to the camera")]
public class PC2DApplyInfluence : FsmStateActionProCamera2DBase
{
	[Tooltip("The vector representing the influence to be applied")]
	public FsmVector2 Influence;

	public override void OnUpdate()
	{
		if (ProCamera2D.Instance != null)
			ProCamera2D.Instance.ApplyInfluence(Influence.Value);
	}
}
#endif