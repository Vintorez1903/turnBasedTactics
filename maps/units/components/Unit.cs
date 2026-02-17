using Godot;
using System;
using static sharedTypes;

public partial class Unit : Node2D
{
	private int defaultMovementRange = 6;
	private int movementRange = 6;
	private const int defaultFrame = 1;
	private MovementType movementType;
	private bool selected = false;
	private bool hasMoved = false;
	private int initialHealth; 
	private int health; 
	private int teamID;
	AnimatedSprite2D animPlayer;
	
	public void initUnit(
		int movementInput,
		string movementTypeInput,
		int healthInput,
		int teamIDInput
	){
		defaultMovementRange = movementInput;
		movementRange = movementInput;
		movementType = (MovementType)Enum.Parse(typeof(MovementType),movementTypeInput);
		initialHealth = healthInput;
		health = healthInput;
		teamID = teamIDInput;
	}
	
	public int getTeamID(){
		return teamID;
	}
	
	public MovementType getMovementType(){
		return movementType;
	}
	
	public bool getSelectedStatus(){
		return selected;
	}
	
	public int getMovementRange(){
		return movementRange;
	}
	
	public void setMovedStatus(bool inputBool){
		hasMoved=inputBool;
		
		if(!hasMoved){
			animPlayer.Modulate = new Color(1f,1f,1f);
		}
	}
	
	public bool getMovedStatus(){
		return hasMoved;
	}
	
	public void selectUnit(){
		selected=true;
		SetProcess(true);
		lastPos=Position;
	}
	
	public void deselectUnit(){
		selected=false;
		
		if(getMovedStatus()==true){
			animPlayer.Modulate = new Color(.5f,.5f,.5f);
		}
		
		SetProcess(false);
		animPlayer.Frame=defaultFrame;
	}
	
	public override void _Ready(){
		animPlayer = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		deselectUnit();
	}
	
	
	public override void _Process(double delta){
		updateAnimation();
		storeLastPos();
	}
	
	Vector2 lastPos;
	private void storeLastPos(){
		lastPos=Position;
	}
	
	private void updateAnimation(){
		Vector2 heading = Position-lastPos;
		float animFrame = heading.Angle();
		//change angle to always be positive;
		animFrame+=(Mathf.Pi/2);
		//convert radians into frame number
		int frameNumber = (int)(animFrame/(Mathf.Pi/(2)));
		animPlayer.Frame=frameNumber;
	}
}
