using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class MapStructureNode : Node2D
{
	public Sprite2D Spite;

	public IInst Inst;

	public bool DirtySprite;
	
	private bool _inited;
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!_inited)
			_init();
		if (DirtySprite)
		{
			SetSprite();
			DirtySprite = false;
		}
	}

	public void SetShow(bool show)
	{
		Spite.Visible = show;
	}
	
	private void SetSprite()
	{
		throw new NotImplementedException();
		// var mapStructComp = Inst.GetComponentOfType<MapStructCompInst>(errorIfNull: true);
		//
		// var spriteComp = Inst.GetComponentOfType<GodotPatchCellSpriteComp>();
		// var spriteDef = spriteComp.CompDef as IPatchSpriteCompDef;
		// var sprite = spriteComp.GetPatchSprite(() =>
		// {
		// 	return DimMaster.GetManager<MapStructureManager>()
		// 		.GetAdjacency(Inst, GameMaster.MapNode.MapSpace, mapStructComp.MapSpot, GameMaster.MapNode.ViewRotation);
		// });
		// var imageText = (ImageTexture)sprite.Sprite;
		// this.Spite.Texture = imageText;
		// this.Spite.Offset = new Vector2(0, -imageText.GetHeight());
	}
}
