using Godot;
using System;
using VillageProject.Core.Map;

public partial class MapNode : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int x = 0; x < 10; x++)
		for (int y = 0; y < 10; y++)
			CreateTerrainNode(new MapSpot(x, y, 0));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private object CreateTerrainNode(MapSpot spot)
	{
		var newNode = new Sprite2D(); // Create a new Sprite2D.
		AddChild(newNode);
		
		var texture = GD.Load<Texture2D>("res://Assets/Sprites/Dirt.png");
		newNode.Texture = texture;
		
		newNode.Position = new Vector2(spot.X * texture.GetWidth(), spot.Y * texture.GetWidth());
		return newNode;
	}
}
