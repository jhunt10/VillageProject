using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Terrain;

public partial class MainCamera : Camera2D
{
	private MapSpot CenterMapSpot;
	private RotationFlag FacingDirection;

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
			CenterViewOnSpot(new MapSpot(), RotationFlag.North);
			_started = true;
		}
	}

	public void MoveCameraInDirection(DirectionFlags direction)
	{
		var newSpot = CenterMapSpot.DirectionToSpot(direction, FacingDirection);
		newSpot.X = Math.Max(newSpot.X, GameMaster.MapNode.MapSpace.MinX);
		newSpot.X = Math.Min(newSpot.X, GameMaster.MapNode.MapSpace.MaxX);
		newSpot.Y = Math.Max(newSpot.Y, GameMaster.MapNode.MapSpace.MinY);
		newSpot.Y = Math.Min(newSpot.Y, GameMaster.MapNode.MapSpace.MaxY);
		newSpot.Z = Math.Max(newSpot.Z, GameMaster.MapNode.MapSpace.MinZ);
		newSpot.Z = Math.Min(newSpot.Z, GameMaster.MapNode.MapSpace.MaxZ);
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
		if (rotation != FacingDirection)
		{
			var tempRot = ((int)rotation + 4) % 4;

			FacingDirection = (RotationFlag)tempRot;
			GameMaster.MapNode.RotateMap(FacingDirection);
		}

		CenterMapSpot = spot;
		var screenSize = GetViewport().GetWindow().Size;
		var screenOffset = screenSize / 2;
		var spotPos = GameMaster.MapNode.MapSpotToWorldPos(CenterMapSpot);

		this.Position = spotPos - screenOffset;
		
		GameMaster.MapNode.ShowZLayer(spot.Z);
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
				MoveCameraInDirection(DirectionFlags.Back);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.S)
			{
				MoveCameraInDirection(DirectionFlags.Front);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.A)
			{
				MoveCameraInDirection(DirectionFlags.Left);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.D)
			{
				MoveCameraInDirection(DirectionFlags.Right);
			}
			if (eventKey.Pressed && eventKey.Keycode == Key.Up)
			{
				MoveCameraInDirection(DirectionFlags.Top);
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.Down)
			{
				MoveCameraInDirection(DirectionFlags.Bottom);
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
			GameMaster.MapNode.CreateGrassNode(MouseOverSprite.MosueOverSpot + new MapSpot(0,0,1));
		}
			
	}
	
}
