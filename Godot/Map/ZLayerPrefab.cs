using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VillageProject.Core.DIM;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class ZLayerPrefab : Node2D
{
	public Sprite2D LayerShadow;
	public Node2D TerrainNodes;
	// public Node2D TerrainShadowNodes;

	public TerrainNode TerrainNodePrefab;
	
	private Dictionary<MapSpot, TerrainNode> _terrainNodes = new Dictionary<MapSpot, TerrainNode>();
	
	
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
		TerrainNodes = GetNode<Node2D>("TerrainNodes");
		// TerrainShadowNodes = GetNode<Node2D>("TerrainShadows");
		LayerShadow = GetNode<Sprite2D>("LayerShadow");
		TerrainNodePrefab = GetNode<TerrainNode>("TerrainNodes/TerrainNodePrefab");
		_init = true;
	}
	
	public void SetShow(bool show, bool showShadows = false)
	{
		foreach (var node in _terrainNodes.Values)
		{
			node.SetShow(show, showShadows);
		}
		LayerShadow.Visible = show;
	}

	public void SetRotation(MapSpace mapSpace, RotationFlag rotation)
	{
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		var spots = _terrainNodes.Keys.ToList().OrderBy(x => -x.Y);
		switch (rotation)
		{
			case RotationFlag.North:
				spots = _terrainNodes.Keys.ToList().OrderBy(x => x.Y);
				break;
			case RotationFlag.East:
				spots = _terrainNodes.Keys.ToList().OrderBy(x => -x.X);
				break;
			case RotationFlag.South:
				spots = _terrainNodes.Keys.ToList().OrderBy(x => -x.Y);
				break;
			case RotationFlag.West:
				spots = _terrainNodes.Keys.ToList().OrderBy(x => x.X);
				break;
		}
		foreach (var spot in spots)
		{
			
			var inst = mapSpace.GetTerrainAtSpot(spot);
			var topSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
			var topSprite = topSpriteComp.GetPatchSprite(() =>
			{
				return terrainManager.GetHorizontalAdjacency(mapSpace, spot, rotation, true );
			});

			var frontSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
			var frontSprite = frontSpriteComp.GetPatchSprite(() =>
			{
				return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, spot, rotation, true);
			});
		
			// var shadowSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
			// var shadowSprite = shadowSpriteComp.GetPatchSprite(() =>
			// {
			// 	return terrainManager.GetHorizontalAdjacency(mapSpace, spot, rotation, true );
			// });
			
			_terrainNodes[spot].Position = GetTerrainNodePosition(spot, rotation);
			_terrainNodes[spot].TopSprite.Texture = (ImageTexture)topSprite.Sprite;
			_terrainNodes[spot].FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
			// _terrainNodes[spot].ShadowSprite.Texture = (ImageTexture)shadowSprite.Sprite;
			
			TerrainNodes.RemoveChild(_terrainNodes[spot]);
			TerrainNodes.AddChild(_terrainNodes[spot]);
		}
	}

	public Node2D CreateTerrainNode(MapNode mapNode, TerrainManager terrainManager, MapSpace mapSpace, MapSpot spot)
	{
		Init();
		
		var inst = mapSpace.GetTerrainAtSpot(spot);
		if (inst == null)
			return null;
		
		var topSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
		var topSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var topSprite = topSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetHorizontalAdjacency(mapSpace, spot, matchAny:true);
		});

		var frontSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var frontSpriteDef = frontSpriteComp.CompDef as IPatchSpriteCompDef;
		var frontSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, spot, matchAny:true);
		});
		
		var shadowSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var shadowSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var shadowSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, spot, matchAny:true);
		});
		
		var pos = GetTerrainNodePosition(spot);
		var newNode = (TerrainNode)TerrainNodePrefab.Duplicate(); // Create a new Sprite2D.

		newNode.Visible = true;
		newNode.TopSprite.Texture = (ImageTexture)topSprite.Sprite; 
		newNode.FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
		newNode.Position = pos;
		TerrainNodes.AddChild(newNode);
		_terrainNodes.Add(spot, newNode);
		
		return newNode;
	}

	private Vector2 GetTerrainNodePosition(MapSpot spot, RotationFlag rotation = RotationFlag.North)
	{
		switch (rotation)
		{
			case RotationFlag.North:
				return new Vector2(spot.X * MapNode.TILE_WIDTH, spot.Y * MapNode.TILE_WIDTH);
			case RotationFlag.East:
				return new Vector2(spot.Y * MapNode.TILE_WIDTH, -spot.X * MapNode.TILE_WIDTH);
			case RotationFlag.South:
				return new Vector2(-spot.X * MapNode.TILE_WIDTH, -spot.Y * MapNode.TILE_WIDTH);
			case RotationFlag.West:
				return new Vector2(-spot.Y * MapNode.TILE_WIDTH, spot.X * MapNode.TILE_WIDTH);
		}
		return new Vector2(spot.X * MapNode.TILE_WIDTH, spot.Y * MapNode.TILE_WIDTH);
	}
}
