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

	private Dictionary<MapSpot, Node2D> TerrainNodes = new Dictionary<MapSpot, Node2D>();
	private Dictionary<int, ZLayerPrefab> ZLayers = new Dictionary<int, ZLayerPrefab>();
	// private Dictionary<int, Sprite2D> ZLayerShadows = new Dictionary<int, Sprite2D>();
	// private Dictionary<int, Node2D> ZTerrainShadows = new Dictionary<int, Node2D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MapSpace = new MapSpace();
		// TerrainManager = DimMaster.GetManager<TerrainManager>();
		// for(int x = 0; x < 10; x++)
		// for (int y = 0; y < 10; y++)
		// {
		// 	var spot = new MapSpot(x, y, 0);
		// 	var index = (int)(GD.Randi() % TerrainManager._terrainInsts.Count);
		// 	var key = TerrainManager._terrainInsts.Keys.ToList()[index];
		// 	TerrainManager.TerrainMap.Add(spot,key);
		// }
		
		GenerateMap();

		foreach (var spot in MapSpace.EnumerateMapSpots().OrderBy(x => -x.Y))
		{
			CreateTerrainNode(spot);
		}
			
		
		// ShowZLayer(0);
		
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
	
	public void GenerateMap()
	{
		// var maxX = 30;
		// var minX = -30;
		// var maxY = 20;
		// var minY = -20;
		// var maxZ = 3;
		// var minZ = -3;
		
		var maxX = 3;
		var minX = -3;
		var maxY = 3;
		var minY = -3;
		var maxZ = 1;
		var minZ = 0;
		MapSpace._buildCellMatrix(maxX,minX,maxY,minY,maxZ,minZ);
		
		TerrainManager = DimMaster.GetManager<TerrainManager>();
		var terrainNoise = new FastNoiseLite();
		
		terrainNoise.Seed = DateTime.Now.Millisecond;
		
		terrainNoise.Frequency = (float)0.05;
		var hightNoise = new FastNoiseLite();
		hightNoise.Seed = terrainNoise.Seed + 1;
		hightNoise.Frequency = (float)0.02;
		
		for(int x = minX; x <= maxX; x++)
		for (int y = minY; y <= maxY; y++)
		{
			if(x == 1 && y == 1)
				continue;
			var terrainVal = terrainNoise.GetNoise2D(x, y);
			var index = 0;
			if (terrainVal > 0)
				index = 1;

			var hightVal = hightNoise.GetNoise2D(x, y);
			var hight = (int)(hightVal * (maxZ - minZ + 1));
			if (minZ == 0 && maxZ == 0)
				hight = 1;
			if (minZ == 0 && maxZ == 1)
				hight = 1;
			
			var terrainInst = TerrainManager._terrainInsts.Values.ToList()[index];
			for(int z = minZ; z < hight; z++)
				MapSpace.SetTerrainAtSpot(terrainInst, new MapSpot(x, y, z));
			
			if(index == 0)
				CreateGrassNode(new MapSpot(x, y, hight));
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
		var x = Mathf.FloorToInt(relativePos.X / (float)TILE_WIDTH);
		
		var maxZ = ZLayers.Keys.Max();
		var minZ = ZLayers.Keys.Min();
		
		for (int z = VisibleZLayer; z >= minZ; z--)
		{
			var y = Mathf.FloorToInt((relativePos.Y + (float)((z+1) * TILE_HIGHT))  / (float)TILE_WIDTH);
			MapSpot spot = default(MapSpot);
			switch (ViewRotation)
			{
				case RotationFlag.North:
					spot = new MapSpot(x, y, z);
					break;
				case RotationFlag.East:
					spot = new MapSpot(-y, x, z);
					break;
				case RotationFlag.South:
					spot = new MapSpot(-x, -y, z);
					break;
				case RotationFlag.West:
					spot = new MapSpot(y, -x, z);
					break;
			}
			
			// Check if top of cell
			if (TerrainManager.GetTerrainAtSpot(MapSpace, spot) != null)
			{
				// Console.WriteLine($"TopFound: Mouse: {relativePos} | Spot: {spot}");
				return spot;
			}
			
			// Check if front of cell
			var backSpot = spot.DirectionToSpot(DirectionFlags.Back, ViewRotation);
			if (TerrainManager.GetTerrainAtSpot(MapSpace,backSpot) != null)
			{
				// Console.WriteLine($"FrontFound: Mouse: {relativePos} | Spot: {backSpot}");
				return backSpot;
			}

			// There is a gap between the bottom of where a tile top would be and where the bottom of the front sprite really is
			// We need to correct for this with an extra check
			var diff = relativePos.Y - ((y * TILE_WIDTH) - (z * TILE_HIGHT));
			if(z ==0)
				// Console.WriteLine($"Diff: {diff}");
			if ( diff < (TILE_HIGHT - TILE_WIDTH))
			{
				var doubleBackSpot = backSpot.DirectionToSpot(DirectionFlags.Back, ViewRotation);
				// Console.WriteLine($"Edge Case: Mouse: {relativePos}Checking Spot: {spot} | FrontSpot: {backSpot} | DoubleCheck: {doubleBackSpot} | Diff: {diff}");
				if (TerrainManager.GetTerrainAtSpot(MapSpace,doubleBackSpot) != null)
				{
					return doubleBackSpot;
				}
			}

		}

		// Console.WriteLine($"NO RESULT!!! relativePos: {relativePos} | mainCamera.Position: {mainCamera.Position}");
		return new MapSpot(0, 0, 0);
	}

	public Vector2 MapSpotToWorldPos(MapSpot spot)
	{
		var pos = MapHelper.MapSpotToWorldPosition(MapSpace, spot, ViewRotation, 
			TILE_WIDTH, TILE_WIDTH, TILE_HIGHT);
		return new Vector2(pos[0], pos[1]);
	}



	private void CreateTerrainNode(MapSpot spot)
	{
		if(!ZLayers.ContainsKey(spot.Z))
			CreateZLayer(spot.Z);
		var newNode = ZLayers[spot.Z].CreateTerrainNode(spot);
		TerrainNodes.Add(spot, newNode);
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


	public Node2D GetTerrainNodeAtSpot(MapSpot spot)
	{
		if (TerrainNodes.ContainsKey(spot))
			return TerrainNodes[spot];
		return null;
	}

	public void CreateGrassNode(MapSpot spot)
	{
		var mapStructManager = DimMaster.GetManager<MapStructureManager>();
		var def = DimMaster.Defs.Single(x => x.DefName == "Defs.MapStructures.Decorations.FakeGrass");
		var newInst = mapStructManager.CreateMapStructureFromDef(def, spot, RotationFlag.North);

		if(!ZLayers.ContainsKey(spot.Z))
			CreateZLayer(spot.Z);
		var newNode = ZLayers[spot.Z]
			.CreateMapStructureNode(newInst);
		
	}
}
