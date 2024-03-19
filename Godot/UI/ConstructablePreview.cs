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
		var spot = GameMaster.MapControllerNode?.GetMouseOverMapSpot();
		// var node = GameMaster.MapControllerNode?.GetMouseOverCell();;
		if (spot == null || spot == _currentSpot)
			return;
		
		// this.Position = Vector2.Zero;
		// if (this.GetParent() != null)
		// 	this.Reparent(node, false);
		// else
		// 	node.AddChild(this);
		_currentSpot = spot.Value;
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
		// Create instance of constructable on mouse click
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if(!MouseOverSprite.MosueOverSpot.HasValue)
				return;
			if(_constructableDef == null)
				return;
			GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
			if (eventMouseButton.Pressed)
			{
				var mapNode = GameMaster.MapControllerNode.GetMouseOverMapNode();
				if (mapNode != null)
				{
					var realRot = _rotation.AddRotation(mapNode.ViewRotation);
					GameMaster.CreateInstAtSpot(
						_constructableDef,
						mapNode.MapSpace,
						_currentSpot,
						realRot);
				}
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
		this.Offset = new Vector2(sprite.XOffset, (-sprite.Hight) + sprite.YOffset);
		this.Visible = true;

		var mapNode = GameMaster.MapControllerNode.GetMouseOverMapNode();
		var canPlace = false;
		var message = "";

		if (mapNode != null)
		{
			var realRot = _rotation.AddRotation(mapNode.ViewRotation);
			var res = DimMaster.GetManager<MapManager>()
				.CouldPlaceDefOnMapSpace(mapNode.MapSpace, _constructableDef, _currentSpot, realRot);
			canPlace = res.Success;
			message = res.Message;

		}
		if (canPlace)
		{
			this.Modulate = Colors.Green;
			Label.Text = message;
		}
		else
		{
			this.Modulate = Colors.Red;
			Label.Text = message;
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
