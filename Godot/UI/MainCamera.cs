using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.Terrain;

public partial class MainCamera : Camera2D
{
	public string CenterMapSpaceId { get; private set; }
	public MapSpot CenterMapSpot { get; private set; }
	public RotationFlag FacingDirection { get; private set; }

	private string _followingMapNodeId;

	private bool _started = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FacingDirection = RotationFlag.North;
		CenterMapSpot = new MapSpot(0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_started)
		{
			_followingMapNodeId = GameMaster.MapControllerNode.GetMainMapNode()?.MapSpace.MapSpaceId;
			CenterViewOnSpot(new MapSpot(), RotationFlag.North);
			_started = true;
		}
	}

	public void FollowMap(string mapNodeId)
	{
		_followingMapNodeId = mapNodeId;
	}

	public void MoveCameraInDirection(DirectionFlag direction)
	{
		var mapNode = GameMaster.MapControllerNode.GetMapNode(_followingMapNodeId);
		if (mapNode == null)
		{
			Console.WriteLine("Tried to move camera, but no map found");
			return;
		}
		Console.WriteLine($"Main Camera moving {direction}.");

		var newSpot = CenterMapSpot.DirectionToSpot(direction, FacingDirection);
		newSpot.X = Math.Max(newSpot.X, mapNode.MapSpace.MinX);
		newSpot.X = Math.Min(newSpot.X, mapNode.MapSpace.MaxX);
		newSpot.Y = Math.Max(newSpot.Y, mapNode.MapSpace.MinY);
		newSpot.Y = Math.Min(newSpot.Y, mapNode.MapSpace.MaxY);
		newSpot.Z = Math.Max(newSpot.Z, mapNode.MapSpace.MinZ);
		newSpot.Z = Math.Min(newSpot.Z, mapNode.MapSpace.MaxZ);
		CenterViewOnSpot(newSpot, FacingDirection);
	}

	public void RotateCamera(int increment, bool clockwise = true)
	{
		var rot = 0;
		if (clockwise)
			rot = (((int)FacingDirection) + increment) % 4;
		else
		{
			rot = ((((int)FacingDirection) + 4) - increment) % 4;
		}
		CenterViewOnSpot(CenterMapSpot, (RotationFlag)rot);
	}
	
	public void CenterViewOnSpot(MapSpot spot, RotationFlag rotation)
	{
		var mapNode = GameMaster.MapControllerNode.GetMapNode(_followingMapNodeId);
		if(mapNode == null)
			return;
		if (rotation != FacingDirection)
		{
			var tempRot = ((int)rotation + 4) % 4;

			FacingDirection = (RotationFlag)tempRot;
			mapNode.RotateMap(FacingDirection);
		}

		CenterMapSpot = spot;
		var screenSize = GetViewport().GetWindow().Size;
		var screenOffset = screenSize / 2;
		var spotPos = mapNode.MapSpotToWorldPos(CenterMapSpot);

		this.Position = spotPos - screenOffset;
		
		mapNode.ShowZLayer(spot.Z);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().Quit();
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.W)
			{
				MoveCameraInDirection(DirectionFlag.Back);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.S)
			{
				MoveCameraInDirection(DirectionFlag.Front);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.A)
			{
				MoveCameraInDirection(DirectionFlag.Left);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.D)
			{
				MoveCameraInDirection(DirectionFlag.Right);
			}
			if (eventKey.Pressed && eventKey.Keycode == Key.Up)
			{
				MoveCameraInDirection(DirectionFlag.Top);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.Down)
			{
				MoveCameraInDirection(DirectionFlag.Bottom);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.E)
			{
				RotateCamera(1, false);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.Q)
			{
				RotateCamera(1, true);
			}
		}

		if (@event is InputEventMouseButton eventMouseButton)
		{
			GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
			
			if(eventMouseButton.IsPressed() && MouseOverSprite.MosueOverSpot.HasValue)
			{ 
				var mapNode = GameMaster.MapControllerNode.GetMouseOverMapNode();
				if(mapNode != null)
					MouseClickedSpot(mapNode.MapSpace, MouseOverSprite.MosueOverSpot.Value + new MapSpot(0,0,1));
			}
		}
	}

	public void MouseClickedSpot(IMapSpace mapSpace, MapSpot spot)
	{
		var pathNode = GameMaster.PathDisplayNode;
		var start = new MapSpot(0, 0, spot.Z);
		Console.WriteLine($"MainCamera Testing Path: {start} to {spot}");
		pathNode.DisplayPath(mapSpace, start, spot);
	}
}
