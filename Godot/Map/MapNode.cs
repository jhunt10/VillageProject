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
using VillageProject.Godot;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class MapNode : Node2D, Old_IInstNode
{
	private const string _MAP_NODE_WATCHER_KEY = "MapNodeMapSpaceWatcher";
	public const int TILE_WIDTH = 32;
	public const int TILE_HIGHT = 40;
	private TerrainManager TerrainManager;


	public IInst Inst => MapSpace?.Instance;
	public IMapSpace MapSpace;
	public RotationFlag ViewRotation;
	public int VisibleZLayer = 0;

	// Max and Min bounds for MapSpace in world
	private Rect2 _worldBounds { get; set; }
	private Dictionary<int, ZLayerPrefab> ZLayers = new Dictionary<int, ZLayerPrefab>();
	public override void _Ready()
	{

		// ShowZLayer(0);

	}

	public bool InWorldBounds(Vector2 relativePos)
	{
		return _worldBounds.HasPoint(relativePos);
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
		MapSpace.Instance.AddComponentWatcher<IMapSpace>(_MAP_NODE_WATCHER_KEY);
		foreach (var spot in MapSpace.EnumerateMapSpots().OrderBy(x => -x.Y))
		{
			CreateMapNode(spot);
		}

		var topLeft = new Vector2(MapSpace.MinX * TILE_WIDTH,
			(-MapSpace.MaxY * TILE_WIDTH) - ((MapSpace.MaxZ+1) * TILE_HIGHT));
		var bottomRight = new Vector2((MapSpace.MaxX+1) * TILE_WIDTH,
			(-MapSpace.MinX * TILE_WIDTH) + (-MapSpace.MinZ * TILE_HIGHT));
		_worldBounds = new Rect2( topLeft, (-topLeft) + bottomRight);
		RotateMap(RotationFlag.North);
		if(GameMaster.MainCamera != null)
			GameMaster.MainCamera.FollowMap(mapSpace.MapSpaceId);
	}
	
	// private void _ResyncMap()
	// {
	// 	foreach (var spot in MapSpace.EnumerateMapSpots().OrderBy(x => -x.Y))
	// 	{
	// 		foreach (var inst in MapSpace.ListInstsAtSpot(spot))
	// 		{
	// 			if (inst.GetComponentOfType<TerrainCompInst>() != null)
	// 			{
	// 				var node = GetMapCellNodeAtSpot(spot);
	// 				if(node.MapObjectNodes.All(x => x.Inst.Id != inst.Id))
	// 					CreateMapNode(spot);
	// 			}
	// 			
	// 		}
	// 	}
	// }

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
				if(z > VisibleZLayer + 1)
					ZLayers[z].SetLayerVisibility(LayerVisibility.None);
				else if (z == VisibleZLayer + 1)
					ZLayers[z].SetLayerVisibility(LayerVisibility.Shadow);
				else
					ZLayers[z].SetLayerVisibility(LayerVisibility.Full);
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
		if(Inst == null)
			return;
		// var mouseOverSpot = GetMouseOverMapSpot();
		// if(mouseOverSpot.HasValue)
		// 	var mouseOverPos = MapSpotToWorldPos(mouseOverSpot.Value);
		// var changed = Inst.GetWatchedChange(_MAP_NODE_WATCHER_KEY);
		// if (changed)
		// { 
		// 	_ResyncMap();
		// }
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


	public MapCellNode GetMapCellNodeAtSpot(MapSpot spot)
	{
		if (ZLayers.ContainsKey(spot.Z))
			return ZLayers[spot.Z].GetCellNode(spot);
		return null;
	}
	
	public void Delete()
	{
		this.QueueFree();
	}
}
