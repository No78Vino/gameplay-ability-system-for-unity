#if PLAYMAKER
using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine;

[Tooltip("Enter a room when using the Rooms extension")]
public class PC2DRoomsEnter : FsmStateActionProCamera2DBase
{
	[Tooltip("Set the current room by index"), RequiredField]
	public FsmInt RoomIndex;
	[Tooltip("Set the current room by ID. Note that using ID will override index")]
	public FsmString RoomId;
	[Tooltip("If false, the camera instantly transitions to the room. If true, the camera uses the transition configured in the Rooms extension editor.")]
	public bool UseTransition = true;
	ProCamera2DRooms _rooms;

	public override void Reset()
	{
		RoomIndex = 0;
		RoomId = null;
	}

	public override void OnEnter()
	{
		var pc2d = ProCamera2D.Instance;
		if (pc2d == null)
		{
			Debug.LogError("No ProCamera2D found! Please add the core component to your Main Camera.");
			Finish();
			return;
		}

		_rooms = pc2d.GetComponent<ProCamera2DRooms>();
		if (_rooms == null)
		{
			Debug.LogError("No Rooms extension found in ProCamera2D!");
			Finish();
			return;
		}

		SetRoom();
		Finish();
	}

	void SetRoom()
	{
		if (!RoomId.IsNone && !string.IsNullOrEmpty(RoomId.Value))
		{
			_rooms.EnterRoom(RoomId.Value, UseTransition);
		}
		else
		{
			_rooms.EnterRoom(RoomIndex.Value, UseTransition);
		}
	}
}
#endif