using Godot;
using System;
using System.Collections.Generic;
using static sharedTypes;
using static JSONParsing;

public partial class MainMapLayer : TileMapLayer
{
	[Export] public Node2D units;
	private Unit selectedUnit;
	[Export] public TileMapLayer commandLayer;
	[Export] public Camera mainCam;
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
	OrderType currentOrder;
	Vector2I lastUnitPos;
	Vector2I initialUnitPos;
	UnitControlMenu unitControlMenu;
	
	public override void _Ready(){
		Vector2I mapSize = GetUsedRect().Size;
		unitLocations = new Unit[mapSize.X,mapSize.Y];
		initUnits();
		validMoveDisplay = new ValidMoveDisplay(commandLayer,this);
		pathfinder = new AStarPathfinder(this);
	}
	
	public override void _PhysicsProcess(double delta){
		float fdelta = (float)delta;
		if(unitSelected && unitMoving){
			doUnitMovement(fdelta);
		}
	}
	
	public override void _Input(InputEvent @event){
		if(Input.IsActionJustPressed("endTurn")){
			endTurn();
		}
		
		if(Input.IsActionJustPressed("giveOrder") && !unitControlMenu.isOpen()){
			executeOrder();
		}
	}
	
	//logic for giving orders
	private void executeOrder(){
		if(currentOrder==OrderType.MOVE){
			initialUnitPos=LocalToMap(selectedUnit.GlobalPosition);
			moveUnit(LocalToMap(GetLocalMousePosition()));
		}
	}
	
	//logic for initializing/managing units
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
		int ownedBy = 0;
		
		if(unit.Name.ToString().Contains("TankBlue")){
			ownedBy=1;
		}
		
		unit.initUnit(ownedBy);
	}
	
	public bool isOccupied(Vector2I cell){
		cell-=(GetUsedRect().Position);
		if(unitLocations[cell.X,cell.Y]!=null){
			return true;
		}
		return false;
	}
	
	//logic for intializing/managing menus
	public void recieveControlMenu(UnitControlMenu controlIn){
		unitControlMenu = controlIn;
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
				mainCam.moveUnitControlMenu(movementPath[currentPathNode]);
				unitMoving=false;
			}
		}
	}
	
	public void updateUnitPosition(Unit unit,Vector2I newLocation){
		Vector2I cell=lastUnitPos-GetUsedRect().Position;
		unitLocations[cell.X,cell.Y]=null;
		
		Vector2I newCell=newLocation-GetUsedRect().Position;
		unitLocations[newCell.X,newCell.Y]=unit;
		unit.GlobalPosition=MapToLocal(newLocation);
		lastUnitPos=newLocation;
	}
	
	public void cancelOrder(){
		updateUnitPosition(selectedUnit,initialUnitPos);
		validMoveDisplay.displayValidMoves(selectedUnit.getMovementRange(),initialUnitPos, selectedUnit.getMovementType());
	}
	
	//logic for selecting units
	public int selectUnit(Vector2I cell){
		Rect2I usedRect = GetUsedRect();
		
		//check if something's actually there
		cell=cell-usedRect.Position;
		if(unitLocations[cell.X,cell.Y]==null){
			return 1;
		}
		
		//deselect a unit if one's already selected
		if(selectedUnit!=null){
			deselectUnit();
			validMoveDisplay.clearDisplayedMoves();
		}
		
		//check if we're actually allowed to select it
		selectedUnit=unitLocations[cell.X,cell.Y];
		if(selectedUnit.getMovedStatus()==true || selectedUnit.getTeamID()!=currentTurn){
			return 2;
		}
		
		setCurrentOrder(OrderType.MOVE);
		unitLocations[cell.X,cell.Y].selectUnit();
		unitSelected=true;
		lastUnitPos=cell+GetUsedRect().Position;
		
		validMoveDisplay.displayValidMoves(selectedUnit.getMovementRange(),cell+GetUsedRect().Position, selectedUnit.getMovementType());
		return 0;
	}
	
	public void deselectUnit(){
		unitSelected=false;
		unitMoving=false;
		if(selectedUnit!=null){
			selectedUnit.deselectUnit();
			selectedUnit=null;
		}
	}
	
	public void SetSelectedUnitMoved(bool boolIn){
		selectedUnit.setMovedStatus(boolIn);
		
		if(boolIn == true){
			deselectUnit();
		}
	}
	
	public void setCurrentOrder(OrderType orderInput){
		currentOrder = orderInput;
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
