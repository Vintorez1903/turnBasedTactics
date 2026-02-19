using Godot;
using System;
using static sharedTypes;
using static JSONParsing;

public partial class Unit : Node2D
{
	[Export] public string statSheet; 
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
	
	//unit initialization logic
	public void initUnit(int teamIDInput){
		UnitStats unitStats = JSONParsing.unitStatsParse(statSheet);
	
		defaultMovementRange = unitStats.MovementRange;
		movementRange = unitStats.MovementRange;
		movementType = (MovementType)Enum.Parse(typeof(MovementType),unitStats.MovementType);
		initialHealth = unitStats.Health;
		health = unitStats.Health;
		teamID = teamIDInput;
	}
	
	//getters/setters
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
	
	//unit selection logic
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
	
	//combat logic
	public void hurt(int dmg){
		health-=dmg;
	}
	
	public void attack(WeaponStats wepStats,Unit target){
		if(target.getTeamID()!=getTeamID()){
			target.hurt(wepStats.Damage);
		}
	}
}
