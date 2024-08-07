using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;
using VillageProject.Core.Sprites.MapStructures;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class MapStructureNode : Node2D, IInstNode
{
	private string ChangeKey = "MapStructNode";
	
	public MapNode MapNode { get; set; }
	public Sprite2D Spite;
	public Sprite2D ShadowSpite;
	public IInst Inst { get; private set; }
	public InstNodeCompInst InstNodeComp { get; private set; }
	public string? MapSpaceId { get; set; }
	public MapSpot? MapSpot { get; set; }
	public RotationFlag RealRotation { get; set; }
	public RotationFlag ViewRotation { get; set; }
	
	public LayerVisibility LayerVisibility { get; private set; }

	public void SetLayerVisibility(LayerVisibility visibility)
	{
		InstNodeComp.SetLayerVisibility(visibility);
		
		this.LayerVisibility = visibility;
		switch (LayerVisibility)
		{
			case LayerVisibility.None:
				this.Visible = false;
				break;
			case LayerVisibility.Shadow:
				this.Visible = true;
				if(this.Spite != null)
					this.Spite.Visible = false;
				break;
			case LayerVisibility.Half:
				this.Visible = true;
				if(this.Spite != null)
					this.Spite.Visible = true;
				break;
			case LayerVisibility.Full:
				this.Visible = true;
				if(this.Spite != null)
					this.Spite.Visible = true;
				break;
		}
	}

	public void SetViewRotation(RotationFlag viewRotation)
	{
		InstNodeComp.SetViewRotation(viewRotation);
		if (ViewRotation != viewRotation)
		{
			Inst.FlagWatchedChange(MapStructChangeFlags.ViewRotationChanged);
			Inst.FlagWatchedChange(SpriteChangeFlags.SpriteDirtied);
			ViewRotation = viewRotation;
			ForceUpdateSprite();
		}
	}

	private bool _inited;
	private bool _forceUpdate = false;
	private const string SPRITE_WATCHER_KEY = "MapStructNodeSpriteWatcher";
	
	private void _init()
	{
		if(_inited)
			return;
		Spite = GetNode<Sprite2D>("Sprite");
		ShadowSpite = GetNode<Sprite2D>("ShadowSprite");
		_inited = true;
	}

	public void Delete()
	{
		this.QueueFree();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(!_inited)
			_init();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Inst == null)
			return;
		if (Inst.GetWatchedChange(ChangeKey, MapStructChangeFlags.MapPositionChanged))
		{
			GameMaster.MapControllerNode.PlaceInstNodeOnMap(this);
			Inst.FlagWatchedChange(SpriteChangeFlags.SpriteDirtied);
		}
		if (Inst.GetWatchedChange(ChangeKey, SpriteChangeFlags.SpriteDirtied))
			UpdateSprite();
	}

	public void SetShow(bool show)
	{
		Spite.Visible = show;
	}

	public void SetInst(IInst inst)
	{
		if (Inst != null)
			throw new Exception("Inst already set");
		_init();
		Inst = inst;
		InstNodeComp = Inst.GetComponentOfType<InstNodeCompInst>();
		Inst.AddChangeWatcher(ChangeKey, new []
		{
			SpriteChangeFlags.SpriteDirtied,
			MapStructChangeFlags.MapPositionChanged,
			MapStructChangeFlags.MapRotationChanged,
			MapStructChangeFlags.ViewRotationChanged
		}, initiallyDirty:true);
		Console.WriteLine($"Inst {inst._DebugId} assigned to Node {this.Name}.");
	}
	
	public void ForceUpdateSprite()
	{
		var mapStructSpriteComp = Inst.GetComponentOfType<MapStructSpriteCompInst>(errorIfNull: true);
		mapStructSpriteComp?.DirtySprite();
	}

	public void UpdateSprite()
	{
		var mapStructComp = Inst.GetComponentOfType<MapStructCompInst>(activeOnly:false, errorIfNull: true);
		var mapStructSpriteComp = Inst.GetComponentOfType<MapStructSpriteCompInst>(activeOnly:false, errorIfNull: true);
		// Move Node position if needed
		if (_forceUpdate 
		    || mapStructComp.MapSpot != MapSpot || mapStructComp.Rotation != RealRotation || mapStructComp.MapSpaceId != MapSpaceId
		    || mapStructSpriteComp.ViewRotation != ViewRotation)
		{
			if (!mapStructComp.MapSpot.HasValue || string.IsNullOrEmpty(mapStructComp.MapSpaceId))
			{
				this.Visible = false;
				return;
			}

			var mapNode = GameMaster.MapControllerNode.GetMapNode(mapStructComp.MapSpaceId);
			if (mapNode == null)
				throw new Exception($"Failed to find MapNode for MapSpace '{mapStructComp.MapSpaceId}'.");
			var cell = mapNode.GetMapCellNodeAtSpot(mapStructComp.MapSpot.Value);
			if (this.GetParent() != cell)
			{
				if(this.GetParent() != null)
					this.GetParent().RemoveChild(this);
				// cell.AddMapObjectNode(this);
			}

			MapSpaceId = mapStructComp.MapSpaceId;
			MapSpot = mapStructComp.MapSpot.Value;
			RealRotation = mapStructComp.Rotation;
			this.Visible = true;
			if(MapNode != null)
				ViewRotation = MapNode.ViewRotation;
		}
		
		// Reset sprite
		if (mapStructSpriteComp == null)
			throw new Exception($"Inst {Inst._DebugId} has no GodotMapStructSpriteComp");
		var sprite = mapStructSpriteComp.GetSprite();
		GameMaster.SpriteHelper.SetSpriteFromData(this.Spite, sprite);
	}
}
