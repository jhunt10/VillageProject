using static Godot.GD;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapGeneration;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class MapNode : Node2D
{
	public const int TILE_WIDTH = 32;
	public const int TILE_HIGHT = 40;
	private TerrainManager TerrainManager;

	
	public MapSpace MapSpace;
	public RotationFlag ViewRotation;
	public int VisibleZLayer = 0;

	// private Dictionary<MapSpot, Node2D> TerrainNodes = new Dictionary<MapSpot, Node2D>();
	private Dictionary<int, ZLayerPrefab> ZLayers = new Dictionary<int, ZLayerPrefab>();
	// private Dictionary<int, Sprite2D> ZLayerShadows = new Dictionary<int, Sprite2D>();
	// private Dictionary<int, Node2D> ZTerrainShadows = new Dictionary<int, Node2D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var mapManager = DimMaster.GetManager<MapManager>();
		mapManager.SetMainMapSpace(BasicMapGenerator.GenerateTestMap());
		LoadMap(mapManager.GetMainMapSpace());

			
		
		// ShowZLayer(0);
		
	}

	public void ClearMap()
	{
		foreach (var layer in ZLayers.Values)
		{
			this.RemoveChild(layer);
			layer.QueueFree();
		}
		ZLayers.Clear();
		MapSpace = null;
	}

	public void LoadMap(MapSpace mapSpace)
	{
		MapSpace = mapSpace;
		foreach (var spot in MapSpace.EnumerateMapSpots().OrderBy(x => -x.Y))
		{
			CreateMapNode(spot);
		}
		RotateMap(RotationFlag.North);
	}

	public void ShowZLayer(int zLayer)
	{
		if(!ZLayers.Any())
			return;
		
		var maxZ = ZLayers.Keys.Max();
		var minZ = ZLayers.Keys.Min();
		VisibleZLayer = zLayer;
		if (zLayer > maxZ)
			VisibleZLayer = maxZ;
		if (VisibleZLayer < minZ)
			VisibleZLayer = minZ;

		for (int z = minZ; z <= maxZ; z++)
			if (ZLayers.ContainsKey(z))
			{
				ZLayers[z].SetShow(z <= VisibleZLayer, z == VisibleZLayer + 1);
			}
	}

	public void RotateMap(RotationFlag rotation)
	{
		ViewRotation = (RotationFlag)(((int)rotation) % 4);
		if (ViewRotation < 0)
			ViewRotation = 0;
		Console.WriteLine($"Rotate Map: {ViewRotation}");
		foreach (var layer in ZLayers)
		{
			layer.Value.ResyncRotation();
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouseOverSpot = GetMouseOverMapSpot();
		var mouseOverPos = MapSpotToWorldPos(mouseOverSpot);
		
	}

	public MapSpot GetMouseOverMapSpot()
	{
		var mainCamera = GameMaster.MainCamera;
		if (mainCamera == null)
			return new MapSpot(0, 0, 0);

		var mousePos = GetViewport().GetMousePosition();
		var relativePos = mousePos + mainCamera.Position;
		var mapSpot = MapHelper.WorldPositionToMapSpot(
			MapSpace, (int)relativePos.X, (int)relativePos.Y,
			VisibleZLayer, ViewRotation);
		if(mapSpot.HasValue)
			return mapSpot.Value;
		return new MapSpot();
	}

	public Vector2 MapSpotToWorldPos(MapSpot spot)
	{
		var pos = MapHelper.MapSpotToWorldPosition(MapSpace, spot, ViewRotation, 
			TILE_WIDTH, TILE_WIDTH, TILE_HIGHT);
		return new Vector2(pos[0], pos[1]);
	}



	private void CreateMapNode(MapSpot spot)
	{
		if(!ZLayers.ContainsKey(spot.Z))
			CreateZLayer(spot.Z);
		var insts = MapSpace.ListInstsAtSpot(spot).ToList();
		if (!insts.Any())
		{
			var newNode = ZLayers[spot.Z].CreateEmptyNode(spot);
			return;
		}
		foreach (var inst in MapSpace.ListInstsAtSpot(spot))
		{
			var terrainComp = inst.GetComponentOfType<TerrainCompInst>();
			if (terrainComp != null)
			{
				var newNode = ZLayers[spot.Z].CreateTerrainNode(spot, inst);
				break;
			}

			var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
			if (mapStructComp != null)
			{
				var def = inst.Def;
				var newNode = CreateMapStructNode(spot, inst);
			}
		}
	}
	
	private void CreateZLayer(int z)
	{
		var prefab = GetNode("ZLayerPrefab");

		var newLayer = (ZLayerPrefab)prefab.Duplicate();
		newLayer.MapNode = this;
		newLayer.Position = new Vector2(0, -40 * z);
		ZLayers.Add(z, newLayer);
		this.AddChild(newLayer);
		var zLayers = ZLayers.OrderBy(x => x.Key).Select(x => x.Value);
		foreach (var layer in zLayers)
		{
			RemoveChild(layer);
			AddChild(layer);
		}
	}

	private Sprite2D CreateZLayerShadow()
	{
		var size = this.GetViewport().GetWindow().Size;
		size.Y += TILE_HIGHT;
		var image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		for(int x = 0; x < size.X; x++)
			for(int y = 0; y < size.Y; y++)
				image.SetPixel(x, y, new Color(0,0,0,(float)0.1));
		var newNode = new Sprite2D();
		newNode.Texture = ImageTexture.CreateFromImage(image);
		newNode.Centered = false;
		return newNode;
	}


	public Node2D GetMapNodeAtSpot(MapSpot spot)
	{
		if (ZLayers.ContainsKey(spot.Z))
			return ZLayers[spot.Z].GetCellNode(spot);
		return null;
	}

	public Node2D CreateMapStructNode(MapSpot spot, IInst inst)
	{
		
		var mapManager = DimMaster.GetManager<MapManager>();
		var res = mapManager.TryPlaceInstOnMapSpace(MapSpace, inst, spot, RotationFlag.North);
		if(!res.Success)
			Console.WriteLine($"Failed to place grass at {spot}: {res.Message}");
		else
		{
		
			if(!ZLayers.ContainsKey(spot.Z))
				CreateZLayer(spot.Z);
			var newNode = ZLayers[spot.Z]
				.CreateMapStructureNode(inst);
			return newNode;
		}

		return null;
	}

	public void CreateGrassNode(MapSpot spot)
	{
		var def = DimMaster.GetDefByName("Defs.MapStructures.Decorations.FakeGrass");
		var mapStructManager = DimMaster.GetManager<MapStructureManager>();
		var newInst = mapStructManager.CreateMapStructureFromDef(def, spot, RotationFlag.North);
		CreateMapStructNode(spot, newInst);
		
	}
}
