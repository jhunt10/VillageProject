using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class TerrainNode : Node2D, IMapObjectNode
{
	public MapNode MapNode { get; set; }
	public IInst TerrainInst { get; set; }
	public IInst Inst => TerrainInst;
	public void DirtySprite()
	{
		throw new NotImplementedException();
	}

	public string? MapSpaceId { get; set; }
	public MapSpot? MapSpot { get; set; }
	public RotationFlag RealRotation { get; private set; }
	public RotationFlag ViewRotation { get; private set; }
	
	public LayerVisibility LayerVisibility { get; private set; }

	private bool _forceUpdate;
	private Sprite2D _shadowSprite;
	public Sprite2D ShadowSprite
	{
		get
		{
			if(_shadowSprite == null)
				_init();
			return _shadowSprite;
		}
	}
	
	private Sprite2D _topSprite;
	public Sprite2D TopSprite
	{
		get
		{
			if(_topSprite == null)
				_init();
			return _topSprite;
		}
	}
	
	private Sprite2D _frontSprite;
	public Sprite2D FrontSprite
	{
		get
		{
			if(_frontSprite == null)
				_init();
			return _frontSprite;
		}
	}

	private bool _inited;

	private void _init()
	{
		if(_inited)
			return;

		_shadowSprite = GetNode<Sprite2D>("ShadowSprite");
		_topSprite = GetNode<Sprite2D>("TopSprite");
		_frontSprite = GetNode<Sprite2D>("FrontSprite");
		
		_inited = true;
	}

	public void Delete()
	{
		this.QueueFree();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_forceUpdate)
		{
			UpdateSprite();
			_forceUpdate = false;
		}
	}


	public void SetShow(bool show, bool showShadows = false)
	{
		_topSprite.Visible = show;
		_frontSprite.Visible = show;
		_shadowSprite.Visible = showShadows;
	}

	public void SetViewRotation(RotationFlag viewRotation)
	{
		if (ViewRotation != viewRotation)
		{
			ViewRotation = viewRotation;
			_forceUpdate = true;
		}
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

	public void ForceUpdateSprite()
	{
		_forceUpdate = true;
	}

	public void SetMapPosition(IMapSpace mapSpace, MapSpot spot, RotationFlag rotation)
	{
		if(MapSpaceId == mapSpace.MapSpaceId && spot == MapSpot && RealRotation == rotation)
			return;
		MapSpaceId = mapSpace.MapSpaceId;
		MapSpot = spot;
		RealRotation = rotation;
		_forceUpdate = true;
	}

	public void UpdateSprite()
	{
		if(TerrainInst == null)
			return;

		var rotation = RealRotation.AddRotation(ViewRotation);
		
		var mapSpace = DimMaster.GetManager<MapManager>().GetMapSpaceById(MapSpaceId);
		var terrainManager = DimMaster.GetManager<TerrainManager>();
		
		var topSpriteComp = TerrainInst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
		var topSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var topSprite = topSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetHorizontalAdjacency(mapSpace, MapSpot.Value, rotation, matchAny:true);
		});

		var frontSpriteComp = TerrainInst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var frontSpriteDef = frontSpriteComp.CompDef as IPatchSpriteCompDef;
		var frontSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, MapSpot.Value, rotation, matchAny:true);
		});
		
		var shadowSpriteComp = TerrainInst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var shadowSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var shadowSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return terrainManager.GetVerticalAdjacencyAsHorizontal(mapSpace, MapSpot.Value, rotation, matchAny:true);
		});
		
		this.TopSprite.Texture = (ImageTexture)topSprite.Sprite; 
		this.FrontSprite.Texture = (ImageTexture)frontSprite.Sprite;
	}
}
