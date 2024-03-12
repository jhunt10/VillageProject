using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Godot;
using VillageProject.Godot.DefDefs;
using VillageProject.Godot.DefDefs.DefPrefabs;
using Environment = System.Environment;
using Timer = Godot.Timer;

public partial class GameMaster : Node2D, IInstWatcher
{
	private bool inited = false;
	public static MapNode MapNode;
	public static MainCamera MainCamera;
	public static MapControllerNode MapControllerNode;
	// public static MouseOverSprite MouseOverSprite;
	public static Dictionary<string, IInstNode> InstNodes = new Dictionary<string, IInstNode>();

	public TextureButton SaveButton;
	public TextureButton LoadButton;

	
	public override void _EnterTree()
	{
		if (!inited)
		{
			DimMaster.AddInstWatcher(this);
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
			// MouseOverSprite = GetNode<MouseOverSprite>("MouseOverSprite");
			if(MainCamera == null)
				Console.WriteLine("Failed to find MouseOverSprite.");
			MapControllerNode = GetNode<MapControllerNode>("MapControllerNode");
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
		MapControllerNode.ClearMaps();
		DimMaster.LoadGameState("test_save");
	}

	public void OnNewInstCreated(IInst inst)
	{
		// Skip terrain to be handled by MapNode
		var terrainComp = inst.GetComponentOfType<TerrainCompInst>(errorIfNull:false);
		if(terrainComp != null)
			return;
		
		Console.WriteLine($"Game Master: Create New Inst '{inst._DebugId}'.");
		var mapSpaceComp = inst.GetComponentOfType<MapSpaceCompInst>(errorIfNull:false);
		if (mapSpaceComp != null)
		{
			if(MapControllerNode != null)
				MapControllerNode.LoadMap(mapSpaceComp);
		}
		
		var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:false);
		if (mapStructComp != null)
		{
			if(MapControllerNode != null)
				MapControllerNode.CreateNewMapStructureNode(inst);
		}
	}

	public void OnInstLoaded(IInst inst)
	{
		// Skip terrain to be handled by MapNode
		var terrainComp = inst.GetComponentOfType<TerrainCompInst>(errorIfNull:false);
		if(terrainComp != null)
			return;
		
		Console.WriteLine($"Game Master: Load Saved Inst '{inst._DebugId}'.");
		var mapSpaceComp = inst.GetComponentOfType<MapSpaceCompInst>(errorIfNull:false);
		if (mapSpaceComp != null)
		{
			if(MapControllerNode != null)
				MapControllerNode.LoadMap(mapSpaceComp);
		}
		
		var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:false);
		if (mapStructComp != null)
		{
			if(MapControllerNode != null)
				MapControllerNode.CreateNewMapStructureNode(inst);
		}
	}

	public void OnInstDestoryed(IInst inst)
	{
		throw new NotImplementedException();
	}

	public static Result CreateInstAtSpot(IDef def, IMapSpace mapSpace, MapSpot mapSpot, RotationFlag rotation)
	{
		var inst = DimMaster.InstantiateDef(def);
		
		var mapManager = DimMaster.GetManager<MapManager>();
		var res = mapManager.TryPlaceInstOnMapSpace(mapSpace, inst, mapSpot, rotation);

		if (!res.Success)
			throw new Exception("CreateInstAtSpot Failed");
		return res;
	}
}
