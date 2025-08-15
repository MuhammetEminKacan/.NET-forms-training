MAIN FUNCTIONS

TimeCalculation (Entry Point)

Task<TimeCalculationResultDto> TimeCalculation(string departureAirportCode, string

destinationAirportCode, string aircraftIcao, DateTime flightDate)

Purpose: Primary orchestration for all flight-time calculations.

Process: Route calculation → aircraft specs → climb/descent → waypoint generation →

weather integration → final time computation.

Returns: Flight time with/without wind, route path, total distance.

GetRouteDataAsync (Route Planning)

Task<(double distance, int altitude, List<string> routePath)>

GetRouteDataAsync(string departureAirportCode, string destinationAirportCode)

Purpose: Determine optimal flight path using A\*; fallback to great circle.

Process: Airport coordinate lookup → graph-based pathfinding → altitude assignment by

distance.

Returns: Total route distance, cruise altitude, waypoint sequence.

CalculateAirwayDistance (Pathfinding Core)

Task<AStarResult> CalculateAirwayDistance(string startIata, string endIata)

Purpose: Implement A\* for optimal airway routing.

Process: Graph validation → node connectivity analysis → heuristic pathfinding → path

reconstruction.

Fallback: Auto-switch to great-circle when airway routing is unavailable.

FindWaypointsOnRouteAsync (Segmentation)

Task<List<WaypointDirection>> FindWaypointsOnRouteAsync(List<string> path, double

climbDistanceMi, double descentDistanceMi, string departureAirportCode, string

destinationAirportCode)

Purpose: Create cruise-phase waypoints and bearings for weather sampling.

Process: Route polyline → cruise distance calc → uniform segmentation → bearing

computation.

Output: Coordinates and track bearings per segment.

GetWeatherData (Meteorological Integration)

Task<(double windSpeed, double windDirection)> GetWeatherData(WaypointDirection

waypoint, DateTime flightTime, int cruiseAltitudeFeet)

Purpose: Retrieve wind at given coordinate, time, and altitude.

Process: Altitude→pressure-level mapping → Open-Meteo call → time handling → data

extraction.

Output: Wind speed (kt) and direction (deg) for ground-speed calc.

FindCruiseSpeedForThatWaypoint (Wind Integration)

double FindCruiseSpeedForThatWaypoint(AircraftSpecs specs, double windSpeed, double

windDirection, double bearing, double totalDistanceMi)

Purpose: Compute actual ground speed with wind effects.

Process: Get TAS → compute wind component → compute ground speed.

Formula: Ground Speed = TAS + tailwind component − headwind component.

MAIN ENTITIES

GraphData (Route Network)

public class GraphData { public Dictionary<string, Node> Graph { get; set; } public

Dictionary<string, List<string>> Incoming { get; set; } }

Purpose: In-memory global airway network.

Components: Node dictionary; incoming adjacency for reverse pathfinding.

Performance: Singleton with background warm-up for sub-millisecond access.

Node (Waypoint Representation)

public class Node { public string Ident { get; set; } public double Lat { get; set;

} public double Lon { get; set; } public List<Edge> Neighbors { get; set; } }

Purpose: Single waypoint in the airway network.

Properties: ID, coordinates, connected airways with costs.

Usage: Foundation for A\* pathfinding.

AircraftSpecs (Performance Profile)

public class AircraftSpecs { public int? Cruise\_TAS { get; set; } public int?

Initial\_ROC { get; set; } public int? Climb\_150\_ROC { get; set; } public int?

Descent\_240\_ROD { get; set; } /\* ... \*/ }

Purpose: Aircraft performance across phases.

Coverage: Initial/intermediate climb, cruise TAS, descent rates.

Validation: Ensure critical params exist before calc.

GeoPoint (Geographic Calculations)

public class GeoPoint { public double Latitude { get; set; } public double

Longitude { get; set; } public double CalculateGreatCircleDistance(GeoPoint other)

{ } }

Purpose: Coordinate type with distance calc.

Methods: Haversine great-circle distance.

Usage: Base for geographic computations.

WaypointDirection (Cruise Segments)

public class WaypointDirection { public double Latitude { get; set; } public double

Longitude { get; set; } public double Bearing { get; set; } }

Purpose: Cruise waypoint with heading.

Application: Weather sampling & wind component calc.

Generation: Uniformly along cruise portion.

TimeCalculationResultDto (Final Output)

public class TimeCalculationResultDto { public double TimeWithWind { get; set; }

public double TimeWithoutWind { get; set; } public List<string> ResultPath { get;

set; } public double TotalDistance { get; set; } }

Purpose: Complete planning result package.

Components: Comparative times, route info, distance.

Usage: Primary API response.
