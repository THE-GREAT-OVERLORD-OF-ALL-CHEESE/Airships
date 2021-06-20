using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIAirshipPilot : MonoBehaviour
{
    public Actor actor;
    public Rigidbody rb;
    public FlightInfo flightInfo;
    public AirshipBuoyancy buoyancy;
    public AirshipEngineManager engines;
    public AeroController aeroController;
    public AutoPilot ap;

    public AIAirshipSpawn.AirshipBehaviours behaviour;

    public Transform waypoint;
    public FollowPath path;

    public float maxSpeed = 40;
    public float targetSpeed = 0;
    public float targetAltitude = 1000;
    public float orbitRadius = 3000;


    public bool debugDrive = false;
    public bool debug = false;

    private void Awake()
    {
        actor = GetComponent<Actor>();
        rb = GetComponent<Rigidbody>();
        flightInfo = GetComponent<FlightInfo>();
        buoyancy = GetComponent<AirshipBuoyancy>();
        engines = GetComponent<AirshipEngineManager>();
        aeroController = GetComponent<AeroController>();
        ap = GetComponent<AutoPilot>();
    }

    private void Start()
    {
        if (waypoint == null) {
            waypoint = new GameObject("TempWaypoint").transform;
            waypoint.position = transform.position;
            waypoint.gameObject.AddComponent<FloatingOriginTransform>();
        }
    }

    public void LandWaypoint(Waypoint wp)
    {
        waypoint = wp.GetTransform();
        behaviour = AIAirshipSpawn.AirshipBehaviours.Parked;
    }

    public void HoverWaypoint(Waypoint wp) {
        waypoint = wp.GetTransform();
        behaviour = AIAirshipSpawn.AirshipBehaviours.Hover;
    }

    public void OrbitWaypoint(Waypoint wp, float radius, float alt)
    {
        waypoint = wp.GetTransform();
        targetAltitude = alt;
        orbitRadius = radius;
        behaviour = AIAirshipSpawn.AirshipBehaviours.Orbit;
    }

    public void SetPath(FollowPath path)
    {
        this.path = path;
        behaviour = AIAirshipSpawn.AirshipBehaviours.FlyPath;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    public void SetAltitude(float altitude)
    {
        targetAltitude = altitude;
    }

    private void OnCollisionEnter() {
        if (actor.alive == false) {
            ExplosionManager.instance.CreateExplosionEffect(ExplosionManager.ExplosionTypes.Massive, transform.position, Vector3.up);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if (actor.alive == false && WaterPhysics.GetAltitude(transform.position) < 0)
        {
            ExplosionManager.instance.CreateExplosionEffect(ExplosionManager.ExplosionTypes.Massive, transform.position, Vector3.up);
            Destroy(gameObject);
        }

        if (debugDrive)
        {
            DebugDrive();
        }
        else
        {
            UpdateAI();
        }
    }

    private void DebugDrive() {
        aeroController.SetPitchYawRoll(new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal")));
    }

    private void UpdateAI()
    {
        Vector3 flatDir = transform.forward;

        switch (behaviour) {
            case AIAirshipSpawn.AirshipBehaviours.Hover:
                Vector3 targetDir = waypoint.position - transform.position;
                targetDir.y = 0;
                flatDir = targetDir;
                targetSpeed = Mathf.Clamp((targetDir.magnitude - 500)/100, -maxSpeed, maxSpeed);
                break;
            case AIAirshipSpawn.AirshipBehaviours.Orbit:
                Vector3 targetDir2 = waypoint.position - transform.position;
                targetDir2.y = 0;
                Vector3 parDir = Vector3.Cross(Vector3.up, targetDir2).normalized;

                float distance = targetDir2.magnitude - orbitRadius;

                flatDir = parDir * 1000 + targetDir2.normalized * distance;

                targetSpeed = maxSpeed;
                break;
            case AIAirshipSpawn.AirshipBehaviours.FlyPath:
                if (path != null)
                {
                    Vector3 pathTGT = path.GetFollowPoint(transform.position, 1000) - transform.position;
                    flatDir = pathTGT - transform.position;
                    targetAltitude = WaterPhysics.GetAltitude(pathTGT);
                    targetSpeed = maxSpeed;
                }
                else {
                    flatDir = waypoint.position - transform.position;
                    targetSpeed = 0;
                }
                break;
            case AIAirshipSpawn.AirshipBehaviours.Parked:
            default:
                Vector3 targetDir3 = waypoint.position - transform.position;
                targetDir3.y = 0;
                flatDir = targetDir3;
                targetSpeed = Mathf.Clamp((targetDir3.magnitude - 500) / 100, 0, maxSpeed);
                targetAltitude = flightInfo.altitudeASL - flightInfo.radarAltitude + 10;
                break;
        }

        buoyancy.targetAltitude = targetAltitude;

        if (flightInfo.radarAltitude < 0) {
            targetSpeed = 0;
        }
        
        flatDir.y = 0;
        flatDir = flatDir.normalized;

        float altDifference = targetAltitude - flightInfo.altitudeASL;

        Vector3 targetPosition = transform.position;
        targetPosition += flatDir + altDifference * Vector3.up / 1000;

        float rudderInput = Vector3.Dot(flatDir, transform.right) * 10;

        engines.UpdateEngines(targetSpeed - flightInfo.airspeed, rudderInput);
        if (ap != null) {
            ap.targetPosition = targetPosition;
            ap.SetOverrideRudder(rudderInput);

            if (Input.GetAxis("Horizontal") != 0) {
                ap.SetOverrideRudder(Input.GetAxis("Horizontal"));
            }
        }
    }

    private void OnGUI()
    {
        if (debug)
        {
            string temp = "";

            if (debugDrive)
            {
                temp += "MANUAL MODE";
            }
            else
            {
                temp += "AI MODE";
            }

            temp += "\n";

            temp += "Airship Pilot Debug\n";

            temp += "\n";

            temp += "Mode: " + behaviour.ToString() +"\n";

            temp += "Throttle: " + Mathf.Round(Input.GetAxis("Vertical") * 100) / 100 + "\n";
            temp += "Pitch: " + Mathf.Round(0 * 100) / 100 + "\n";
            temp += "Yaw: " + Mathf.Round(0 * 100) / 100 + "\n";
            temp += "Roll: " + Mathf.Round(0 * 100) / 100 + "\n";

            temp += "\n";

            temp += "AutoPilot\n";

            temp += "\n";

            temp += "Target Speed: " + Mathf.Round(targetSpeed) + "\n";
            temp += "Target Altitude: " + Mathf.Round(targetAltitude) + "\n";

            GUI.TextArea(new Rect(500, 100, 400, 800), temp);

            targetAltitude = GUI.HorizontalSlider(new Rect(925, 100, 200, 30), targetAltitude, 0f, 3000f);
        }
    }
}