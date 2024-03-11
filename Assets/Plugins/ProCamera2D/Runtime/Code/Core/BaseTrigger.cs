using System;
using System.Collections;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	abstract public class BaseTrigger : BasePC2D
	{
		public Action OnEnteredTrigger;
		public Action OnExitedTrigger;

		[TooltipAttribute("Every X seconds detect collision. Smaller intervals are more precise but also require more processing.")]
		public float UpdateInterval = .1f;

		public TriggerShape TriggerShape;

		[TooltipAttribute("If enabled, use the targets mid point to know when inside/outside the trigger.")]
		public bool UseTargetsMidPoint = true;
		[TooltipAttribute("If UseTargetsMidPoint is disabled, use this transform to know when inside/outside the trigger.")]
		public Transform TriggerTarget;

		protected float _exclusiveInfluencePercentage;

		Coroutine _testTriggerRoutine;

		protected bool _insideTrigger;
		protected Vector2 _vectorFromPointToCenter;

		protected int _instanceID;

		bool _triggerEnabled;

		protected override void Awake()
		{
			base.Awake();

			if (ProCamera2D == null)
				return;

			_instanceID = GetInstanceID();

			// Small random time offset to avoid having all the triggers calculatations on the same frame
			UpdateInterval += UnityEngine.Random.Range(-.02f, .02f);

			// Start update routine
			Toggle(true);
		}

		override protected void OnEnable()
		{
			base.OnEnable();

			if(ProCamera2D == null)
				return;

			if (_triggerEnabled)
				Toggle(true);
		}

		override protected void OnDisable()
		{
			base.OnDisable();

			_testTriggerRoutine = null;
		}

		/// <summary>Manually enable or disable the trigger</summary>
		/// <param name="value">If true it will enable the trigger. If false it will disable it.</param>
		public void Toggle(bool value)
		{
			if (value)
			{
				if (_testTriggerRoutine == null)
					_testTriggerRoutine = StartCoroutine(TestTriggerRoutine());

				_triggerEnabled = true;
			}
			else
			{
				if (_testTriggerRoutine != null)
				{
					StopCoroutine(_testTriggerRoutine);
					_testTriggerRoutine = null;
				}

				if (_insideTrigger)
					ExitedTrigger();

				_triggerEnabled = false;
			}
		}

		/// <summary>Manually force the trigger to test if the target(s) is inside it</summary>
		public void TestTrigger()
		{
			var triggerPos = ProCamera2D.TargetsMidPoint;
			if (!UseTargetsMidPoint && TriggerTarget != null)
				triggerPos = TriggerTarget.position;

			if (TriggerShape == TriggerShape.RECTANGLE &&
				Utils.IsInsideRectangle(
					Vector3H(_transform.position),
					Vector3V(_transform.position),
					Vector3H(_transform.localScale),
					Vector3V(_transform.localScale),
					Vector3H(triggerPos),
					Vector3V(triggerPos)))
			{
				if (!_insideTrigger)
					EnteredTrigger();
			}
			else if (TriggerShape == TriggerShape.CIRCLE &&
				Utils.IsInsideCircle(
					Vector3H(_transform.position),
					Vector3V(_transform.position),
					(Vector3H(_transform.localScale) + Vector3V(_transform.localScale)) * .25f,
					Vector3H(triggerPos),
					Vector3V(triggerPos)))
			{
				if (!_insideTrigger)
					EnteredTrigger();
			}
			else
			{
				if (_insideTrigger)
					ExitedTrigger();
			}
		}

		protected virtual void EnteredTrigger()
		{
			_insideTrigger = true;

			if (OnEnteredTrigger != null)
				OnEnteredTrigger();
		}

		protected virtual void ExitedTrigger()
		{
			_insideTrigger = false;

			if (OnExitedTrigger != null)
				OnExitedTrigger();
		}

		IEnumerator TestTriggerRoutine()
		{
			yield return new WaitForEndOfFrame();

			var waitForSeconds = new WaitForSeconds(UpdateInterval);
			var waitForSecondsRealtime = new WaitForSecondsRealtime(UpdateInterval);
			while (true)
			{
				TestTrigger();
				
				if(ProCamera2D.IgnoreTimeScale)
					yield return waitForSecondsRealtime;
				else
					yield return waitForSeconds;
			}
		}

		protected float GetDistanceToCenterPercentage(Vector2 point)
		{
			_vectorFromPointToCenter = point - new Vector2(Vector3H(_transform.position), Vector3V(_transform.position));
			if (TriggerShape == TriggerShape.RECTANGLE)
			{
				var distancePercentageH = Vector3H(_vectorFromPointToCenter) / (Vector3H(_transform.localScale) * .5f);
				var distancePercentageV = Vector3V(_vectorFromPointToCenter) / (Vector3V(_transform.localScale) * .5f);
				var distancePercentage = (Mathf.Max(Mathf.Abs(distancePercentageH), Mathf.Abs(distancePercentageV))).Remap(_exclusiveInfluencePercentage, 1, 0, 1);
				return distancePercentage;
			}
			else
			{
				var distancePercentage = (_vectorFromPointToCenter.magnitude / ((Vector3H(_transform.localScale) + Vector3V(_transform.localScale)) * .25f)).Remap(_exclusiveInfluencePercentage, 1, 0, 1);
				return distancePercentage;
			}
		}

#if UNITY_EDITOR
		override protected void DrawGizmos()
		{
			float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);
			var cameraCenter = VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset);

			Gizmos.color = EditorPrefsX.GetColor(PrefsData.TriggerShapeColorKey, PrefsData.TriggerShapeColorValue);

			if (TriggerShape == TriggerShape.RECTANGLE)
			{
				Gizmos.DrawWireCube(cameraCenter, VectorHVD(Vector3H(transform.localScale), Vector3V(transform.localScale), 0f));

				if (_exclusiveInfluencePercentage > 0)
					Gizmos.DrawWireCube(cameraCenter, VectorHVD(Vector3H(transform.localScale) * _exclusiveInfluencePercentage, Vector3V(transform.localScale) * _exclusiveInfluencePercentage, 0f));
			}
			else
			{
				var axis = Vector3.zero;
				switch (ProCamera2D.Axis)
				{
					case MovementAxis.XY:
						axis = new Vector3(1f, 1f, 0f);
						break;

					case MovementAxis.XZ:
						axis = new Vector3(1f, 0f, 1f);
						break;

					case MovementAxis.YZ:
						axis = new Vector3(0f, 1f, 1f);
						break;
				}

				Gizmos.matrix = Matrix4x4.TRS(cameraCenter, Quaternion.identity, axis);
				Gizmos.DrawWireSphere(Vector3.zero, ((Vector3H(transform.localScale) + Vector3V(transform.localScale)) * .25f));

				if (_exclusiveInfluencePercentage > 0)
					Gizmos.DrawWireSphere(Vector3.zero, ((Vector3H(transform.localScale) + Vector3V(transform.localScale)) * .25f) * _exclusiveInfluencePercentage);

				Gizmos.matrix = Matrix4x4.identity;
			}
		}
#endif
	}

	public enum TriggerShape
	{
		CIRCLE,
		RECTANGLE
	}
}