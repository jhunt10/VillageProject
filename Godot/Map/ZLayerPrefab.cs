using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
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

	public TerrainNode TerrainNodePrefab;
	public MapStructureNode MapStructureNodePrefab;
	
	private Dictionary<MapSpot, Node2D> _cellNodes = new Dictionary<MapSpot, Node2D>();
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
		// TerrainShadowNodes = GetNode<Node2D>("TerrainShadows");
		LayerShadow = GetNode<Sprite2D>("LayerShadow");
		TerrainNodePrefab = GetNode<TerrainNode>("PrefabNodes/TerrainNodePrefab");
		MapStructureNodePrefab = GetNode<MapStructureNode>("PrefabNodes/MapStructureNodePrefab");
		_init = true;
	}
	
	public void SetShow(bool show, bool showShadows = false)
	{
		foreach (var node in _cellNodes.Values)
		{
			if(node is TerrainNode)
				((TerrainNode)node).SetShow(show, showShadows);
			
			if(node is MapStructureNode)
				((MapStructureNode)node).SetShow(show | showShadows);
		}
		LayerShadow.Visible = show;
	}

	public void ResyncRotation()
	{
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		var spots = _cellNodes.Keys.ToList().OrderBy(x => -x.Y);
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

			if (_cellNodes[spot] is TerrainNode)
			{
				var inst = terrainManager.GetTerrainAtSpot(MapNode.MapSpace, spot);
				var topSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
				var topSprite = topSpriteComp.GetPatchSprite(() =>
				{
					return terrainManager.GetHorizontalAdjacency(MapNode.MapSpace, spot, MapNode.ViewRotation, true);
				});

				var frontSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
				var frontSprite = frontSpriteComp.GetPatchSprite(() =>
				{
					return terrainManager.GetVerticalAdjacencyAsHorizontal(MapNode.MapSpace, spot, MapNode.ViewRotation, true);
				});

				// var shadowSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
				// var shadowSprite = shadowSpriteComp.GetPatchSprite(() =>
				// {
				// 	return terrainManager.GetHorizontalAdjacency(mapSpace, spot, rotation, true );
				// });
				var node = (TerrainNode)_cellNodes[spot];
				node.TopSprite.Texture = (ImageTexture)topSprite.Sprite;
				node.FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
				// _terrainNodes[spot].ShadowSprite.Texture = (ImageTexture)shadowSprite.Sprite;
				_cellNodes[spot].Position = SpotToLocalPosition(spot, MapNode.ViewRotation);

			}

			if (_cellNodes[spot] is MapStructureNode)
			{
				((MapStructureNode)_cellNodes[spot]).DirtySprite = true;
				_cellNodes[spot].Position = SpotToLocalPosition(spot, MapNode.ViewRotation);
			}

			CellsParentNode.RemoveChild(_cellNodes[spot]);
			CellsParentNode.AddChild(_cellNodes[spot]);
		}
	}

	public Node2D CreateMapStructureNode(IInst mapStructInst)
	{
		throw new  NotImplementedException();
		// var mapStructureManager = DimMaster.GetManager<MapStructureManager>();
		// var mapStructComp = mapStructInst.GetComponentOfType<MapStructCompInst>(errorIfNull: true);
		//
		// if (_cellNodes.ContainsKey(mapStructComp.MapSpot))
		// 	return _cellNodes[mapStructComp.MapSpot];
		//
		// var spriteComp = mapStructInst.GetComponentOfType<GodotPatchCellSpriteComp>();
		// var spriteDef = spriteComp.CompDef as IPatchSpriteCompDef;
		// var sprite = spriteComp.GetPatchSprite(() =>
		// {
		// 	return mapStructureManager.GetAdjacency(mapStructInst, MapNode.MapSpace, mapStructComp.MapSpot, mapStructComp.Rotation);
		// });
		//
		// var pos = SpotToLocalPosition(mapStructComp.MapSpot, MapNode.ViewRotation);
		// var newNode = (MapStructureNode)MapStructureNodePrefab.Duplicate();
		// newNode.Inst = mapStructInst;
		// newNode.DirtySprite = true;
		// newNode.Position = pos;
		// CellsParentNode.AddChild(newNode);
		// _cellNodes.Add(mapStructComp.MapSpot, newNode);
		//
		// foreach (var pair in mapStructComp.MapSpot.ListAdjacentSpots())
		// {
		// 	if (_cellNodes.ContainsKey(pair.Value))
		// 		if(_cellNodes[pair.Value] is MapStructureNode)
		// 			((MapStructureNode)_cellNodes[pair.Value]).DirtySprite = true;
		// }
		//
		// return newNode;
	}
	
	public Node2D CreateTerrainNode(MapSpot spot)
	{
		Init();
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		
		var inst = terrainManager.GetTerrainAtSpot(MapNode.MapSpace, spot);
		if (inst == null)
			return null;
		
		var topSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
		var topSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var topSprite = topSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetHorizontalAdjacency(MapNode.MapSpace, spot, matchAny:true);
		});

		var frontSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var frontSpriteDef = frontSpriteComp.CompDef as IPatchSpriteCompDef;
		var frontSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(MapNode.MapSpace, spot, matchAny:true);
		});
		
		var shadowSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var shadowSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var shadowSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(MapNode.MapSpace, spot, matchAny:true);
		});
		
		var pos = SpotToLocalPosition(spot, MapNode.ViewRotation);
		var newNode = (TerrainNode)TerrainNodePrefab.Duplicate(); // Create a new Sprite2D.

		newNode.Visible = true;
		newNode.TopSprite.Texture = (ImageTexture)topSprite.Sprite; 
		newNode.FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
		newNode.Position = pos;
		CellsParentNode.AddChild(newNode);
		_cellNodes.Add(spot, newNode);
		
		return newNode;
	}

	private Vector2 SpotToLocalPosition(MapSpot spot, RotationFlag rotation)
	{
		var mapSpace = MapNode.MapSpace;
		var pos = MapHelper.MapSpotToWorldPosition(mapSpace, new MapSpot(spot.X, spot.Y, 0), rotation);
		return new Vector2(pos[0], pos[1]);
	}
}
