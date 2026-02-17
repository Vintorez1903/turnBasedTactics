using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class UnitStats{
		public int MovementRange { get; set; }
		public string MovementType { get; set; }
		public int Health { get; set; }
}

public static class JSONParsing
{
	
	public static UnitStats unitStatsParse(string inString){
		string json = File.ReadAllText(inString);
		return JsonSerializer.Deserialize<UnitStats>(json);
	}
}
