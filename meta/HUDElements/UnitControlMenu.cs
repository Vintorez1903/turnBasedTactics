using Godot;
using System;
using static sharedTypes;

public partial class UnitControlMenu : CanvasLayer
{
	private MainMapLayer mainMap;
	
	public void initMenu(MainMapLayer mapIn){
		mainMap=mapIn;
	}
	
	private void OnMoveButtonPressed(){
		mainMap.setCurrentOrder(OrderType.MOVE);
		Visible=false;
	}
	
	public void OnAttackButtonPressed(){
		mainMap.setCurrentOrder(OrderType.ATTACK);
		Visible=false;
	}
}
