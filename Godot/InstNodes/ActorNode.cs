using Godot;
using System;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Godot.Actors;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class ActorNode : Node2D, IInstNode
{
	public MapNode MapNode { get; set; }
	public Sprite2D Spite;
	public IInst Inst { get; private set; }
	// public string? MapSpaceId { get; set; }
	// public MapSpot? MapSpot { get; set; }
	public RotationFlag RealRotation { get; set; }
	public RotationFlag ViewRotation { get; set; }
	
	public LayerVisibility LayerVisibility { get; private set; }

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
				if(this.Spite != null)
					this.Spite.Visible = true;
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
		if (ViewRotation != viewRotation)
		{
			var spriteComp = Inst.GetComponentOfType<GodotActorSpriteComp>(errorIfNull: true);
			spriteComp.SetViewRotation(viewRotation);
			ViewRotation = viewRotation;
			ForceUpdateSprite();
		}
	}

	private bool _forceUpdate = false;
	private const string SPRITE_WATCHER_KEY = "MapStructNodeSpriteWatcher";
	

	public void Delete()
	{
		this.QueueFree();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Inst == null)
		{
			return;
		}
		if(Inst.GetWatchedChange("ActorNode"))
			GameMaster.MapControllerNode.PlaceInstNodeOnMap(this);

		// Get sprite comp to check for updates
		// var spriteChange = Inst.GetWatchedChange(SPRITE_WATCHER_KEY);
		// if (spriteChange)
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
		Inst = inst;
		Spite = GetNode<Sprite2D>("Sprite2D");
		Inst.AddComponentWatcher<GodotActorSpriteComp>(SPRITE_WATCHER_KEY);
		Console.WriteLine($"Inst {inst._DebugId} assigned to Node {this.Name}.");
		Inst.AddComponentWatcher<ActorCompInst>("ActorNode", false);
		this.Visible = true;
		this.Spite.Visible = true;
	}
	
	public void ForceUpdateSprite()
	{
		var spriteComp = Inst.GetComponentOfType<GodotActorSpriteComp>(errorIfNull: true);
		spriteComp?.DirtySprite();
	}

	public void UpdateSprite()
	{
		var actorComp = Inst.GetComponentOfType<ActorCompInst>(errorIfNull: true);
		var spriteComp = Inst.GetComponentOfType<GodotActorSpriteComp>();
		if (!actorComp.MapPosition.HasValue)
		{
			this.Visible = false;
			return;
		}
		var mapPos = actorComp.MapPosition.Value;
		RealRotation = actorComp.MapPosition.Value.Rotation;
		
		// Move Node position if needed
		// if (_forceUpdate 
		//     || mapPos.MapSpot != MapSpot || mapPos.Rotation != RealRotation || mapPos.MapSpaceId != MapSpaceId
		//     || spriteComp.ViewRotation != ViewRotation)
		// {
		// 	var mapNode = GameMaster.MapControllerNode.GetMapNode(mapPos.MapSpaceId);
		// 	if (mapNode == null)
		// 		throw new Exception($"Failed to find MapNode for MapSpace '{mapPos.MapSpaceId}'.");
		// 	var cell = mapNode.GetMapCellNodeAtSpot(mapPos.MapSpot);
		// 	if (this.GetParent() != cell)
		// 	{
		// 		if(this.GetParent() != null)
		// 			this.GetParent().RemoveChild(this);
		// 		// cell.AddMapObjectNode(this);
		// 	}
		//
		// 	MapSpaceId = mapPos.MapSpaceId;
		// 	MapSpot = mapPos.MapSpot;
		// 	RealRotation = mapPos.Rotation;
		// 	if(MapNode != null)
		// 		ViewRotation = MapNode.ViewRotation;
		// }
		
		// Reset sprite
		if (spriteComp == null)
			throw new Exception($"Inst {Inst._DebugId} has no GodotActorSpriteComp");
		spriteComp.DirtySprite();
		var sprite = spriteComp.GetSprite();
		var imageText = (ImageTexture)sprite.Sprite;
		this.Spite.Texture = imageText;
		this.Spite.Offset = new Vector2(sprite.XOffset, -sprite.Hight + sprite.YOffset);
		var offset = new Vector2(actorComp.MapPosition.Value.Offset.X * MapNode.TILE_WIDTH,
			-actorComp.MapPosition.Value.Offset.Y * MapNode.TILE_WIDTH);
		switch (ViewRotation)
		{
			case RotationFlag.North:
				offset = offset;
				break;
			case RotationFlag.East:
				offset = new Vector2(offset.Y, -offset.X);
				break;
			case RotationFlag.South:
				offset = new Vector2(-offset.X, -offset.Y);
				break;
			case RotationFlag.West:
				offset = new Vector2(-offset.Y, offset.X);
				break;
		}

		this.Position = offset;
	}
}
