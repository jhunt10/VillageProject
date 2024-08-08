using Godot;
using System;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites;
using VillageProject.Godot;
using VillageProject.Godot.DefDefs;
using VillageProject.Godot.DefDefs.DefPrefabs;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Sprites;
using Environment = System.Environment;
using Timer = Godot.Timer;

public partial class GameMaster : Node2D
{
	
	public static string PrefabScenePath = @"res://Scenes\Prefabs";
	public static GameMaster Instance { get; private set; }
	public static GodotSpriteHelper SpriteHelper = new GodotSpriteHelper();
	private bool inited = false;
	public static MainCamera MainCamera;
	public static MapControllerNode MapControllerNode;
	public static Dictionary<string, Old_IInstNode> InstNodes = new Dictionary<string, Old_IInstNode>();

	public static PathDisplayNode PathDisplayNode;

	public TextureButton SaveButton;
	public TextureButton LoadButton;
	public TextureButton ClearButton;

	private bool doneTest = false;

	private void DoTest()
	{
		var actorDefName = BehaviorDefs.TestActor.DefName;
		var actorDef = DimMaster.GetDefByName(actorDefName);
		var actorInst = DimMaster.InstantiateDef(actorDef);
		
		// var wanderDefName = BehaviorDefs.WanderBehaviorDef.DefName;
		// var wanderDef = DimMaster.GetDefByName(wanderDefName);
		// var behaviorManager = DimMaster.GetManager<BehaviorManager>();
		// var wanderBehavior = behaviorManager.InstantiateBehavior(wanderDef, actorInst);
		// actorInst.GetComponentOfType<ActorCompInst>()?.TrySetMapPosition(
		// 	new MapPositionData(MapControllerNode.GetMainMapNode().MapSpace,
		// 	new MapSpot(0,0,1), RotationFlag.South));
		//
		var actorInst2 = DimMaster.InstantiateDef(actorDef);
		actorInst2.GetComponentOfType<ActorCompInst>()?.TrySetMapPosition(
			new MapPositionData(MapControllerNode.GetMainMapNode().MapSpace,
				new MapSpot(0,2,1), RotationFlag.South));
		
		var t = true;
	}
	
	public override void _EnterTree()
	{
		if (!inited)
		{
			Instance = this;
			DefWriter.SaveAllDefs();
			SpriteManager.SetSpriteLoader(SpriteHelper);
			DimMaster.StartUp();
			MapControllerNode = GetNode<MapControllerNode>("MapControllerNode");
			inited = true;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		if (MainCamera == null)
		{
			MainCamera = GetNode<MainCamera>("MainCamera");
			if(MainCamera == null)
				Console.WriteLine("Failed to find MainCamera.");
			// MouseOverSprite = GetNode<MouseOverSprite>("MouseOverSprite");
			if(MainCamera == null)
				Console.WriteLine("Failed to find MouseOverSprite.");
			SaveButton = GetNode<TextureButton>("CanvasLayer/SaveButton");
			SaveButton.Pressed += () => SaveGame();
			LoadButton = GetNode<TextureButton>("CanvasLayer/LoadButton");
			LoadButton.Pressed += () => LoadGame();
			ClearButton = GetNode<TextureButton>("CanvasLayer/ClearButton");
			ClearButton.Pressed += () => ClearGame();
			PathDisplayNode = GetNode<PathDisplayNode>("PathDisplayNode");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!doneTest)
		{
			DoTest();
			doneTest = true;
		}
		DimMaster.UpdateGameState((float)delta);
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

	public void ClearGame()
	{
		DimMaster.ClearGameState();
	}

	// public void OnInstLoaded(IInst inst)
	// {
	// 	// Skip terrain to be handled by MapNode
	// 	var terrainComp = inst.GetComponentOfType<TerrainCompInst>(errorIfNull:false);
	// 	if(terrainComp != null)
	// 		return;
	// 	
	// 	Console.WriteLine($"Game Master: Load Saved Inst '{inst._DebugId}'.");
	// 	var mapSpaceComp = inst.GetComponentOfType<MapSpaceCompInst>(errorIfNull:false);
	// 	if (mapSpaceComp != null)
	// 	{
	// 		if (MapControllerNode != null)
	// 		{
	// 			var newMap = MapControllerNode.LoadMap(mapSpaceComp);
	// 			if(newMap != null)
	// 				InstNodes.Add(inst.Id, newMap);
	// 		}
	// 	}
	// 	
	// 	// var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(activeOnly:false, errorIfNull:false);
	// 	// if (mapStructComp != null)
	// 	// {
	// 	// 	if (MapControllerNode != null)
	// 	// 	{
	// 	// 		var newNode = MapControllerNode.CreateNewMapStructureNode(inst);
	// 	// 		if(newNode != null)
	// 	// 			InstNodes.Add(inst.Id, newNode);
	// 	// 		
	// 	// 	}
	// 	// }
	// }
	//
	// public void OnInstDeleted(IInst inst)
	// {
	// 	Console.WriteLine($"GameMaster asked to delete {inst._DebugId}.");
	// 	MapControllerNode.NotifyOfDeletedInst(inst);
	// 	if (InstNodes.ContainsKey(inst.Id))
	// 	{
	// 		var instNode = InstNodes[inst.Id];
	// 		instNode.Delete();
	// 		Console.WriteLine($"\tGameMaster deleted {inst._DebugId}.");
	//
	// 	}
	// 	else
	// 	{
	// 		Console.WriteLine($"\tNo node found for inst {inst._DebugId}.");
	// 	}
	// }

	public static IInstNode CreateInstNodeForInst(InstNodeCompInst instComp)
	{
		var scenePath = Path.Combine(PrefabScenePath, instComp.InstNodeCompDef.PrefabNodeName);
		var prefab = GD.Load<PackedScene>(scenePath);
		var newNode = prefab.Instantiate<Node2D>();
		var instNode = (IInstNode)newNode;
		instNode.SetInst(instComp.Instance);
		Instance.CallDeferred("add_child", (Node2D)instNode);
		return instNode;
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
