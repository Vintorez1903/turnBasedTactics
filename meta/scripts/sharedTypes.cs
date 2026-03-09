using Godot;
using System;

public static class sharedTypes
{
	public enum MovementType{
		FEET,
		TRACKS,
		WHEELS,
		FLYING
	}
	
	public enum OrderType{
		NULL,
		MOVE,
		ATTACK,
		WAIT
	}
}
