using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Sprites.Interfaces;

public partial class ConstructablePreview : Sprite2D
{
	private IDef _constructableDef;
	private IInst _constructableInst;
	private MapSpot _currentSpot;
	private RotationFlag _rotation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mapNode = GameMaster.MapNode;
		if(mapNode == null)
			return;
		
		var mouseSpot = mapNode.GetMouseOverMapSpot() + new MapSpot(0,0,1);
		if(mouseSpot == _currentSpot)
			return;
		
		var node = mapNode.GetMapNodeAtSpot(mouseSpot);
		if(node == null)
			return;
		Console.WriteLine($"SPot: {mouseSpot}");
		this.Position = Vector2.Zero;
		if (this.GetParent() != null)
			this.Reparent(node, false);
		else
			node.AddChild(this);
		_currentSpot = mouseSpot;
		UpdateSprite();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.R)
			{
				_rotation = _rotation.ApplyRotationDirection(RotationDirection.Clockwise);
				UpdateSprite();
			}
		}
			
	}

	public void SetConstructableDef(IDef def)
	{
		this._constructableDef = def;
		_constructableInst = DimMaster.InstantiateDef(_constructableDef);
	}

	public void UpdateSprite()
	{
		if(_constructableInst == null)
			return;
		
		var mapSpace = DimMaster.GetManager<MapManager>().GetMainMapSpace();
		
		var constructSpriteComp = _constructableInst.GetComponentOfType<IConstructableSpriteProvider>();
		var sprite = constructSpriteComp.GetConstructablePreviewSprite(mapSpace, _currentSpot, _rotation);
		this.Texture = (ImageTexture)sprite.Sprite;
		this.Offset = new Vector2(sprite.XOffset, sprite.YOffset);
	}
}
