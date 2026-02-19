using Godot;
using System;
using System.Collections.Generic;
using static sharedTypes;

public partial class ValidMoveDisplay
{
	public TileMapLayer commandLayer;
	public MainMapLayer terrainLayer;
	private Queue<Vector2I> lastTiles = new Queue<Vector2I>();
	
	public ValidMoveDisplay(TileMapLayer commandin,MainMapLayer terrainin){
		commandLayer=commandin;
		terrainLayer=terrainin;
	}
	
	private struct queuedMove{
		public int movesLeft;
		public Vector2I location;
		
		public queuedMove(int mv, Vector2I loc){
			movesLeft=mv;
			location=loc;
		}
	}
	
	public void initDisplay(MainMapLayer terrainIn, TileMapLayer commandIn){
		terrainLayer=terrainIn;
		commandLayer=commandIn;
	}
	
	public void clearDisplayedMoves(){
		Rect2 usedRect = commandLayer.GetUsedRect();
		Vector2I startPos = (Vector2I)usedRect.Position;
		Vector2I endPos = (Vector2I)usedRect.Size + startPos;
		
		for(int i = startPos.X; i < endPos.X; i++){
			for(int j = startPos.Y; j < endPos.Y; j++){
					commandCellSetClear(new Vector2I(i,j));
			}
		}
	}
	
	private void commandCellSetMove(Vector2I cell){
		commandLayer.SetCell(cell, 0, new Vector2I(0, 0), 0);
	}
	
	private void commandCellSetAttack(Vector2I cell){
		commandLayer.SetCell(cell, 1, new Vector2I(0, 0), 0);
	}
	
	private void commandCellSetClear(Vector2I cell){
		commandLayer.SetCell(cell, -1, new Vector2I(0, 0), -1);
	}
	
	private void validMovesHelper(int movesLeft,Vector2I cell,Queue<queuedMove> activeTiles, MovementType moveType){
		//check if tile has been marked already
		TileData cellData = terrainLayer.GetCellTileData(cell);
		//check if tile is in bounds and passable
		if(terrainLayer.GetCellSourceId(cell)>-1 && !terrainLayer.isOccupied(cell)){
			string moveTypeString = "COST_"+moveType.ToString();
			int movesAfterCost = movesLeft-(int)cellData.GetCustomData(moveTypeString);
			if(movesAfterCost>0){
				queuedMove returnStruct =  new queuedMove(movesAfterCost,cell);
				activeTiles.Enqueue(returnStruct);
				lastTiles.Enqueue(returnStruct.location);
				commandCellSetMove(returnStruct.location);
			}
		}
	}
	
	public void displayValidMoves(int moves,Vector2I cell, MovementType moveType){
		clearDisplayedMoves();
		Queue<queuedMove> queue = new Queue<queuedMove>();
		queuedMove initialMove = new queuedMove(moves+1,cell);
		
		queue.Enqueue(initialMove);
		
		while(queue.Count>0){
			queuedMove activeCell = queue.Dequeue();
			Vector2I cellLeft = cell;
			validMovesHelper(activeCell.movesLeft,activeCell.location+Vector2I.Left,queue,moveType);
			validMovesHelper(activeCell.movesLeft,activeCell.location+Vector2I.Right,queue,moveType);
			validMovesHelper(activeCell.movesLeft,activeCell.location+Vector2I.Up,queue,moveType);
			validMovesHelper(activeCell.movesLeft,activeCell.location+Vector2I.Down,queue,moveType);
		}
		
		commandCellSetClear(initialMove.location);
	}
}
