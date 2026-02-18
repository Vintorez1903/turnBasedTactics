using Godot;
using System;
using System.Collections.Generic;
using static sharedTypes;
using static JSONParsing;

public partial class MainMapLayer : TileMapLayer
{
	[Export]
	public Node2D units;
	private Unit selectedUnit;
	
	[Export]
	public TileMapLayer commandLayer;
	private ValidMoveDisplay validMoveDisplay;
	
	private Unit[,] unitLocations;
	private AStarPathfinder pathfinder;
	
	private float charMoveSpeed=100f;
	
	Vector2I[] movementPath;
	Vector2I pathNode;
	private int pathCost = 0;
	private int currentPathNode = 0;
	
	private bool unitMoving = false;
	private bool unitSelected = false;
	
	private int numPlayers = 2;
	private int currentTurn = 0;
	
	
	public override void _Ready(){
		Vector2I mapSize = GetUsedRect().Size;
		unitLocations = new Unit[mapSize.X,mapSize.Y];
		initUnits();
		validMoveDisplay = new ValidMoveDisplay(commandLayer,this);
		pathfinder = new AStarPathfinder(this);
	}
	
	public override void _PhysicsProcess(double delta){
		float fdelta = (float)delta;
		if(unitSelected){
			if(unitMoving){
				doUnitMovement(fdelta);
			}
		}
	}
	
	public override void _Input(InputEvent @event){
		if(Input.IsActionJustPressed("endTurn")){
			endTurn();
		}
	}
	
	//logic for initializing units
	private void initUnits(){
		Godot.Collections.Array<Node> children = units.GetChildren();
		
		foreach(Unit child in children){
			Vector2I cell = LocalToMap(child.GlobalPosition);
			initFromResFile(child);
			child.Position=MapToLocal(cell);
			cell=cell-GetUsedRect().Position;
			unitLocations[cell.X,cell.Y]=child;
		}
	}
	
	private void initFromResFile(Unit unit){
		UnitStats unitStats = unitStatsParse(unit.GetMeta("UnitStats").ToString());
		int ownedBy = 0;
		
		if(unit.Name.ToString().Contains("TankBlue")){
			ownedBy=1;
		}
		
		unit.initUnit(
			unitStats.MovementRange,
			unitStats.MovementType,
			unitStats.Health,
			ownedBy
		);
	}
	
	public bool isOccupied(Vector2I cell){
		cell-=(GetUsedRect().Position);
		if(unitLocations[cell.X,cell.Y]!=null){
			return true;
		}
		return false;
	}
	
	//logic for changing unit positions
	public void moveUnit(Vector2I newLocation){
		if(selectedUnit==null){
			return;
		}
		(pathCost, movementPath) = pathfinder.pathfind(LocalToMap(selectedUnit.Position),newLocation,selectedUnit.getMovementType());
		if(selectedUnit.getMovementRange()>=pathCost && !isOccupied(newLocation)){
			currentPathNode=0;
			pathNode=movementPath[currentPathNode];
			unitMoving=true;
			selectedUnit.setMovedStatus(true);
			validMoveDisplay.clearDisplayedMoves();
		}
	}
	
	private void doUnitMovement(float delta){
		Vector2 cellPosition = MapToLocal(pathNode);
		Vector2 targetBearing = cellPosition-selectedUnit.Position;
		Vector2 movement = targetBearing.Normalized();
		movement=movement*charMoveSpeed*delta;
		
		if(movement.Length()<targetBearing.Length()){
			selectedUnit.Position+=movement;
		}
		else{
			selectedUnit.Position=cellPosition;
			updateUnitPosition(selectedUnit,pathNode);
			if(movementPath.Length>currentPathNode+1){
				currentPathNode++;
				pathNode=movementPath[currentPathNode];
			}
			else{
				deselectUnit();
			}
		}
	}
	
	Vector2I initialNode;
	public void updateUnitPosition(Unit unit,Vector2I newLocation){
		Vector2I cell=initialNode-GetUsedRect().Position;
		unitLocations[cell.X,cell.Y]=null;
		
		Vector2I newCell=newLocation-GetUsedRect().Position;
		unitLocations[newCell.X,newCell.Y]=unit;
		unit.GlobalPosition=MapToLocal(newLocation);
		initialNode=newLocation;
	}
	
	//logic for selecting units
	public void selectUnit(Vector2I cell){
		Rect2I usedRect = GetUsedRect();
		
		//deselect a unit if one's already selected
		if(selectedUnit!=null){
			deselectUnit();
			validMoveDisplay.clearDisplayedMoves();
		}
		
		//check if something's actually there
		cell=cell-usedRect.Position;
		if(unitLocations[cell.X,cell.Y]==null){
			return;
		}
		
		//check if we're actually allowed to select it
		selectedUnit=unitLocations[cell.X,cell.Y];
		if(selectedUnit.getMovedStatus()==true || selectedUnit.getTeamID()!=currentTurn){
			return;
		}
		
		unitLocations[cell.X,cell.Y].selectUnit();
		unitSelected=true;
		initialNode=cell+GetUsedRect().Position;
			
			
		validMoveDisplay.displayValidMoves(selectedUnit.getMovementRange(),cell+GetUsedRect().Position, selectedUnit.getMovementType());
	}
	
	public void deselectUnit(){
		unitSelected=false;
		unitMoving=false;
		if(selectedUnit!=null){
			selectedUnit.deselectUnit();
			selectedUnit=null;
		}
	}
	
	//logic for handling turns
	public void endTurn(){
		Godot.Collections.Array<Node> children = units.GetChildren();
		
		currentTurn++;
		if(currentTurn>=numPlayers){
			currentTurn=0;
		}
		
		foreach(Unit child in children){
			child.setMovedStatus(false);
		}
	}
}
