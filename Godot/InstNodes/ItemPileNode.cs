using Godot;
using System;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Items;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Sprites;

public partial class ItemPileNode : Node2D, IInstNode
{
	public MapNode MapNode { get; set; }
	public Sprite2D ItemSprite { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Inst.GetWatchedChange("ItemPileNode:Inv", true))
			SetSprites();
		

	}

	public IInst Inst { get; set; }
	
	public void SetInst(IInst inst)
	{
		this.Inst = inst;
		ItemSprite = GetNode<Sprite2D>("HeldItemSprite");
		inst.AddComponentWatcher<MapStructCompInst>("ItemPileNode:MapPos", true);
		inst.AddComponentWatcher<InventoryCompInst>("ItemPileNode:Inv", true);
	}
	
	public void Delete()
	{
		
	}

	private void SetSprites()
	{
		var invComp = Inst.GetComponentOfType<InventoryCompInst>(activeOnly:false);
		foreach (var itemComp in invComp.ListHeldItems())
		{
			var itemSpriteDef = itemComp.ItemCompDef.ItemSpriteDef;
			GodotSpriteHelper.SetSpriteFromDef(ItemSprite, itemComp.Instance.Def, itemSpriteDef);
		}
	}

}
