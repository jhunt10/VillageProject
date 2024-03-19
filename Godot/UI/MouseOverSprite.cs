using Godot;
using System;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;

public partial class MouseOverSprite : Sprite2D
{
	private MapSpot? _lastSpot;
	public Label TextLabel;
	public CanvasLayer Canvas;

	public ConstructablePreview ConstructablePreview;
	
	public static MapSpot? MosueOverSpot {get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Canvas = GetNode<CanvasLayer>("CanvasLayer");
		TextLabel = GetNode<Label>("Label");
		ConstructablePreview = GetNode<ConstructablePreview>("ConstructablePreview");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouseSpot = GameMaster.MapControllerNode?.GetMouseOverMapSpot();
		var node = GameMaster.MapControllerNode?.GetMouseOverCell();
		if(mouseSpot.HasValue && node != null)
		{
			if (mouseSpot == _lastSpot)
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
		else
		{
			_lastSpot = null;
			TextLabel.Text = "";
			
			var parent = this.GetParent();
			if (parent == null)
			{
				GameMaster.MapControllerNode?.AddChild(this);
			}
			else if (parent != GameMaster.MapControllerNode)
			{
				this.Reparent(GameMaster.MapControllerNode, false);
			}

			this.Position = GetViewport().GetMousePosition() + GameMaster.MainCamera.Position +
			                new Vector2(0, MapNode.TILE_HIGHT);
		}
	}

	public void UpdateText()
	{
		TextLabel.Text = MosueOverSpot.ToString();

		if(!MosueOverSpot.HasValue)
			return;;
		
		var sb = TextLabel.Text;
		var mapNode = GameMaster.MapControllerNode.GetMouseOverMapNode();
		if (mapNode != null)
		{
			foreach (var inst in mapNode.MapSpace.ListInstsAtSpot(MosueOverSpot.Value))
			{
				sb += $"\n{inst._DebugId}";
				var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
				if (mapStructComp != null)
				{
					sb += $"\n{mapStructComp.MapSpot} {mapStructComp.Rotation}";
				}
			}
		}

		TextLabel.Text = sb;

	}
}
