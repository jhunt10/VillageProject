using Godot;
using System;

public partial class TerrainNode : Node2D
{
	private Sprite2D _shadowSprite;
	public Sprite2D ShadowSprite
	{
		get
		{
			if(_shadowSprite == null)
				_init();
			return _shadowSprite;
		}
	}
	
	private Sprite2D _topSprite;
	public Sprite2D TopSprite
	{
		get
		{
			if(_topSprite == null)
				_init();
			return _topSprite;
		}
	}
	
	private Sprite2D _frontSprite;
	public Sprite2D FrontSprite
	{
		get
		{
			if(_frontSprite == null)
				_init();
			return _frontSprite;
		}
	}

	private bool _inited;

	private void _init()
	{
		if(_inited)
			return;

		_shadowSprite = GetNode<Sprite2D>("ShadowSprite");
		_topSprite = GetNode<Sprite2D>("TopSprite");
		_frontSprite = GetNode<Sprite2D>("FrontSprite");
		
		_inited = true;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void SetShow(bool show, bool showShadows = false)
	{
		_topSprite.Visible = show;
		_frontSprite.Visible = show;
		_shadowSprite.Visible = showShadows;
	}
}
