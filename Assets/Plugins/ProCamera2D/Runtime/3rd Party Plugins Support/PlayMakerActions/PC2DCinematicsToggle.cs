#if PLAYMAKER
using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine;

[Tooltip("Starts or stops a cinematic")]
public class PC2DCinematicsToggle : FsmStateActionProCamera2DBase
{
	[RequiredField]
	[Tooltip("The gameObject that contains the ProCamera2DCinematics component")]
	public FsmGameObject Cinematics;

	public override void OnEnter()
	{
		var cinematics = Cinematics.Value.GetComponent<ProCamera2DCinematics>();

		if (cinematics == null)
			Debug.LogError("No Cinematics component found in the gameObject: " + Cinematics.Value.name);

		if (ProCamera2D.Instance != null && cinematics != null)
			cinematics.Toggle();

		Finish();
	}
}
#endif