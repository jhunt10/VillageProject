using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;

public partial class ConstructablePreview : Sprite2D
{
	public Label Label;
	
	private IDef _constructableDef;
	private MapSpot _currentSpot;
	private RotationFlag _rotation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
		UpdateSprite();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mapNode = GameMaster.MapNode;
		if(mapNode == null)
			return;
		
		var mouseSpot = mapNode.GetMouseOverMapSpot() + new MapSpot(0,0,1);
		if(!mouseSpot.HasValue || mouseSpot == _currentSpot)
			return;
		
		var node = mapNode.GetMapNodeAtSpot(mouseSpot.Value);
		if (node == null)
			return;
		
		this.Position = Vector2.Zero;
		if (this.GetParent() != null)
			this.Reparent(node, false);
		else
			node.AddChild(this);
		_currentSpot = mouseSpot.Value;
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
		// _constructableInst = DimMaster.InstantiateDef(_constructableDef);
	}

	public void UpdateSprite()
	{
		if (_constructableDef == null)
		{
			this.Visible = false;
			return;
		}

		var sprite = GetSpite();
		this.Texture = (ImageTexture)sprite.Sprite;
		this.Offset = new Vector2(sprite.XOffset, sprite.YOffset);
		this.Visible = true;

		var mapSpace = GameMaster.MapNode.MapSpace;
		var realRot = _rotation.AddRotation(GameMaster.MapNode.ViewRotation);
		var canPlace = DimMaster.GetManager<MapManager>()
			.CouldPlaceDefOnMapSpace(mapSpace, _constructableDef, _currentSpot, realRot);
		if (canPlace.Success)
		{
			this.Modulate = Colors.Green;
			Label.Text = "";
		}
		else
		{
			this.Modulate = Colors.Red;
			Label.Text = canPlace.Message;
		}
	}

	public SpriteData GetSpite()
	{
		var constructableDef = _constructableDef.GetComponentDefOfType<ConstructableCompDef>(errorIfNull:true);
		var spriteDef = constructableDef.DefaultSprite;

		var rotaion = _rotation;
		if (constructableDef.RotationSprites.ContainsKey(rotaion))
			spriteDef = constructableDef.RotationSprites[rotaion];
        
		var spritePath = Path.Combine(constructableDef.ParentDef.LoadPath, spriteDef.SpriteName);

		var image = Image.LoadFromFile(spritePath);
		if (image == null)
			throw new Exception($"Failed to load image from '{spritePath}'.");
        
		return new SpriteData(ImageTexture.CreateFromImage(image), spriteDef);
	}
}
