using Godot;
using System;
using static sharedTypes;

public partial class UnitControlMenu : CanvasLayer
{
	private MainMapLayer mainMap;
	
	public void initMenu(MainMapLayer mapIn){
		mainMap=mapIn;
	}
	
	public bool isOpen(){
		return Visible;
	}
	
	private void OnWaitButtonPressed(){
		Visible=false;
		mainMap.SetSelectedUnitMoved(true);
	}
	
	private void OnAttackButtonPressed(){
		Visible=false;
		mainMap.setCurrentOrder(OrderType.ATTACK);
	}
	
	private void OnCancelButtonPressed(){
		Visible=false;
		mainMap.setCurrentOrder(OrderType.MOVE);
		mainMap.cancelOrder();
	}
}
