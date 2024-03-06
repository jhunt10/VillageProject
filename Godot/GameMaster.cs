using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map;
using VillageProject.Godot.DefDefs;
using Environment = System.Environment;
using Timer = Godot.Timer;

public partial class GameMaster : Node2D
{
	private bool inited = false;
	public static MapNode MapNode;
	public static MainCamera MainCamera;
	public static MouseOverSprite MouseOverSprite;

	public TextureButton SaveButton;
	public TextureButton LoadButton;
	
	public override void _EnterTree()
	{
		if (!inited)
		{
			DefWriter.SaveAllDefs();
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

			SaveButton = GetNode<TextureButton>("CanvasLayer/SaveButton");
			SaveButton.Pressed += () => SaveGame();
			LoadButton = GetNode<TextureButton>("CanvasLayer/LoadButton");
			LoadButton.Pressed += () => LoadGame();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SaveGame()
	{
		DimMaster.SaveGameState("test_save");
	}
	public void LoadGame()
	{
		MapNode.ClearMap();
		DimMaster.LoadGameState("test_save");
		var mapManager = DimMaster.GetManager<MapManager>();
		MapNode.LoadMap(mapManager.GetMainMapSpace());
	}
}
