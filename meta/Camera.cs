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
	}
	
	public override void _Input(InputEvent @event){
		if(Input.IsActionJustPressed("select")){
			Vector2I cell = mainLayer.LocalToMap(GetViewport().GetCamera2D().GetGlobalMousePosition());
			int unitSelected = mainLayer.selectUnit(cell);
			if(unitSelected == 0){
				unitControlMenu.Offset = mainLayer.MapToLocal(cell);
				unitControlMenu.Visible = true;
			}
		}
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
