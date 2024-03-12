using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class ZLayerPrefab : Node2D
{
	public Sprite2D LayerShadow;

	public Node2D CellsParentNode;

	public MapNode MapNode;
	// public Node2D TerrainNodes;
	// public Node2D MapStructureNodes;
	// public Node2D TerrainShadowNodes;

	// public MapStructureNode MapStructureNodePrefab;
	
	private Dictionary<MapSpot, MapCellNode> _cellNodes = new Dictionary<MapSpot, MapCellNode>();
	// private Dictionary<MapSpot, MapStructureNode> _mapStructNodes = new Dictionary<MapSpot, MapStructureNode>();
	
	
	private bool _init;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Init();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void Init()
	{
		if(_init)
			return;
		this.YSortEnabled = true;
		CellsParentNode = GetNode<Node2D>("CellNodes");
		LayerShadow = GetNode<Sprite2D>("LayerShadow");
		// TerrainShadowNodes = GetNode<Node2D>("TerrainShadows");
		// MapStructureNodePrefab = GetNode<MapStructureNode>("PrefabNodes/MapStructureNodePrefab");
		_init = true;
	}
	
	public void SetShow(bool show, bool showShadows = false)
	{
		// foreach (var node in _cellNodes.Values)
		// {
		// 	if(node is TerrainNode)
		// 		((TerrainNode)node).SetShow(show, showShadows);
		// 	
		// 	if(node is MapStructureNode)
		// 		((MapStructureNode)node).SetShow(show | showShadows);
		// }
		LayerShadow.Visible = show;
	}

	public void BuildMapCells(IMapSpace mapSpace, int zLayer)
	{
		if(!_init)
			Init();
		
		for(int y = mapSpace.MaxY; y >= mapSpace.MinY; y--)
		for (int x = mapSpace.MinX; x <= mapSpace.MaxX; x++)
		{
			var spot = new MapSpot(x, y, zLayer);
			var pos = SpotToLocalPosition(spot, MapNode.ViewRotation);
			var newNode = (MapCellNode)MapControllerNode.MapCellPrefab.Duplicate();
			newNode.Position = pos;
			newNode.MapNode = MapNode;
			CellsParentNode.AddChild(newNode);
			_cellNodes.Add(spot, newNode);
			newNode.Visible = true;
		}
	}

	public MapCellNode? GetCellNode(MapSpot spot)
	{
		if(_cellNodes.ContainsKey(spot))
			return _cellNodes[spot];
		return null;
	}

	public void ResyncRotation()
	{
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		var spots = _cellNodes.Keys.ToList().OrderBy(x => x.Y);
		switch (MapNode.ViewRotation)
		{
			case RotationFlag.North:
				spots = _cellNodes.Keys.ToList().OrderBy(x => -x.Y);
				break;
			case RotationFlag.East:
				spots = _cellNodes.Keys.ToList().OrderBy(x => -x.X);
				break;
			case RotationFlag.South:
				spots = _cellNodes.Keys.ToList().OrderBy(x => x.Y);
				break;
			case RotationFlag.West:
				spots = _cellNodes.Keys.ToList().OrderBy(x => x.X);
				break;
		}
		foreach (var spot in spots)
		{
			var mapCell = _cellNodes[spot];
			mapCell.UpdateSprites(MapNode.MapSpace, spot, MapNode.ViewRotation);
			mapCell.Position = SpotToLocalPosition(spot, MapNode.ViewRotation);
			CellsParentNode.RemoveChild(mapCell);
			CellsParentNode.AddChild(mapCell);
		}
	}

	// public Node2D CreateEmptyNode(MapSpot spot)
	// {
	// 	var pos = SpotToLocalPosition(spot, MapNode.ViewRotation);
	// 	var newNode = (MapStructureNode)MapStructureNodePrefab.Duplicate();
	// 	newNode.DirtySprite = true;
	// 	newNode.Position = pos;
	// 	CellsParentNode.AddChild(newNode);
	// 	_cellNodes.Add(spot, newNode);
	// 	newNode.Visible = true;
	// 	return newNode;
	// }

	// public Node2D CreateMapStructureNode(IInst mapStructInst)
	// {
	//
	// }
	
	public Node2D CreateTerrainNode(MapSpot spot, IInst inst)
	{
		Init();
		var newNode = (TerrainNode)MapControllerNode.TerrainNodePrefab.Duplicate(); // Create a new Sprite2D.
		newNode.TerrainInst = inst;
		newNode.Visible = true;
		var cellNode = GetCellNode(spot);
		cellNode.AddMapObjectNode(newNode);
		newNode.SetMapPosition(MapNode.MapSpace, spot, RotationFlag.North);
		return newNode;
	}

	private Vector2 SpotToLocalPosition(MapSpot spot, RotationFlag rotation)
	{
		var mapSpace = MapNode.MapSpace;
		var pos = MapHelper.MapSpotToWorldPosition(mapSpace, new MapSpot(spot.X, spot.Y, 0), rotation);
		return new Vector2(pos[0], pos[1]);
	}
}
