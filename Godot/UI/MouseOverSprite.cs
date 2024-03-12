using Godot;
using System;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;

public partial class MouseOverSprite : Sprite2D
{
	private MapSpot? _lastSpot;
	public Label TextLabel;
	public CanvasLayer Canvas;
	
	public static MapSpot? MosueOverSpot {get; private set; }
	
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
		if (!mouseSpot.HasValue)
		{
			this.Visible = false;
			return;
		}
		
		var node = mapNode.GetMapNodeAtSpot(mouseSpot.Value);
		if(node == null)
			return;
		
		this.Position = Vector2.Zero;
		if (this.GetParent() != null)
			this.Reparent(node, false);
		else
			node.AddChild(this);
		_lastSpot = mouseSpot;
		this.Visible = true;
		MosueOverSpot = _lastSpot;
		UpdateText();
	}

	public void UpdateText()
	{
		TextLabel.Text = MosueOverSpot.ToString();

		if(!MosueOverSpot.HasValue)
			return;;
		
		var sb = TextLabel.Text;
		var mapNode = GameMaster.MapNode;
		var insts = mapNode.MapSpace.ListInstsAtSpot(MosueOverSpot.Value);
		foreach (var inst in insts)
		{
			sb += $"\n{inst._DebugId}";
			var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
			if (mapStructComp != null)
			{
				sb += $"\n{mapStructComp.MapSpot} {mapStructComp.Rotation}";
			}
		}

		TextLabel.Text = sb;

	}
}
