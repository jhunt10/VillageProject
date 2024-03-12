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
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class MapNode : Node2D
{
	public const int TILE_WIDTH = 32;
	public const int TILE_HIGHT = 40;
	private TerrainManager TerrainManager;

	
	public IMapSpace MapSpace;
	public RotationFlag ViewRotation;
	public int VisibleZLayer = 0;

	// Max and Min bounds for MapSpace in world
	private Rect2 _worldBounds;

	// private Dictionary<MapSpot, Node2D> TerrainNodes = new Dictionary<MapSpot, Node2D>();
	private Dictionary<int, ZLayerPrefab> ZLayers = new Dictionary<int, ZLayerPrefab>();
	// private Dictionary<int, Sprite2D> ZLayerShadows = new Dictionary<int, Sprite2D>();
	// private Dictionary<int, Node2D> ZTerrainShadows = new Dictionary<int, Node2D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

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

	public void LoadMap(IMapSpace mapSpace)
	{
		MapSpace = mapSpace;
		foreach (var spot in MapSpace.EnumerateMapSpots().OrderBy(x => -x.Y))
		{
			CreateMapNode(spot);
		}

		var topLeft = new Vector2(MapSpace.MinX * TILE_WIDTH,
			(-MapSpace.MaxY * TILE_WIDTH) - ((MapSpace.MaxZ+1) * TILE_HIGHT));
		var bottomRight = new Vector2(MapSpace.MaxX * TILE_WIDTH,
			(-MapSpace.MinX * TILE_WIDTH) + (-MapSpace.MinZ * TILE_HIGHT));
		_worldBounds = new Rect2( topLeft, (-topLeft) + bottomRight);
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
		// var mouseOverSpot = GetMouseOverMapSpot();
		// if(mouseOverSpot.HasValue)
		// 	var mouseOverPos = MapSpotToWorldPos(mouseOverSpot.Value);
		
	}

	public MapSpot? GetMouseOverMapSpot()
	{
		var mainCamera = GameMaster.MainCamera;
		if (mainCamera == null)
			return null;

		var mousePos = GetViewport().GetMousePosition();

		var relativePos = mousePos + mainCamera.Position;
		if (!_worldBounds.HasPoint(relativePos))
		{
			// Console.WriteLine($"Out of world bounds: {_worldBounds} | {relativePos}");
			return null;
		}

		var mapSpot = MapHelper.WorldPositionToMapSpot(
			MapSpace, (int)relativePos.X, (int)relativePos.Y,
			VisibleZLayer + 1, ViewRotation);

		return mapSpot;
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
		
		foreach (var inst in MapSpace.ListInstsAtSpot(spot))
		{
			var terrainComp = inst.GetComponentOfType<TerrainCompInst>();
			if (terrainComp != null)
			{
				var newNode = ZLayers[spot.Z].CreateTerrainNode(spot, inst);
				break;
			}
		}
	}
	
	private void CreateZLayer(int z)
	{
		var prefab = GetNode("ZLayerPrefab");

		var newLayer = (ZLayerPrefab)prefab.Duplicate();
		newLayer.MapNode = this;
		newLayer.Position = new Vector2(0, -40 * z);
		newLayer.BuildMapCells(MapSpace, z);
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


	public MapCellNode GetMapNodeAtSpot(MapSpot spot)
	{
		if (ZLayers.ContainsKey(spot.Z))
			return ZLayers[spot.Z].GetCellNode(spot);
		return null;
	}

	// public Node2D CreateMapStructNode(MapSpot spot, IInst inst)
	// {
	// 	
	// 	var mapManager = DimMaster.GetManager<MapManager>();
	// 	var res = mapManager.TryPlaceInstOnMapSpace(MapSpace, inst, spot, RotationFlag.North);
	// 	if(!res.Success)
	// 		Console.WriteLine($"Failed to place grass at {spot}: {res.Message}");
	// 	else
	// 	{
	// 	
	// 		if(!ZLayers.ContainsKey(spot.Z))
	// 			CreateZLayer(spot.Z);
	// 		var newNode = ZLayers[spot.Z]
	// 			.CreateMapStructureNode(inst);
	// 		if (newNode == null)
	// 			throw new Exception("Failed to create new MapStructure node.");
	// 		return newNode;
	// 	}
	//
	// 	return null;
	// }

	
}
