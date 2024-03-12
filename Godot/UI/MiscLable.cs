using Godot;
using System;

public partial class MiscLable : Control
{
	public Label Label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var viewMapSpace = GameMaster.MainCamera.CenterMapSpot;
		var viewRotaion = GameMaster.MainCamera.FacingDirection;
		Label.Text = $"Map View: {viewMapSpace} {viewRotaion}";

	}
}
