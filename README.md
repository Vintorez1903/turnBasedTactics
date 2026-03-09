This is the beginnings of my implementation of an Advance Wars/ GBA Fire Emblem style battle system. I had initially intended to keep it private.
My reasons for making it were both to learn more about making this style of game in godot, as well as to make an actual game.
However, my heart lies with the Real Time Strategy genre, while I do love turn based games as well they are not truly what I want to make.
So while this was a very good learning experience, at this point most of what I will be implementing will not help me make a real time strategy game, so I've decided to move on.
If I choose to revisit this at a later point I will probably clone this repo and take it private, but this one is free for anyone to use if you feel you can get some use out of it. Should be avaiable under the MIT open source license.

below is a small list of features that are currently in this prototype:
	
	-units can be selected and moved around the map using a custom A* pathfinding implementation
	
	-movement ranges are displayed as an overlay on top of the main map
	
	-movement orders can be canceled or completed via a popup menu
	
	-support for four different movement types (walking, tracks, wheels, flight)
	
	-a variety of terrains that affect movement types differently
	
	-support for two players in local co-op
	
	-JSON objects that store weapon and unit stats and are parsed at runtime for easy modification

At the time of writing I was preparing to add attacking to the game, if I ever come back to this it will probably be the first non-refactoring change to be made.

Controls: 
	
	-WASD to move camera
	
	-RMB to select a unit
	
	-LMB to command a unit
	
	-upon selecting where a unit is to go a contextual menu pops up with additional options.

Below is a summary of some of the game's main classes.
----- Major Classes (broad strokes) -----

- MainMapLayer.cs -
Not Static
Purpose:

  -Be the main scene around which most others revolve

  -store terrain tiles and data pertaining to them

  -Initialize units and store unit positions

  -Move units around the board at the behest of the players

  -Store data about and manage player turns

- AStarPathfinding.cs -
Not Static
Purpose:

  -provide a grid-based A* pathfinding implimentation that accounts for terrain cost

Contains:
	
	-AStarPathfinder constructor that accepts a tilemap node
	
	-the pathfind() method that finds the shortest path given a start and end position as well as a movement type
		-returns a tuple containing the true cost of a path as well as an array containing the nodes in the path

- JSONParsing.cs -

Static

Purpose:
	 
	 -Fetch JSON objects from specified path, turn them into corresponding objects
Contains:
	
	-Objects for unit/weapon stats
	
	-methods to parse JSON into these classes

- unit.cs -

Not Static

Purpose:
	
	-contain logic for unit animations,
  	
	-store unit stats in the form of objects that have been provided to it by JSONParsing.cs
  	
	-store unit-specific state like whether it has moved and what player it belongs to
