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
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class MapStructureNode : Node2D, IMapObjectNode, ISpriteWatcher
{
	public MapNode MapNode { get; set; }
	public Sprite2D Spite;
	public IInst Inst;
	public string? MapSpaceId { get; set; }
	public MapSpot? MapSpot { get; set; }
	public RotationFlag RealRotation { get; set; }
	public RotationFlag ViewRotation { get; set; }
	public void SetViewRotation(RotationFlag viewRotation)
	{
		if (ViewRotation != viewRotation)
		{
			var mapStructSpriteComp = Inst.GetComponentOfType<GodotMapStructSpriteComp>(errorIfNull: true);
			mapStructSpriteComp.SetViewRotation(viewRotation);
			ViewRotation = viewRotation;
			ForceUpdateSprite();
		}
	}

	private bool _inited;
	private bool _forceUpdate = false;
	
	private void _init()
	{
		if(_inited)
			return;
		Spite = GetNode<Sprite2D>("Sprite2D");
		_inited = true;
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
		if (_forceUpdate)
		{
			OnSpriteUpdate();
			_forceUpdate = false;
		}
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
		Console.WriteLine($"Inst {inst._DebugId} assigned to Node {this.Name}.");
		var spriteComps = Inst.GetComponentsOfType<ISpriteComp>();
		foreach (var sprite in spriteComps)
		{
			sprite.AddSpriteWatcher(this);
		}
	}
	
	public void ForceUpdateSprite()
	{
		_forceUpdate = true;
	}

	public void OnSpriteUpdate()
	{
		var mapStructComp = Inst.GetComponentOfType<MapStructCompInst>(errorIfNull: true);
		var mapStructSpriteComp = Inst.GetComponentOfType<GodotMapStructSpriteComp>(errorIfNull: true);
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
			var cell = mapNode.GetMapNodeAtSpot(mapStructComp.MapSpot.Value);
			if (this.GetParent() != cell)
			{
				if(this.GetParent() != null)
					this.GetParent().RemoveChild(this);
				cell.AddMapObjectNode(this);
			}

			MapSpaceId = mapStructComp.MapSpaceId;
			MapSpot = mapStructComp.MapSpot.Value;
			RealRotation = mapStructComp.Rotation;
			if(MapNode != null)
				ViewRotation = MapNode.ViewRotation;
		}
		
		// Reset sprite
		var spriteComp = Inst.GetComponentOfType<GodotMapStructSpriteComp>();
		if (spriteComp == null)
			throw new Exception($"Inst {Inst._DebugId} has no GodotMapStructSpriteComp");
		var sprite = spriteComp.GetSprite();
		var imageText = (ImageTexture)sprite.Sprite;
		this.Spite.Texture = imageText;
		this.Spite.Offset = new Vector2(sprite.XOffset, sprite.YOffset);
	}
}
