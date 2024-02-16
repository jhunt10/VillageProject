using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Terrain;
using VillageProject.Core.Sprites.PatchSprites;
using VillageProject.Godot.Sprites;

public partial class MapNode : Node
{
	private MapSpace MapSpace;
	private TerrainManager TerrainManager;

	private Dictionary<int, Node2D> ZLayers = new Dictionary<int, Node2D>();
	private Dictionary<int, Sprite2D> ZLayerShadows = new Dictionary<int, Sprite2D>();
	private Dictionary<int, Node2D> ZTerrainShadows = new Dictionary<int, Node2D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MapSpace = new MapSpace();
		// TerrainManager = DimMaster.GetManager<TerrainManager>();
		// for(int x = 0; x < 10; x++)
		// for (int y = 0; y < 10; y++)
		// {
		// 	var spot = new MapSpot(x, y, 0);
		// 	var index = (int)(GD.Randi() % TerrainManager._terrainInsts.Count);
		// 	var key = TerrainManager._terrainInsts.Keys.ToList()[index];
		// 	TerrainManager.TerrainMap.Add(spot,key);
		// }
		
		GenerateMap();

		foreach (var spot in MapSpace.EnumerateMapSpots())
		{
			CreateTerrainNode(spot);
		}
			
		
	}

	public void GenerateMap()
	{
		var maxX = 30;
		var maxY = 20;
		var maxZ = 10;
		MapSpace._buildCellMatrix(maxX,0,maxY,0,maxZ,0);
		
		TerrainManager = DimMaster.GetManager<TerrainManager>();
		var terrainNoise = new FastNoiseLite();
		terrainNoise.Frequency = (float)0.05;
		var hightNoise = new FastNoiseLite();
		hightNoise.Seed = terrainNoise.Seed + 1;
		hightNoise.Frequency = (float)0.05;
		
		for(int x = 0; x < 30; x++)
		for (int y = 0; y < 20; y++)
		{
			var terrainVal = terrainNoise.GetNoise2D(x, y);
			var index = 0;//(int)(GD.Randi() % TerrainManager._terrainInsts.Count);
			if (terrainVal > 0)
				index = 1;

			var hightVal = hightNoise.GetNoise2D(x, y);
			var hight = 5 - (int)(hightVal * 10);
			
			var terrainInst = TerrainManager._terrainInsts.Values.ToList()[index];
			for(int z = 0; z < hight; z++)
				MapSpace.SetTerrainAtSpot(terrainInst, new MapSpot(x, y, z));
		}
	}

	private double zTimmer = 0;
	private double zDelay = 2;
	private int currentZ = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		zTimmer += delta;
		if (zTimmer > zDelay)
		{
			zTimmer = 0;
			var maxZ = ZLayers.Keys.Max();
			var minZ = ZLayers.Keys.Min();
			currentZ += 1;
			if (currentZ == maxZ)
				currentZ = minZ;
			else
				currentZ += 1;
		
			for (int z = minZ; z <= maxZ; z++)
				if (ZLayers.ContainsKey(z))
				{
					ZTerrainShadows[z].Visible = z == (currentZ + 1);
					if (z <= currentZ)
					{
						ZLayers[z].Visible = true;
						ZLayerShadows[z].Visible = true;
					}
					else
					{
						ZLayers[z].Visible = false;
						ZLayerShadows[z].Visible = false;
					}
				}
		}
	}

	private void CreateZLayer(int z)
	{
		var node = new Node2D();
		AddChild(node);
		node.Position = new Vector2(0, -40 * z);
		ZLayers.Add(z, node);
		
		var shadow = CreateZLayerShadow();
		node.AddChild(shadow);
		shadow.Position = new Vector2(0, 40 * z);
		ZLayerShadows.Add(z, shadow);

		var terrainShadows = new Node2D();
		AddChild(terrainShadows);
		terrainShadows.Position = new Vector2(0, -40 * (z - 1));
		ZTerrainShadows.Add(z, terrainShadows);
	}

	private Sprite2D CreateZLayerShadow()
	{
		var size = this.GetViewport().GetWindow().Size;
		var image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		for(int x = 0; x < size.X; x++)
			for(int y = 0; y < size.Y; y++)
				image.SetPixel(x, y, new Color(0,0,0,(float)0.1));
		var newNode = new Sprite2D();
		newNode.Texture = ImageTexture.CreateFromImage(image);
		newNode.Centered = false;
		return newNode;
	}

	private Sprite2D CreateTerrainShadow()
	{
		var size = new Vector2I(32,32);
		var image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		for(int x = 0; x < size.X; x++)
		for(int y = 0; y < size.Y; y++)
			image.SetPixel(x, y, new Color(0,0,0,(float)0.3));
		var newNode = new Sprite2D();
		newNode.Texture = ImageTexture.CreateFromImage(image);
		newNode.Centered = false;
		return newNode;
	}

	private object CreateTerrainNode(MapSpot spot)
	{
		var inst = MapSpace.GetTerrainAtSpot(spot);
		if (inst == null)
			return null;
		
		var topSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("TopSprite");
		var topSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var topSprite = topSpriteComp.GetPatchSprite(() =>
		{
			return TerrainManager.GetHorizontalAdjacency(MapSpace, spot);
		});

		var frontSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var frontSpriteDef = frontSpriteComp.CompDef as IPatchSpriteCompDef;
		var frontSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return TerrainManager.GetVerticalAdjacencyAsHorizontal(MapSpace, spot);
		});
		
		var shadowSpriteComp = inst.GetComponentWithKey<GodotPatchCellSpriteComp>("FrontSprite");
		var shadowSpriteDef = topSpriteComp.CompDef as IPatchSpriteCompDef;
		var shadowSprite = frontSpriteComp.GetPatchSprite(() =>
		{
			return TerrainManager.GetVerticalAdjacencyAsHorizontal(MapSpace, spot);
		});
		
		var newNode = new Node2D(); // Create a new Sprite2D.
		if (!ZLayers.ContainsKey(spot.Z))
		{
			CreateZLayer(spot.Z);
		}
		ZLayers[spot.Z].AddChild(newNode);
		
		var topSpriteNode = new Sprite2D();
		newNode.AddChild(topSpriteNode);
		topSpriteNode.Texture = (ImageTexture)topSprite.Sprite;
		topSpriteNode.Centered = false;
		
		var frontSpriteNode = new Sprite2D();
		newNode.AddChild(frontSpriteNode);
		frontSpriteNode.Texture = (ImageTexture)frontSprite.Sprite;
		frontSpriteNode.Position = new Vector2(0, topSpriteDef.SpriteHight);
		frontSpriteNode.Centered = false;
		
		
		var pos = new Vector2(spot.X * topSpriteDef.SpriteWidth, spot.Y * topSpriteDef.SpriteHight);
		newNode.Position = pos;

		var terrainShadow = CreateTerrainShadow();
		ZTerrainShadows[spot.Z].AddChild(terrainShadow);
		terrainShadow.Position = new Vector2(spot.X * topSpriteDef.SpriteWidth, (spot.Y * topSpriteDef.SpriteHight)); ;
		
		return newNode;
	}
}
