using Godot;
using System;
using VillageProject.Core.Map;

public partial class MouseOverSprite : Sprite2D
{
	private MapSpot _lastSpot = new MapSpot();
	public Label TextLabel;
	public CanvasLayer Canvas;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Canvas = GetNode<CanvasLayer>("CanvasLayer");
		TextLabel = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mapNode = GameMaster.MapNode;
		if(mapNode == null)
			return;
		
		var mouseSpot = mapNode.GetMouseOverMapSpot();
		if(mouseSpot == _lastSpot)
			return;
		
		var node = mapNode.GetTerrainNodeAtSpot(mouseSpot);
		if(node == null)
			return;
		TextLabel.Text = mouseSpot.ToString();
		this.Position = Vector2.Zero;
		if (this.GetParent() != null)
			this.Reparent(node, false);
		else
			node.AddChild(this);
		_lastSpot = mouseSpot;
	}
}
