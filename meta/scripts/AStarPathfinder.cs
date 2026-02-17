using Godot;
using System;
using System.Collections.Generic;
using static sharedTypes;

public class AStarPathfinder
{
	MainMapLayer mapLayer;
	int mapWidth;
	int mapHeight;
	Vector2I mapPos;
	MovementType currentMovementType;
	
	public AStarPathfinder(MainMapLayer mapIn){
		mapLayer=mapIn;
		Rect2I map = mapLayer.GetUsedRect();
		mapWidth=map.Size.X;
		mapHeight=map.Size.Y;
		mapPos=map.Position;
	}
	
	private int getHScore(Vector2I currentPos, Vector2I endPos){
		int horizontalDistance = Mathf.Abs(currentPos.X-endPos.X);
		int verticalDistance = Mathf.Abs(currentPos.Y-endPos.Y);
		return horizontalDistance+verticalDistance;
	}
	
	private struct pathNode{
		public Vector2I position;
		public int cost;
		
		public pathNode(int costIn,Vector2I positionIn){
			cost=costIn;
			position=positionIn;
		}
	}
	
	private void pathfindHandleNode(Vector2I startPos,Vector2I previousPos, Vector2I currentPos, Vector2I endPos, pathNode activeNode, bool[,] closedList, PriorityQueue<pathNode,int> openList, Vector2I[,] neighborList){
		Vector2I arrayPosition = currentPos-mapPos;
		if(arrayPosition.X < 0 || arrayPosition.Y < 0){
			return;
		}
		if(arrayPosition.X >= mapWidth || arrayPosition.Y >= mapHeight){
			return;
		}
		
		if(closedList[arrayPosition.X,arrayPosition.Y]==false && (!mapLayer.isOccupied(currentPos) || currentPos==startPos)){
			TileData cellData = mapLayer.GetCellTileData(currentPos);
			string costType = "COST_"+currentMovementType.ToString();
			pathNode returnNode = new pathNode(activeNode.cost+(int)cellData.GetCustomData(costType),currentPos);
			
			openList.Enqueue(returnNode,returnNode.cost+getHScore(startPos,endPos));
			
			closedList[arrayPosition.X,arrayPosition.Y]=true;
			neighborList[arrayPosition.X,arrayPosition.Y]=previousPos;
		}
	}
	
	public (int,Vector2I[]) pathfind(Vector2I startPos, Vector2I endPos, MovementType moveType){
		currentMovementType = moveType;
		bool[,] closedList = new bool[mapWidth,mapHeight];
		Vector2I[,] neighborList = new Vector2I[mapWidth,mapHeight];
		
		PriorityQueue<pathNode,int> openList = new PriorityQueue<pathNode,int>();
		pathNode startNode = new pathNode(0,endPos);
		Vector2I endArrayPos = endPos-mapPos;
		closedList[endArrayPos.X,endArrayPos.Y]=true;
		openList.Enqueue(startNode,0);
		
		//pathfind from endpos to startpos
		while(openList.Count>0 && openList.Peek().position != startPos){
			pathNode activeNode = openList.Dequeue();
			pathfindHandleNode(startPos,activeNode.position,activeNode.position+Vector2I.Left,endPos,activeNode,closedList,openList,neighborList);
			pathfindHandleNode(startPos,activeNode.position,activeNode.position+Vector2I.Right,endPos,activeNode,closedList,openList,neighborList);
			pathfindHandleNode(startPos,activeNode.position,activeNode.position+Vector2I.Up,endPos,activeNode,closedList,openList,neighborList);
			pathfindHandleNode(startPos,activeNode.position,activeNode.position+Vector2I.Down,endPos,activeNode,closedList,openList,neighborList);
		}
		
		//create list of nodes to follow
		List<Vector2I> returnList = new List<Vector2I>();
		returnList.Add(startPos);
		int index=0;
		while(returnList[index] != endPos){
			Vector2I posInArray = returnList[index]-mapPos;
			returnList.Add(neighborList[posInArray.X,posInArray.Y]);
			index++;
		}
		TileData tData = mapLayer.GetCellTileData(startPos);
		string costType = "COST_"+currentMovementType.ToString();
		int returnCost = openList.Peek().cost-((int)tData.GetCustomData(costType));
		tData = mapLayer.GetCellTileData(endPos);
		returnCost += ((int)tData.GetCustomData(costType));
		return (returnCost,returnList.ToArray());
	}
}
