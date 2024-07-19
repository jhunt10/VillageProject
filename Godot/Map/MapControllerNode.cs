using Godot;
using System;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapGeneration;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Godot;
using VillageProject.Godot.InstNodes;
using VillageProject.Godot.Map;

public partial class MapControllerNode : Node2D
{
	private Dictionary<string, IMapObjectNode> _mapObjectNodes = new Dictionary<string, IMapObjectNode>();
	private Dictionary<string, MapNode> _mapNodes = new Dictionary<string, MapNode>();
	public MapNode MapNodePrefab;
	
	public MouseOverSprite MouseOverSprite;
	// public ConstructablePreview ConstructablePreview;

	public static ActorNode ActorNodePrefab;
	public static MapStructureNode MapStructureNodePrefab;
	public static MapCellNode MapCellPrefab;
	
	private static Vector2 _lastMousePos = Vector2.Zero;
	private static MapSpot? _lastMouseMapSpot = null;
	private static string _lastMouseMapNodeId = null;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseOverSprite = GetNode<MouseOverSprite>("MouseOverSprite");
		// ConstructablePreview = GetNode<ConstructablePreview>("ConstructablePreview");
		
		MapStructureNodePrefab = GetNode<MapStructureNode>("PrefabNodes/MapStructureNodePrefab");
		MapCellPrefab = GetNode<MapCellNode>("PrefabNodes/MapCellPrefab");
		MapNodePrefab = GetNode<MapNode>("MapNode");
		
		var mapManager = DimMaster.GetManager<MapManager>();
		
		// Build Test Map
		var mapSpace = BasicMapGenerator.GenerateTestMap();
		if (!_mapNodes.ContainsKey(mapSpace.MapSpaceId))
		{
			_mapNodes.Add(mapSpace.MapSpaceId, MapNodePrefab);
			MapNodePrefab.LoadMap(mapSpace);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlaceInstNodeOnMap(IInstNode instNode)
	{
		var inst = instNode.Inst;
		
		var mapPositionComp = inst.GetComponentOfType<IMapPositionComp>(activeOnly:true, errorIfNull: false);
		if (mapPositionComp == null)
			throw new Exception($"Inst {inst._DebugId} does not have a IMapPositionComp.");
		if(string.IsNullOrEmpty(mapPositionComp.MapSpaceId))
			throw new Exception($"Inst {inst._DebugId} is not placed on map.");
		
		var mapNode = _mapNodes[mapPositionComp.MapSpaceId];
		var spot = mapPositionComp.MapSpot.Value;
		var cellNode = mapNode.GetMapCellNodeAtSpot(spot);
		cellNode.AddInstNode(instNode);
		
	}
	
	// public Old_IInstNode CreateNewMapStructureNode(IInst mapStructInst)
	// {
	// 	if (_mapObjectNodes.ContainsKey(mapStructInst.Id))
	// 	{
	// 		Console.WriteLine($"MapObjInst '{mapStructInst._DebugId}' attempted to make MapStructNode again.");
	// 		return null;
	// 	}
	// 	
	// 	var mapStructComp = mapStructInst.GetComponentOfType<MapStructCompInst>(activeOnly:false, errorIfNull: false);
	// 	if (mapStructComp == null)
	// 		throw new Exception($"Inst {mapStructInst._DebugId} does not have a MapStructCompInst.");
	// 	
	// 	var newNode = (MapStructureNode)MapStructureNodePrefab.Duplicate();
	// 	newNode.SetInst(mapStructInst);
	// 	newNode.Visible = true;
	// 	_mapObjectNodes.Add(mapStructInst.Id, newNode);
	// 	//Inst has not yet been placed on a map
	// 	if (string.IsNullOrEmpty(mapStructComp.MapSpaceId) || !mapStructComp.MapSpot.HasValue
	// 	    ||  !_mapNodes.ContainsKey(mapStructComp.MapSpaceId))
	// 	{
	// 		
	// 		this.AddChild(newNode);
	// 	}
	// 	else
	// 	{
	// 		var mapNode = _mapNodes[mapStructComp.MapSpaceId];
	// 		var spot = mapStructComp.MapSpot.Value;
	// 		var cellNode = mapNode.GetMapCellNodeAtSpot(spot);
	// 		cellNode.AddMapObjectNode(newNode);
	// 		newNode.ForceUpdateSprite();
	// 		// newNode.UpdateSprite(mapStructComp.MapSpace, spot, mapStructComp.Rotation);
	// 	}
	//
	// 	return newNode;
	// }
	
	// public Old_IInstNode CreateNewActorNode(IInst mapStructInst)
	// {
	// 	
	// 	if (_mapObjectNodes.ContainsKey(mapStructInst.Id))
	// 	{
	// 		Console.WriteLine($"ActorInst '{mapStructInst._DebugId}' attempted to make ActorNode again.");
	// 		return null;
	// 	}
	// 	
	// 	var actorComp = mapStructInst.GetComponentOfType<ActorCompInst>(errorIfNull: true);
	// 	if (actorComp == null)
	// 		throw new Exception($"Inst {mapStructInst._DebugId} does not have a ActorCompInst.");
	// 	
	// 	var newNode = (ActorNode)ActorNodePrefab.Duplicate();
	// 	newNode.SetInst(mapStructInst);
	// 	newNode.Visible = true;
	// 	_mapObjectNodes.Add(mapStructInst.Id, newNode);
	// 	var mapPos = actorComp.MapPosition;
	// 	//Inst has not yet been placed on a map
	// 	if (!mapPos.HasValue)
	// 	{
	// 		this.AddChild(newNode);
	// 	}
	// 	else
	// 	{
	// 		var mapNode = _mapNodes[mapPos.Value.MapSpaceId];
	// 		var spot = mapPos.Value.MapSpot;
	// 		var cellNode = mapNode.GetMapCellNodeAtSpot(spot);
	// 		cellNode.AddMapObjectNode(newNode);
	// 		newNode.ForceUpdateSprite();
	// 		// newNode.UpdateSprite(mapStructComp.MapSpace, spot, mapStructComp.Rotation);
	// 	}
	// 	Console.WriteLine($"Created New Actor Node: {newNode.GetInstanceId()}");
	// 	return newNode;
	// }

	public MapNode GetMainMapNode()
	{
		return _mapNodes.First().Value;
	}

	public MapNode GetMapNode(string mapSpaceId)
	{
		if (!string.IsNullOrEmpty(mapSpaceId) && _mapNodes.ContainsKey(mapSpaceId))
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
		
		// if (ConstructablePreview.GetParent() != null && ConstructablePreview.GetParent() != this)
		// 	ConstructablePreview.Reparent(this, false);
		// else if(ConstructablePreview.GetParent() == null)
		// 	this.AddChild(this);

		// MapNodePrefab.ClearMap();
		// _mapNodes.Clear();
	}

	public Old_IInstNode LoadMap(MapSpaceCompInst mapSpace)
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

		return newMapNode;
	}

