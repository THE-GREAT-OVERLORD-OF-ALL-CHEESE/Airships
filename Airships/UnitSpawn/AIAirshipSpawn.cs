using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIAirshipSpawn : AIUnitSpawn
{
	public enum AirshipBehaviours {
		Parked,
		Hover,
		Orbit,
		FlyPath
	}

	public AIAirshipPilot pilot;

	[UnitSpawn("Debug Buoyancy")]
	public bool debugBuoyancy;

	[UnitSpawn("Debug AI")]
	public bool debugAI;

	[UnitSpawn("Default Behavior")]
	public AirshipBehaviours defaultBehavior;

	[UnitSpawnAttributeRange("Initial Airpeed", 10f, 40f, UnitSpawnAttributeRange.RangeTypes.Float)]
	public float spawnSpeed = 40;

	[UnitSpawnAttributeRange("Target Nav Speed", 10f, 40f, UnitSpawnAttributeRange.RangeTypes.Float)]
	public float targetSpeed = 40;

	[UnitSpawn("Starting Waypoint")]
	public Waypoint waypoint;

	[UnitSpawn("Starting Path")]
	public FollowPath path;

	[UnitSpawnAttributeRange("Target Altitude", 0f, 3000f, UnitSpawnAttributeRange.RangeTypes.Float)]
	public float targetAltitude = 1000;

	[UnitSpawnAttributeRange("Fuel %", 0f, 100f, UnitSpawnAttributeRange.RangeTypes.Float)]
	public float startingFuel = 100f;

	[VTEvent("Land", "Command the aircraft to land at a waypoint.", new string[]{"Waypoint"})]
	public void Land(Waypoint waypoint)
	{
		pilot.LandWaypoint(waypoint);
	}

	[VTEvent("Hover", "Command the aircraft to hover at a waypoint.", new string[] {"Waypoint"})]
	public void Hover(Waypoint waypoint)
	{
		pilot.HoverWaypoint(waypoint);
	}

	[VTEvent("Orbit Waypoint", "Command the aircraft to orbit a waypoint.", new string[]
	{
	"Waypoint",
	"Radius",
	"Altitude"
	})]
	public void Orbit(Waypoint waypoint, [VTRangeParam(1000f, 10000f)] float radius, [VTRangeParam(0f, 3000f)] float alt)
	{
		pilot.OrbitWaypoint(waypoint, radius, alt);
	}

	[VTEvent("Set Path", "Set the aircraft to fly along a path.", new string[]
	{
	"Path"
	})]
	public void FlyPath(FollowPath path)
	{
		pilot.SetPath(path);
	}

	[VTEvent("Set Speed", "Set the maximum airspeed.", new string[]
	{
	"Airspeed"
	})]
	public void SetSpeed([VTRangeParam(10f, 40f)] float speed)
	{
		pilot.SetMaxSpeed(speed);
	}

	[VTEvent("Set Altitude", "Set the target altitude.", new string[]
	{
	"Altitude"
	})]
	public void SetAltitude([VTRangeParam(0f, 3000f)] float altitude)
	{
		pilot.SetAltitude(altitude);
	}

    public override void OnSpawnUnit()
    {
        base.OnSpawnUnit();
		pilot.behaviour = defaultBehavior;
		if (waypoint != null) {
			pilot.waypoint = waypoint.GetTransform();
		}
		pilot.path = path;

		pilot.maxSpeed = spawnSpeed;
		pilot.targetAltitude = targetAltitude;

		pilot.debug = debugAI;
		pilot.buoyancy.debug = debugBuoyancy;
	}
}
