using Godot;
using System;
using VillageProject.Core;

public partial class Node2D : Godot.Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var test = new TestClass();
		GD.Print(test.GetTestString());

		this.Position = this.Position + new Vector2(1, 1);
	}
}