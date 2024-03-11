using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[System.Serializable]
	public class Room
	{
		public string ID = "";

		public Rect Dimensions;

		[Range(0f, 10f)]
		public float TransitionDuration;

		public EaseType TransitionEaseType;
		public bool ScaleCameraToFit;
		public bool Zoom;

		[Range(0.1f, 10f)]
		public float ZoomScale;

		public int InternalID;

		public Room(Room otherRoom)
		{
			Dimensions = otherRoom.Dimensions;
			TransitionDuration = otherRoom.TransitionDuration;
			TransitionEaseType = otherRoom.TransitionEaseType;
			ScaleCameraToFit = otherRoom.ScaleCameraToFit;
			Zoom = otherRoom.Zoom;
			ZoomScale = otherRoom.ZoomScale;
		}

		public Room()
		{
		}
	}

	[System.Serializable]
	public class RoomEvent : UnityEvent<int, int>
	{
	}

	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-rooms/")]
	public class ProCamera2DRooms : BasePC2D, IPositionOverrider, ISizeOverrider
	{
		public const string ExtensionName = "Rooms";

		public int CurrentRoomIndex
		{
			get { return _currentRoomIndex; }
		}
		private int _currentRoomIndex = -1;

		public int PreviousRoomIndex
		{
			get { return _previousRoomIndex; }
		}
		private int _previousRoomIndex = -1;

		public Room CurrentRoom
		{
			get { return (_currentRoomIndex >= 0 && _currentRoomIndex < Rooms.Count) ? Rooms[_currentRoomIndex] : null; }
		}

		public float OriginalSize { get; private set; }

		public List<Room> Rooms = new List<Room>();

		public float UpdateInterval = .1f;

		public bool UseTargetsMidPoint = true;

		public Transform TriggerTarget;

		public bool TransitionInstanlyOnStart = true;
		public bool RestoreOnRoomExit;
		public float RestoreDuration = 1f;
		public EaseType RestoreEaseType = EaseType.EaseInOut;

		public bool AutomaticRoomActivation = true;
		public bool UseRelativePosition;

		public RoomEvent OnStartedTransition = new RoomEvent();
		public RoomEvent OnFinishedTransition = new RoomEvent();
		public UnityEvent OnExitedAllRooms = new UnityEvent();

		ProCamera2DNumericBoundaries _numericBoundaries;
		NumericBoundariesSettings _defaultNumericBoundariesSettings;

		bool _transitioning;
		Vector3 _newPos;
		float _newSize;

		Coroutine _transitionRoutine;

		private int _currentRoomID = -1;

		override protected void Awake()
		{
			base.Awake();

			_numericBoundaries = ProCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
			_defaultNumericBoundariesSettings = _numericBoundaries.Settings;

			OriginalSize = ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;

			ProCamera2D.AddPositionOverrider(this);
			ProCamera2D.AddSizeOverrider(this);
		}

		void Start()
		{
			var instanceID = GetInstanceID();
			var count = 0;
			foreach (var room in Rooms)
			{
				room.InternalID = instanceID + count;
				count++;
			}
			
			StartCoroutine(TestRoomRoutine());

			if (TransitionInstanlyOnStart)
			{
				var targetPos = ProCamera2D.TargetsMidPoint;
				if (!UseTargetsMidPoint && TriggerTarget != null)
					targetPos = TriggerTarget.position;

				var startingRoom = ComputeCurrentRoom(targetPos);
				if (startingRoom != -1)
					EnterRoom(startingRoom, false);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if(ProCamera2D == null) return;

			ProCamera2D.RemovePositionOverrider(this);
			ProCamera2D.RemoveSizeOverrider(this);
		}

		#region IPositionOverrider implementation

		public Vector3 OverridePosition(float deltaTime, Vector3 originalPosition)
		{
			if (!enabled)
				return originalPosition;

			return _transitioning ? _newPos : originalPosition;
		}

		public int POOrder { get { return _poOrder; } set { _poOrder = value; } }

		int _poOrder = 1001;

		#endregion

		#region ISizeOverrider implementation

		public float OverrideSize(float deltaTime, float originalSize)
		{
			if (!enabled)
				return originalSize;

			return _transitioning ? _newSize : originalSize;
		}

		public int SOOrder { get { return _soOrder; } set { _soOrder = value; } }

		int _soOrder = 3001;

		#endregion

		/// <summary>
		/// Manually test to see if the target(s) is inside a room
		/// </summary>
		public void TestRoom()
		{
			var targetPos = ProCamera2D.TargetsMidPoint;
			if (!UseTargetsMidPoint && TriggerTarget != null)
				targetPos = TriggerTarget.position;

			var roomToEnter = ComputeCurrentRoom(targetPos);
			
			if (roomToEnter != -1 && _currentRoomIndex != roomToEnter)
			{
				EnterRoom(roomToEnter);
			}

			if (roomToEnter == -1 && _currentRoomIndex != -1)
			{
				ExitRoom();
			}
		}

		/// <summary>
		/// Returns what room the target currently is. Useful for when you set the AutomaticRoomActivation to false
		/// </summary>
		/// <returns>The current room</returns>
		/// <param name="targetPos">Target position</param>
		public int ComputeCurrentRoom(Vector3 targetPos)
		{
			int roomToEnter = -1;
			for (int i = 0; i < Rooms.Count; i++)
			{
				if (Utils.IsInsideRectangle(
						Rooms[i].Dimensions.x + (UseRelativePosition ? _transform.position.x : 0),
						Rooms[i].Dimensions.y + (UseRelativePosition ? _transform.position.y : 0),
						Rooms[i].Dimensions.width,
						Rooms[i].Dimensions.height,
						Vector3H(targetPos),
						Vector3V(targetPos)))
				{
					roomToEnter = i;
				}
			}

			return roomToEnter;
		}

		/// <summary>
		/// Enter a room. Only use when the AutomaticRoomActivation is set to false.
		/// </summary>
		/// <param name="roomIndex">The room number on the list</param>
		/// <param name="useTransition">Use a camera movement transition</param>
		/// <param name="forceEntrance">Forces the transition to the room (even if the current room is the same)</param>
		public void EnterRoom(int roomIndex, bool useTransition = true, bool forceEntrance = false)
		{
			if (roomIndex < 0 || roomIndex > Rooms.Count - 1)
				throw new System.Exception("Can't find room with index: " + roomIndex);

			if (!forceEntrance && Rooms[roomIndex].InternalID == _currentRoomID)
				return;

			_previousRoomIndex = _currentRoomIndex;
			_currentRoomIndex = roomIndex;
			
			_currentRoomID = Rooms[roomIndex].InternalID;

			if (OnStartedTransition != null)
				OnStartedTransition.Invoke(roomIndex, _previousRoomIndex);

			TransitionToRoom(Rooms[_currentRoomIndex], useTransition);
		}

		/// <summary>
		/// Enter a room. Only use when the AutomaticRoomActivation is set to false.
		/// </summary>
		/// <param name="roomID">The room ID</param>
		/// <param name="useTransition">Use a camera movement transition</param>
		/// <param name="forceEntrance">Forces the transition to the room (even if the current room is the same)</param>
		public void EnterRoom(string roomID, bool useTransition = true, bool forceEntrance = false)
		{
			var foundIndex = Rooms.FindIndex(room => room.ID == roomID);

			if (foundIndex < 0)
				throw new System.Exception("Can't find room with ID: " + roomID);

			EnterRoom(foundIndex, useTransition, forceEntrance);
		}

		/// <summary>
		/// Exit current room. Only use when the AutomaticRoomActivation is set to false.
		/// </summary>
		public void ExitRoom()
		{
			_currentRoomIndex = -1;
			_currentRoomID = -1;
			if (RestoreOnRoomExit)
			{
				if (OnStartedTransition != null)
					OnStartedTransition.Invoke(_currentRoomIndex, _previousRoomIndex);

				if (_transitionRoutine != null)
					StopCoroutine(_transitionRoutine);

				ProCamera2D.StopUpdateScreenSizeCoroutine();

				_transitionRoutine = StartCoroutine(TransitionRoutine(_defaultNumericBoundariesSettings, OriginalSize, RestoreDuration, RestoreEaseType));
			}

			if (OnExitedAllRooms != null)
				OnExitedAllRooms.Invoke();
		}

		/// <summary>
		/// Add a new room
		/// </summary>
		/// <param name="roomX">Room horizontal position</param>
		/// <param name="roomY">Room vertical position</param>
		/// <param name="roomWidth">Room width</param>
		/// <param name="roomHeight">Room height</param>
		/// <param name="transitionDuration">Transition duration</param>
		/// <param name="transitionEaseType">Transition ease type</param>
		/// <param name="scaleToFit">If set to <c>true</c> the camera will scale to fit the room</param>
		/// <param name="zoom">If set to <c>true</c> the camera will scale to the zoomScale</param>
		/// <param name="zoomScale">Zoom scale</param>
		public void AddRoom(
			float roomX,
			float roomY,
			float roomWidth,
			float roomHeight,
			float transitionDuration = 1f,
			EaseType transitionEaseType = EaseType.EaseInOut,
			bool scaleToFit = false,
			bool zoom = false,
			float zoomScale = 1.5f,
			string id = "")
		{
			var newRoom = new Room()
			{
				ID = id,
				Dimensions = new Rect(roomX, roomY, roomWidth, roomHeight),
				TransitionDuration = transitionDuration,
				TransitionEaseType = transitionEaseType,
				ScaleCameraToFit = scaleToFit,
				Zoom = zoom,
				ZoomScale = zoomScale,
				InternalID = GetInstanceID() + Rooms.Count
			};

			Rooms.Add(newRoom);
		}

		/// <summary>
		/// Removes a specific room from the list.
		/// </summary>
		/// <param name="roomName">Room name / ID</param>
		public void RemoveRoom(string roomName)
		{
			var room = Rooms.Find((Room obj) => obj.ID == roomName);

			if (room != null)
				Rooms.Remove(room);
			else
				Debug.LogWarning(roomName + " not found in the Rooms list.");
		}

		/// <summary>
		/// Sets the default numeric boundaries settings (i.e. The boundaries to use when not inside any room).
		/// </summary>
		/// <param name="settings">The numeric boundaries settings. 
		/// You can grab them from the NumericBoundaries extension, or manually create them</param>
		public void SetDefaultNumericBoundariesSettings(NumericBoundariesSettings settings)
		{
			_defaultNumericBoundariesSettings = settings;
		}

		/// <summary>
		/// Returns a room by its ID. From this you can get all the room properties such as dimensions, zoom, etc.
		/// </summary>
		/// <param name="roomID">The room ID</param>
		public Room GetRoom(string roomID)
		{
			return Rooms.Find((Room obj) => obj.ID == roomID);
		}
		
		/// <summary>
		/// Returns an appropriate camera size for the specified rect representing a room
		/// </summary>
		/// <param name="roomRect">The room rect</param>
		public float GetCameraSizeForRoom(Rect roomRect)
		{
			var scaleFactorW = roomRect.width / ProCamera2D.ScreenSizeInWorldCoordinates.x;
			var scaleFactorH = roomRect.height / ProCamera2D.ScreenSizeInWorldCoordinates.y;

			if (scaleFactorW < scaleFactorH)
				return roomRect.width / ProCamera2D.GameCamera.aspect / 2f;
			else
				return roomRect.height / 2f;
		}

		IEnumerator TestRoomRoutine()
		{
			yield return new WaitForEndOfFrame();

			var waitForSeconds = new WaitForSeconds(UpdateInterval);
			var waitForSecondsRealtime = new WaitForSecondsRealtime(UpdateInterval);
			while (true)
			{
				if (AutomaticRoomActivation)
					TestRoom();

				if(ProCamera2D.IgnoreTimeScale)
					yield return waitForSecondsRealtime;
				else
					yield return waitForSeconds;
			}
		}

		void TransitionToRoom(Room room, bool useTransition = true)
		{
			// Stop any previous transition
			if (_transitionRoutine != null)
				StopCoroutine(_transitionRoutine);
			
			ProCamera2D.StopUpdateScreenSizeCoroutine();

			// Numeric boundaries
			var numericBoundariesSettings = new NumericBoundariesSettings()
			{
				UseNumericBoundaries = true,
				UseTopBoundary = true,
				TopBoundary = (room.Dimensions.y + (UseRelativePosition ? _transform.position.y : 0)) + room.Dimensions.height / 2,
				UseBottomBoundary = true,
				BottomBoundary = (room.Dimensions.y + (UseRelativePosition ? _transform.position.y : 0)) - room.Dimensions.height / 2,
				UseLeftBoundary = true,
				LeftBoundary = (room.Dimensions.x + (UseRelativePosition ? _transform.position.x : 0)) - room.Dimensions.width / 2,
				UseRightBoundary = true,
				RightBoundary = (room.Dimensions.x + (UseRelativePosition ? _transform.position.x : 0)) + room.Dimensions.width / 2
			};

			// Size
			var targetSize = ProCamera2D.ScreenSizeInWorldCoordinates.y / 2f;
			var cameraSizeForRoom = GetCameraSizeForRoom(room.Dimensions);
			if (room.ScaleCameraToFit)
			{
				targetSize = cameraSizeForRoom;
			}
			else if (room.Zoom && OriginalSize * room.ZoomScale <= cameraSizeForRoom)
			{
				targetSize = OriginalSize * room.ZoomScale;
			}
			else if (cameraSizeForRoom < targetSize)
			{
				targetSize = cameraSizeForRoom;
			}

			// Move camera "manually"
			_transitionRoutine = StartCoroutine(TransitionRoutine(numericBoundariesSettings, targetSize, useTransition ? room.TransitionDuration : 0f, room.TransitionEaseType));
		}

		IEnumerator TransitionRoutine(NumericBoundariesSettings numericBoundariesSettings, float targetSize, float transitionDuration = 1f, EaseType transitionEaseType = EaseType.EaseOut)
		{
			_transitioning = true;

			// Disable the current numeric boundaries
			_numericBoundaries.UseNumericBoundaries = false;

			// Size
			var initialSize = ProCamera2D.ScreenSizeInWorldCoordinates.y / 2f;

			//Position
			var initialCamPosH = Vector3H(ProCamera2D.LocalPosition);
			var initialCamPosV = Vector3V(ProCamera2D.LocalPosition);

			// Transition
			var t = 0f;
			while (t <= 1.0f)
			{
				// Prevents a NaN error on some platforms
				if (transitionDuration < float.Epsilon)
					t = 1.1f;
				else if (ProCamera2D.DeltaTime > float.Epsilon)
					t += ProCamera2D.DeltaTime / transitionDuration;

				// Size
				_newSize = transitionDuration > 0 ? Utils.EaseFromTo(initialSize, targetSize, t, transitionEaseType) : targetSize;

				// Position
				var targetPosH = ProCamera2D.CameraTargetPositionSmoothed.x;
				var targetPosV = ProCamera2D.CameraTargetPositionSmoothed.y;

				LimitToNumericBoundaries(
					ref targetPosH,
					ref targetPosV,
					targetSize * ProCamera2D.GameCamera.aspect,
					targetSize,
					numericBoundariesSettings);

				var newPosH = Utils.EaseFromTo(initialCamPosH, targetPosH, t, transitionEaseType);
				var newPosV = Utils.EaseFromTo(initialCamPosV, targetPosV, t, transitionEaseType);
				_newPos = VectorHVD(newPosH, newPosV, 0);

				if(transitionDuration > 0)
					yield return ProCamera2D.GetYield();
			}

			if (transitionDuration == 0)
				yield return ProCamera2D.GetYield();

			if (ProCamera2D.UpdateType == UpdateType.FixedUpdate && ProCamera2D.IgnoreTimeScale)
				yield return new WaitForFixedUpdate();
			
			_transitioning = false;

			_numericBoundaries.Settings = numericBoundariesSettings;

			_transitionRoutine = null;

			if (OnFinishedTransition != null)
				OnFinishedTransition.Invoke(_currentRoomIndex, _previousRoomIndex);

			_previousRoomIndex = _currentRoomIndex;
		}

		void LimitToNumericBoundaries(
			ref float horizontalPos,
			ref float verticalPos,
			float halfCameraWidth,
			float halfCameraHeight,
			NumericBoundariesSettings numericBoundaries)
		{
			if (numericBoundaries.UseLeftBoundary && horizontalPos - halfCameraWidth < numericBoundaries.LeftBoundary)
				horizontalPos = numericBoundaries.LeftBoundary + halfCameraWidth;
			else if (numericBoundaries.UseRightBoundary && horizontalPos + halfCameraWidth > numericBoundaries.RightBoundary)
				horizontalPos = numericBoundaries.RightBoundary - halfCameraWidth;

			if (numericBoundaries.UseBottomBoundary && verticalPos - halfCameraHeight < numericBoundaries.BottomBoundary)
				verticalPos = numericBoundaries.BottomBoundary + halfCameraHeight;
			else if (numericBoundaries.UseTopBoundary && verticalPos + halfCameraHeight > numericBoundaries.TopBoundary)
				verticalPos = numericBoundaries.TopBoundary - halfCameraHeight;
		}

#if UNITY_EDITOR
		override protected void DrawGizmos()
		{
			base.DrawGizmos();

			UnityEditor.Handles.color = EditorPrefsX.GetColor(PrefsData.RoomsColorKey, PrefsData.RoomsColorValue);

			for (int i = 0; i < Rooms.Count; i++)
			{
				// Room border
				var rect = Rooms[i].Dimensions;
				rect.x -= rect.width / 2f - (UseRelativePosition ? transform.position.x : 0);
				rect.y -= rect.height / 2f - (UseRelativePosition ? transform.position.y : 0);
				Vector3[] rectangleCorners =
					{
						VectorHVD(rect.position.x, rect.position.y, 0),                              // Bottom Left
                        VectorHVD(rect.position.x + rect.width, rect.position.y, 0),                 // Bottom Right
                        VectorHVD(rect.position.x + rect.width, rect.position.y + rect.height, 0),   // Top Right
                        VectorHVD(rect.position.x, rect.position.y + rect.height, 0)                 // Top Left
                    };

				UnityEditor.Handles.DrawSolidRectangleWithOutline(rectangleCorners, Color.clear, Color.white);
			}
		}
#endif
	}
}