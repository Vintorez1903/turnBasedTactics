using Godot;
using System;
using System.Collections.Generic;

public partial class Camera : Node2D
{
	private float camMoveSpeed = 15f;
	[Export]
	public MainMapLayer terrainLayer;
	[Export]
	public TileMapLayer commandLayer;
	
	public override void _Input(InputEvent @event){
		if(Input.IsActionJustPressed("select")){
			Vector2I cell = terrainLayer.LocalToMap(GetViewport().GetCamera2D().GetGlobalMousePosition());
			terrainLayer.selectUnit(cell);
		}
	}
	
	private void OnEndTurnButtonPressed(){
		terrainLayer.endTurn();
	}
	
	public override void _PhysicsProcess(double delta){
		float floatDelta = (float)delta;
		moveCam(floatDelta);
	}
	
	private void moveCam(float delta){
		Vector2 inputVector = Input.GetVector("moveLeft","moveRight","moveUp","moveDown");
		Position+=inputVector*camMoveSpeed;
	}
}
