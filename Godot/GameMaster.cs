using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using Environment = System.Environment;
using Timer = Godot.Timer;

public partial class GameMaster : Node2D
{
	private bool inited = false;
	public static MapNode MapNode;
	public static MainCamera MainCamera;
	public static MouseOverSprite MouseOverSprite;
	
	public override void _EnterTree()
	{
		if (!inited)
		{
			DimMaster.StartUp();
			inited = true;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		if (MapNode == null)
		{
			MapNode = (MapNode)FindChild("MapNode");
			if(MapNode == null)
				Console.WriteLine("Failed to find MapNode.");
			MainCamera = GetNode<MainCamera>("MainCamera");
			if(MainCamera == null)
				Console.WriteLine("Failed to find MainCamera.");
			MouseOverSprite = GetNode<MouseOverSprite>("MouseOverSprite");
			if(MainCamera == null)
				Console.WriteLine("Failed to find MouseOverSprite.");

		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
