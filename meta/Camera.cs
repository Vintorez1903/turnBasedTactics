using Godot;
using System;
using System.Collections.Generic;

public partial class Camera : Node2D
{
	private float camMoveSpeed = 15f;
	[Export] public MainMapLayer mainLayer;
	[Export] public TileMapLayer commandLayer;
	public UnitControlMenu unitControlMenu;
	
	public override void _Ready(){
		unitControlMenu = (UnitControlMenu)GetNode("UnitControlMenu");
		unitControlMenu.initMenu(mainLayer);
		mainLayer.recieveControlMenu(unitControlMenu);
	}
	
	public override void _Input(InputEvent @event){
		//select unit
		if(Input.IsActionJustPressed("select")){
			Vector2I cell = mainLayer.LocalToMap(GetViewport().GetCamera2D().GetGlobalMousePosition());
			int unitSelected = mainLayer.selectUnit(cell);
		}
	}
	
	//logic related to the unit control menu
	public void moveUnitControlMenu(Vector2I cell){
		unitControlMenu.Offset = mainLayer.MapToLocal(cell);
		unitControlMenu.Visible = true;
	}
	
	public void hideUnitControlMenu(){
		unitControlMenu.Visible = false;
	}
	
	public void showUnitControlMenu(){
		unitControlMenu.Visible = true;
	}
	
	private void OnEndTurnButtonPressed(){
		mainLayer.endTurn();
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
