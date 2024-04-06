using Godot;
using System;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.Pathing;

public partial class PathDisplayNode : Node2D
{
	public Sprite2D PathTilePrefab { get; set; }

	private Dictionary<MapSpot, Sprite2D> _pathTiles { get; set; } = new Dictionary<MapSpot, Sprite2D> ();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PathTilePrefab = GetNode<Sprite2D>("PathTilePrefab");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DisplayPath(IMapSpace mapSpace, MapSpot spotA, MapSpot spotB)
	{
		var time = DateTime.Now;
		Console.WriteLine($"PathDisplayNode: Start {time}");
		foreach (var pair in _pathTiles)
		{
			try
			{
				pair.Value.QueueFree();
			}
			// Catch if it's already been deleted
			catch (System.ObjectDisposedException e)
			{
				
			}
		}
		_pathTiles.Clear();
		Console.WriteLine($"PathDisplayNode: Clear Path {time - DateTime.Now}");
		var path = PathFinder.FindPath(mapSpace, null, spotA, spotB, cacheSearchedCells: true);
		if (!path.Successful)
		{
			var realSpot = PathFinder.CachedSearchedCells.MaxBy(pair => pair.Key.DistanceToSpot(spotA)).Value.Spot;
			path = PathFinder.FindPath(mapSpace, null, realSpot, spotB, cacheSearchedCells: true);
		}

		foreach (var pathedSpot in path.ListAllSpots())
		{
			_BuildPathNode(mapSpace, pathedSpot, true);
		}
		
		foreach (var searched in PathFinder.CachedSearchedCells)
		{
			_BuildPathNode(mapSpace, searched.Key, false);
		}
	}

	private void _BuildPathNode(IMapSpace mapSpace, MapSpot spot, bool isPathed)
	{
		if(_pathTiles.ContainsKey(spot))
			return;
		
		var mapNode = GameMaster.MapControllerNode.GetMapNode(mapSpace.MapSpaceId);
		if (mapNode == null)
			throw new Exception("Failed to find MapNode");
		var cellNode = mapNode.GetMapCellNodeAtSpot(spot);
		if(cellNode == null)
			// throw new Exception("Failed to find CellNode");
			return;

		var newSprite = (Sprite2D)PathTilePrefab.Duplicate();
		newSprite.Visible = true;
		cellNode.AddChild(newSprite);
		if (isPathed)
			newSprite.Modulate = new Color(0, 1, 0, 0.5f);
		else
			newSprite.Modulate = new Color(1, 0, 0, 0.5f);
		_pathTiles.Add(spot, newSprite);
	}
}