	public void NotifyOfDeletedInst(IInst inst)
	{
		if (_mapNodes.ContainsKey(inst.Id))
		{
			
			// Need to pull UI element so they don't get wiped out with the layers
			if (MouseOverSprite.GetParent() != null && MouseOverSprite.GetParent() != this)
				MouseOverSprite.Reparent(this, false);
			else if(MouseOverSprite.GetParent() == null)
				this.AddChild(this);
		
			// if (ConstructablePreview.GetParent() != null && ConstructablePreview.GetParent() != this)
			// 	ConstructablePreview.Reparent(this, false);
			// else if(ConstructablePreview.GetParent() == null)
			// 	this.AddChild(this);
			
			var node = _mapNodes[inst.Id];
			// node.QueueFree();
			_mapNodes.Remove(inst.Id);
		}
		if (_mapObjectNodes.ContainsKey(inst.Id))
		{
			var node = _mapObjectNodes[inst.Id];
			// node.QueueFree();
			_mapObjectNodes.Remove(inst.Id);
		}
	}

	public MapCellNode GetMouseOverCell()
	{
		_syncMouseOver();
		if (_lastMouseMapSpot.HasValue &&
		    _mapNodes.ContainsKey(_lastMouseMapNodeId))
		{
			var mapNode = _mapNodes[_lastMouseMapNodeId];
			return mapNode.GetMapCellNodeAtSpot(_lastMouseMapSpot.Value);
		}
		return null;

	}

	public MapSpot? GetMouseOverMapSpot()
	{
		_syncMouseOver();
		return _lastMouseMapSpot;
	}

	public MapNode? GetMouseOverMapNode()
	{
		_syncMouseOver();
		if (!string.IsNullOrEmpty(_lastMouseMapNodeId) && _mapNodes.ContainsKey(_lastMouseMapNodeId))
			return _mapNodes[_lastMouseMapNodeId];
		return null;
		
	}

	private void _syncMouseOver()
	{
		var mousePos = GetViewport().GetMousePosition();
		var mainCamera = GameMaster.MainCamera;
		if (mainCamera == null)
			return;

		var relativePos = mousePos + mainCamera.Position;
		if(relativePos == _lastMousePos)
			return;
		
		_lastMouseMapSpot = null;
		_lastMouseMapNodeId = null;
		_lastMousePos = relativePos;
		foreach (var mapNode in _mapNodes.Values)
		{
			if (mapNode.InWorldBounds(relativePos))
			{
				
				var mapSpot = MapHelper.WorldPositionToMapSpot(
					mapNode.MapSpace, (int)relativePos.X, (int)relativePos.Y,
					mapNode.VisibleZLayer, mapNode.ViewRotation);
				if (mapSpot != null)
				{
					_lastMouseMapSpot = mapSpot;
					_lastMouseMapNodeId = mapNode.MapSpace.MapSpaceId;
				}
			}
		}
	}
}
