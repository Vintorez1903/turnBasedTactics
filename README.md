----- Major Classes -----

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
