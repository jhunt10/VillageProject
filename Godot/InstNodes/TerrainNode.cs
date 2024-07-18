using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class TerrainNode : Node2D, IInstNode
{
	public MapNode MapNode { get; set; }
	public IInst Inst { get; set; }
	public RotationFlag RealRotation { get; private set; }
	public RotationFlag ViewRotation { get; private set; }
	//
	public LayerVisibility LayerVisibility { get; private set; }

	private bool _forceUpdate;
	private Sprite2D _shadowSprite;
	public Sprite2D ShadowSprite
	{
		get
		{
			return _shadowSprite;
		}
	}
	
	private Sprite2D _topSprite;
	public Sprite2D TopSprite
	{
		get
		{
			return _topSprite;
		}
	}
	
	private Sprite2D _frontSprite;
	public Sprite2D FrontSprite
	{
		get
		{
			return _frontSprite;
		}
	}

	public void SetInst(IInst inst)
	{
		this.Inst = inst;
		_shadowSprite = GetNode<Sprite2D>("ShadowSprite");
		_topSprite = GetNode<Sprite2D>("TopSprite");
		_frontSprite = GetNode<Sprite2D>("FrontSprite");
		Inst.AddComponentWatcher<MapStructCompInst>("TerrainNode:MapPos", true);
	}

	public void Delete()
	{
		this.QueueFree();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var t = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Inst.GetWatchedChange("TerrainNode:MapPos", true))
			SetSprites();
	}

	public void SetViewRotation(RotationFlag viewRotation)
	{
		if (ViewRotation != viewRotation)
		{
			ViewRotation = viewRotation;
			_forceUpdate = true;
		}
		SetSprites();
	}
	
	public void SetLayerVisibility(LayerVisibility visibility)
	{
		this.LayerVisibility = visibility;
		switch (LayerVisibility)
		{
			case LayerVisibility.None:
				this.Visible = false;
				break;
			case LayerVisibility.Shadow:
				this.Visible = true;
				if(_topSprite != null)
					this._topSprite.Visible = false;
				if(_frontSprite != null)
					this._frontSprite.Visible = false;
				if(_shadowSprite != null)
					this._shadowSprite.Visible = true;
				break;
			case LayerVisibility.Half:
				this.Visible = true;
				if(_topSprite != null)
					this._topSprite.Visible = true;
				if(_frontSprite != null)
					this._frontSprite.Visible = true;
				if(_shadowSprite != null)
					this._shadowSprite.Visible = false;
				break;
			case LayerVisibility.Full:
				this.Visible = true;
				if(_topSprite != null)
					this._topSprite.Visible = true;
				if(_frontSprite != null)
					this._frontSprite.Visible = true;
				if(_shadowSprite != null)
					this._shadowSprite.Visible = false;
				break;
		}
	}

	public void SetSprites()
	{
		if(Inst == null)
			return;

		var rotation = RealRotation.AddRotation(ViewRotation);
		var mapStructComp = Inst.GetComponentOfType<MapStructCompInst>();
		var mapSpot = mapStructComp.MapSpot;
		var mapSpace = DimMaster.GetManager<MapManager>().GetMapSpaceById(mapStructComp.MapSpaceId);
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		
		var topSpriteComp = Inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
		var topSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var topSprite = topSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetHorizontalAdjacency(mapSpace, mapSpot.Value, rotation, matchAny:true);
		});

		var frontSpriteComp = Inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var frontSpriteDef = frontSpriteComp.CompDef as IPatchSpriteCompDef;
		var frontSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, mapSpot.Value, rotation, matchAny:true);
		});
		
		var shadowSpriteComp = Inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var shadowSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var shadowSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, mapSpot.Value, rotation, matchAny:true);
		});
		
		this.TopSprite.Texture = (ImageTexture)topSprite.Sprite; 
		this.FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
	}
}
