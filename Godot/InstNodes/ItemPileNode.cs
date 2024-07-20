using Godot;
using System;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Items;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Map;
using VillageProject.Godot.Sprites;

public partial class ItemPileNode : Node2D, IInstNode
{
	public MapNode MapNode { get; set; }
	public Sprite2D ItemSprite { get; set; }
	
	public RotationFlag RealRotation { get; private set; }
	public RotationFlag ViewRotation { get; private set; }
	public LayerVisibility LayerVisibility { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Inst.ListWatchedChanges("ItemPileNode").Any())
			SetSprites();
	}

	public IInst Inst { get; set; }
	
	public void SetInst(IInst inst)
	{
		this.Inst = inst;
		ItemSprite = GetNode<Sprite2D>("HeldItemSprite");
		inst.AddChangeWatcher("ItemPileNode", new []
		{
			MapStructChangeFlags.MapPositionChanged,
			MapStructChangeFlags.MapRotationChanged,
			InventoryChangeFlags.HeldItemsChange
		});
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
	
	
	public void SetViewRotation(RotationFlag viewRotation)
	{
		if (ViewRotation != viewRotation)
		{
			ViewRotation = viewRotation;
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
				this.Visible = false;
				break;
			case LayerVisibility.Half:
				this.Visible = true;
				break;
			case LayerVisibility.Full:
				this.Visible = true;
				break;
		}
	}

}
