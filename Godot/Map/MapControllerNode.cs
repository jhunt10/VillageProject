using Godot;
using System;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapGeneration;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Godot.Map;

public partial class MapControllerNode : Node2D
{
	private Dictionary<string, IMapObjectNode> _mapObjectNodes = new Dictionary<string, IMapObjectNode>();
	private Dictionary<string, MapNode> _mapNodes = new Dictionary<string, MapNode>();
	public MapNode MapNodePrefab;
	
	public MouseOverSprite MouseOverSprite;
	public ConstructablePreview ConstructablePreview;
	
	public static MapStructureNode MapStructureNodePrefab;
	public static MapCellNode MapCellPrefab;
	public static TerrainNode TerrainNodePrefab;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseOverSprite = GetNode<MouseOverSprite>("MouseOverSprite");
		ConstructablePreview = GetNode<ConstructablePreview>("ConstructablePreview");
		
		MapStructureNodePrefab = GetNode<MapStructureNode>("PrefabNodes/MapStructureNodePrefab");
		MapCellPrefab = GetNode<MapCellNode>("PrefabNodes/MapCellPrefab");
		TerrainNodePrefab = GetNode<TerrainNode>("PrefabNodes/TerrainNodePrefab");
		MapNodePrefab = GetNode<MapNode>("MapNode");
		
		var mapManager = DimMaster.GetManager<MapManager>();
		var mapSpace = BasicMapGenerator.GenerateTestMap();
		_mapNodes.Add(mapSpace.MapSpaceId, MapNodePrefab);
		MapNodePrefab.LoadMap(mapSpace);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CreateMapNode(MapSpaceCompInst mapSpaceCompInst)
	{
		if(_mapNodes.ContainsKey(mapSpaceCompInst.MapSpaceId))
			return;
	}

	public void CreateNewMapStructureNode(IInst mapStructInst)
	{
		if (_mapObjectNodes.ContainsKey(mapStructInst.Id))
		{
			Console.WriteLine($"MapObjInst '{mapStructInst._DebugId}' attempted to make MapStructNode again.");
			return;
		}
		
		var mapStructComp = mapStructInst.GetComponentOfType<MapStructCompInst>(errorIfNull: true);
		if (mapStructComp == null)
			throw new Exception($"Inst {mapStructInst._DebugId} does not have a MapStructCompInst.");
		
		var newNode = (MapStructureNode)MapStructureNodePrefab.Duplicate();
		newNode.SetInst(mapStructInst);
		newNode.Visible = true;
		_mapObjectNodes.Add(mapStructInst.Id, newNode);
		//Inst has not yet been placed on a map
		if (string.IsNullOrEmpty(mapStructComp.MapSpaceId) || !mapStructComp.MapSpot.HasValue
		    ||  !_mapNodes.ContainsKey(mapStructComp.MapSpaceId))
		{
			// MapSpaces need to be implemented as Insts, otherwise they can't use load logic proeperly and we get stuck here
			var t = true;
		}
		else
		{
			var mapNode = _mapNodes[mapStructComp.MapSpaceId];
			var spot = mapStructComp.MapSpot.Value;
			var cellNode = mapNode.GetMapNodeAtSpot(spot);
			cellNode.AddMapObjectNode(newNode);
			newNode.ForceUpdateSprite();
			// newNode.UpdateSprite(mapStructComp.MapSpace, spot, mapStructComp.Rotation);
		}
	}

	public MapNode GetMapNode(string mapSpaceId)
	{
		if (_mapNodes.ContainsKey(mapSpaceId))
			return _mapNodes[mapSpaceId];
		return null;
	}

	public void ClearMaps()
	{
		// Need to pull UI element so they don't get wiped out with the layers
		if (MouseOverSprite.GetParent() != null && MouseOverSprite.GetParent() != this)
			MouseOverSprite.Reparent(this, false);
		else if(MouseOverSprite.GetParent() == null)
			this.AddChild(this);
		
		if (ConstructablePreview.GetParent() != null && ConstructablePreview.GetParent() != this)
			ConstructablePreview.Reparent(this, false);
		else if(ConstructablePreview.GetParent() == null)
			this.AddChild(this);

		// MapNodePrefab.ClearMap();
		// _mapNodes.Clear();
	}

	public void LoadMap(MapSpaceCompInst mapSpace)
	{
		var newMapNode = (MapNode)MapNodePrefab.Duplicate();
		newMapNode.Visible = true;
		this.AddChild(newMapNode);
		_mapNodes.Add(mapSpace.MapSpaceId, newMapNode);
		newMapNode.LoadMap(mapSpace);

		foreach (var inst in mapSpace.EnumerateAllInsts())
		{
			if (_mapObjectNodes.ContainsKey(inst.Id))
			{
				var objNode = _mapObjectNodes[inst.Id];
				objNode.ForceUpdateSprite();
			}
		}
	}
}
