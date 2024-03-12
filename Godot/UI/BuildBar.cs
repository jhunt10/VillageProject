using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Sprites.Interfaces;

public partial class BuildBar : Control
{
	public const int BUTTON_PADDING = 8;
	public const int BUTTON_WIDTH = 72;
	public const int BUTTON_GAP = 8;

	// public ConstructablePreview ConstructablePreview;
	public NinePatchRect Background;
	public TextureButton ButtonPrefab;
	public List<TextureButton> Buttons;

	public IInst ConructionInst;
	private List<IDef> _constructableDefs = new List<IDef>(); 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Background = GetNode<NinePatchRect>("Background");
		Buttons = new List<TextureButton>();
		ButtonPrefab = GetNode<TextureButton>("BuildableDefButtonPrefab");
		ButtonPrefab.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Buttons.Count == 0)
			LoadButtons();
	}

	private void LoadButtons()
	{
		foreach (var but in Buttons)
		{
			but.QueueFree();
			this.RemoveChild(but);
		}
		Buttons.Clear();
		_constructableDefs.Clear();
		var defs = DimMaster.GetAllDefsWithCompDefType<ConstructableCompDef>();
		foreach (var def in defs)
		{
			var index = Buttons.Count;
			var constrDef = def.GetComponentDefOfType<ConstructableCompDef>();
			var imagePath = Path.Combine(def.LoadPath, constrDef.IconSprite.SpriteName);
			var image = Image.LoadFromFile(imagePath);

			var newButton = (TextureButton)ButtonPrefab.Duplicate();
			this.AddChild(newButton);
			newButton.Position = new Vector2(
				BUTTON_PADDING + (Buttons.Count * (BUTTON_WIDTH + BUTTON_GAP)),
				BUTTON_PADDING);
			newButton.Pressed += () => _on_def_button_press(index);
			newButton.GetNode<TextureRect>("TextureRect").Texture = ImageTexture.CreateFromImage(image);
			newButton.Visible = true;
			Buttons.Add(newButton);
			_constructableDefs.Add(def);

		}

		var width = (defs.Count() * BUTTON_WIDTH) + ((defs.Count()-1) * BUTTON_GAP) + (2 * BUTTON_PADDING);
		var hight = BUTTON_WIDTH + (2 * BUTTON_PADDING);
		var size = new Vector2(width, hight);
		this.Size = size;
		Background.Size = size;

	}

	private void _on_def_button_press(int index)
	{
		var button = Buttons[index];
		// GameMaster.MouseOverSprite.Texture = button.GetNode<TextureRect>("TextureRect").Texture;
		GameMaster.MapControllerNode.ConstructablePreview.SetConstructableDef(_constructableDefs[index]);

	}
}
