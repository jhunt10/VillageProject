using Godot;
using System;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Godot.Map;

public partial class MapCellNode : Node2D
{
	public MapNode MapNode;
	public List<IMapObjectNode> MapObjectNodes = new List<IMapObjectNode>();
	public string MapSpaceId { get; set; }
	public MapSpot Spot { get; set; }
	public RotationFlag Rotation { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddMapObjectNode(IMapObjectNode newNode)
	{
		this.AddChild((Node2D)newNode);
		MapObjectNodes.Add(newNode);
		newNode.MapNode = MapNode;
	}

	public void UpdateSprites(IMapSpace mapSpace, MapSpot spot, RotationFlag rotation)
	{
		MapSpaceId = mapSpace.MapSpaceId;
		Spot = spot;
		Rotation = rotation;
		foreach (var objNode in MapObjectNodes)
		{
			objNode.SetViewRotation(rotation);
			objNode.ForceUpdateSprite();
		}
	}
}
