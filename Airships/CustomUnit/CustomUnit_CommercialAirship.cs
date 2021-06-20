using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomUnit_CommercialAirship : CustomUnitBase
{
    public CustomUnit_CommercialAirship(Teams team, string category, string id, string name, string description, UnitSpawn.PlacementModes placementMode, bool alignToSurface) : base(team, category, id, name, description, placementMode, alignToSurface)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = unitID;

        root.SetActive(false);

        Debug.Log("Adding airship spawn");
        AIAirshipSpawn spawn = root.AddComponent<AIAirshipSpawn>();
        spawn.category = category;
        spawn.unitName = unitName;
        spawn.unitDescription = description;
        spawn.placementMode = placementMode;

        spawn.alignToGround = alignToSurface;
        spawn.createFloatingOriginTransform = true;
        spawn.heightFromSurface = 8f;

        Debug.Log("Adding airship model");
        GameObject airship = GameObject.Instantiate(Airships.instance.airshipPrefab);
        airship.transform.parent = root.transform;
        airship.transform.localPosition = Vector3.zero;

        Debug.Log("Adding rigidbody");
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.mass = 5;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Debug.Log("Adding health");
        Health health = root.AddComponent<Health>();
        health.startHealth = 600;
        health.maxHealth = 600;
        health.minDamage = 0;
        health.invincible = false;
        health.OnDeath = new UnityEngine.Events.UnityEvent();

        spawn.health = health;

        Debug.Log("Adding actor");
        Actor actor = root.AddComponent<Actor>();
        actor.team = team;
        actor.role = Actor.Roles.Ground;
        actor.iconType = actor.iconType = team == Teams.Allied ? UnitIconManager.MapIconTypes.FriendlyAir : UnitIconManager.MapIconTypes.EnemyAir;

        Debug.Log("Adding hitboxes");
        Hitbox hitbox = airship.transform.Find("Colliders").Find("Blimp").gameObject.AddComponent<Hitbox>();
        hitbox.health = health;
        Hitbox hitbox2 = airship.transform.Find("Colliders").Find("Gondola").gameObject.AddComponent<Hitbox>();
        hitbox2.health = health;

        Debug.Log("Adding fire");
        VehicleFireDeath fire = root.AddComponent<VehicleFireDeath>();
        fire.firePrefab = CustomScenarioAssets.instance.largeFirePrefab;
        fire.explosionType = ExplosionManager.ExplosionTypes.MediumAerial;

        Debug.Log("Adding flight info");
        FlightInfo flightInfo = root.AddComponent<FlightInfo>();
        flightInfo.suspensions = new RaySpringDamper[0];

        Debug.Log("Adding mass updater");
        MassUpdater massUpdater = root.AddComponent<MassUpdater>();
        massUpdater.baseMass = 5;
        massUpdater.updateInterval = -1;

        Debug.Log("Adding buoyancy");
        AirshipBuoyancy buoyancy = root.AddComponent<AirshipBuoyancy>();
        buoyancy.liftingPoint = airship.transform.Find("Colliders").Find("Blimp");
        buoyancy.rb = rb;
        buoyancy.displacement = 8225f;
        buoyancy.actor = actor;

        FuelTank liftingGas = airship.transform.Find("Colliders").Find("Blimp").gameObject.AddComponent<FuelTank>();
        liftingGas.fuelDensity = 0.00017025423f;
        liftingGas.maxFuel = 8225f;
        liftingGas.startingFuel = 8225f;
        buoyancy.liftingGas = liftingGas;

        FuelTank ballonet = airship.transform.Find("Ballonet").gameObject.AddComponent<FuelTank>();
        ballonet.fuelDensity = 0.001225f;
        ballonet.maxFuel = 3000;
        ballonet.startingFuel = 3000;
        buoyancy.ballonet = ballonet;

        Debug.Log("Adding aero");
        SimpleDrag drag = airship.transform.Find("Colliders").Find("Blimp").gameObject.AddComponent<SimpleDrag>();
        drag.area = 153.94f;

        OmniWing omni = airship.transform.Find("Colliders").Find("Blimp").gameObject.AddComponent<OmniWing>();
        omni.liftArea = 75 * 14;
        omni.liftCoefficient = 0.39f;
        omni.dragCoefficient = 0.05f;

        Wing rudder = airship.transform.Find("small_blimp").Find("Vertical fin holder").Find("WingTransform").gameObject.AddComponent<Wing>();
        rudder.liftArea = 36f;
        rudder.liftCoefficient = 0.35f;
        rudder.dragCoefficient = 0.14f;

        Wing elavator1 = airship.transform.Find("small_blimp").Find("Left fin holder").Find("WingTransform").gameObject.AddComponent<Wing>();
        elavator1.liftArea = 36f;
        elavator1.liftCoefficient = 0.35f;
        elavator1.dragCoefficient = 0.14f;

        Wing elavator2 = airship.transform.Find("small_blimp").Find("Right fin holder").Find("WingTransform").gameObject.AddComponent<Wing>();
        elavator2.liftArea = 36f;
        elavator2.liftCoefficient = 0.35f;
        elavator2.dragCoefficient = 0.14f;

        AeroController aeroController = root.AddComponent<AeroController>();
        aeroController.controlSurfaces = new AeroController.ControlSurfaceTransform[3];

        Wing rudderFlap = airship.transform.Find("small_blimp").Find("Vertical fin holder").Find("fin_controlsurface").Find("WingTransform").gameObject.AddComponent<Wing>();
        rudderFlap.liftArea = 6f;
        rudderFlap.liftCoefficient = 0.35f;
        rudderFlap.dragCoefficient = 0.14f;
        aeroController.controlSurfaces[0] = new AeroController.ControlSurfaceTransform();
        aeroController.controlSurfaces[0].transform = airship.transform.Find("small_blimp").Find("Vertical fin holder").Find("fin_controlsurface").transform;
        aeroController.controlSurfaces[0].axis = Vector3.down;
        aeroController.controlSurfaces[0].maxDeflection = 30;
        aeroController.controlSurfaces[0].actuatorSpeed = 90;
        aeroController.controlSurfaces[0].yawFactor = 1;
        aeroController.controlSurfaces[0].rollFactor = -1;

        Wing elavator1Flap = airship.transform.Find("small_blimp").Find("Left fin holder").Find("fin_controlsurface").Find("WingTransform").gameObject.AddComponent<Wing>();
        elavator1Flap.liftArea = 6f;
        elavator1Flap.liftCoefficient = 0.35f;
        elavator1Flap.dragCoefficient = 0.14f;
        aeroController.controlSurfaces[1] = new AeroController.ControlSurfaceTransform();
        aeroController.controlSurfaces[1].transform = airship.transform.Find("small_blimp").Find("Left fin holder").Find("fin_controlsurface").transform;
        aeroController.controlSurfaces[1].axis = Vector3.up;
        aeroController.controlSurfaces[1].maxDeflection = 30;
        aeroController.controlSurfaces[1].actuatorSpeed = 90;
        aeroController.controlSurfaces[1].pitchFactor = 1;
        aeroController.controlSurfaces[1].rollFactor = 1;

        Wing elavator2Flap = airship.transform.Find("small_blimp").Find("Right fin holder").Find("fin_controlsurface").Find("WingTransform").gameObject.AddComponent<Wing>();
        elavator2Flap.liftArea = 6f;
        elavator2Flap.liftCoefficient = 0.35f;
        elavator2Flap.dragCoefficient = 0.14f;
        aeroController.controlSurfaces[2] = new AeroController.ControlSurfaceTransform();
        aeroController.controlSurfaces[2].transform = airship.transform.Find("small_blimp").Find("Right fin holder").Find("fin_controlsurface").transform;
        aeroController.controlSurfaces[2].axis = Vector3.down;
        aeroController.controlSurfaces[2].maxDeflection = 30;
        aeroController.controlSurfaces[2].actuatorSpeed = 90;
        aeroController.controlSurfaces[2].pitchFactor = 1;
        aeroController.controlSurfaces[2].rollFactor = -1;

        Debug.Log("Adding Engines");
        FuelTank fuel = root.AddComponent<FuelTank>();
        fuel.maxFuel = 100;
        fuel.startingFuel = 100;
        fuel.fuelDensity = 0.00071f;

        buoyancy.fuelTank = fuel;

        AirshipEngineManager engineManager = root.AddComponent<AirshipEngineManager>();
        engineManager.engines = new AirshipEngine[2];

        ModuleEngine lEngine = airship.transform.Find("small_blimp").Find("Engines").Find("Left Engine").gameObject.AddComponent<ModuleEngine>();
        lEngine.thrustTransform = airship.transform.Find("small_blimp").Find("Engines").Find("Left Engine").Find("ThrustTF");
        lEngine.rb = rb;
        lEngine.maxThrust = -10;
        lEngine.fuelDrain = 0.01f;
        lEngine.spoolRate = 1.0f;
        lEngine.lerpSpool = true;
        lEngine.idleThrottle = 0.0f;
        lEngine.thrustHeatMult = 15f;
        lEngine.fuelTank = fuel;
        engineManager.engines[0] = lEngine.gameObject.AddComponent<AirshipEngine>();
        engineManager.engines[0].engine = lEngine;
        engineManager.engines[0].throttleResponse = 1;
        engineManager.engines[0].yawResponse = 1;

        HeatEmitter lHeat = airship.transform.Find("small_blimp").Find("Engines").Find("Left Engine").gameObject.AddComponent<HeatEmitter>();
        lHeat.cooldownRate = 100;

        EngineRotator lRot = airship.transform.Find("small_blimp").Find("Engines").Find("Left Engine").Find("Prop Holder").gameObject.AddComponent<EngineRotator>();
        lRot.engine = lEngine;
        lRot.axis = Vector3.forward;
        lRot.lerpRate = 1;
        lRot.minSpeed = 90;
        lRot.maxSpeed = 2000;

        ModuleEngine rEngine = airship.transform.Find("small_blimp").Find("Engines").Find("Right Engine").gameObject.AddComponent<ModuleEngine>();
        rEngine.thrustTransform = airship.transform.Find("small_blimp").Find("Engines").Find("Right Engine").Find("ThrustTF");
        rEngine.rb = rb;
        rEngine.maxThrust = -10;
        rEngine.fuelDrain = 0.01f;
        rEngine.spoolRate = 1.0f;
        rEngine.lerpSpool = true;
        rEngine.idleThrottle = 0.0f;
        rEngine.thrustHeatMult = 15f;
        rEngine.fuelTank = fuel;
        engineManager.engines[1] = rEngine.gameObject.AddComponent<AirshipEngine>();
        engineManager.engines[1].engine = rEngine;
        engineManager.engines[1].throttleResponse = 1;
        engineManager.engines[1].yawResponse = -1;

        HeatEmitter rHeat = airship.transform.Find("small_blimp").Find("Engines").Find("Right Engine").gameObject.AddComponent<HeatEmitter>();
        rHeat.cooldownRate = 100;

        EngineRotator rRot = airship.transform.Find("small_blimp").Find("Engines").Find("Right Engine").Find("Prop Holder").gameObject.AddComponent<EngineRotator>();
        rRot.engine = rEngine;
        rRot.axis = Vector3.forward;
        rRot.lerpRate = 1;
        rRot.minSpeed = 90;
        rRot.maxSpeed = 2000;

        Debug.Log("Adding LED Sign");
        AirshipLEDBillboard billboard = airship.transform.Find("small_blimp").Find("Blimp").gameObject.AddComponent<AirshipLEDBillboard>();
        billboard.renderer = airship.transform.Find("small_blimp").Find("Blimp").gameObject.GetComponent<Renderer>();
        billboard.scrollingMaterialID = 1;
        billboard.screenAspectRatio = 3;
        billboard.scrollSpeed = 0.05f;

        Debug.Log("Adding Pilot");
        AIAirshipPilot pilot = root.AddComponent<AIAirshipPilot>();
        spawn.pilot = pilot;
        AutoPilot ap = root.AddComponent<AutoPilot>();
        ap.rb = rb;
        ap.outputs = new FlightControlComponent[1];
        ap.outputs[0] = aeroController;
        ap.pitchPID = new PID(0.15f, 1, 0, -0.2f, 0.2f);
        ap.yawPID = new PID(0.15f, 0, 0, 0, 0);
        ap.rollPID = new PID(0, 0, -0.01f, 0, 0);

        ap.rollUpBias = 1;
        ap.maxBank = 0;
        ap.maxClimb = 15;
        ap.maxDescend = 15;

        ap.engines = new List<ModuleEngine>();
        ap.throttlePID = new PID(0, 0, 0, 0, 0);
        ap.controlThrottle = false;

        ap.updateMode = UpdateModes.Fixed;
        ap.steerMode = AutoPilot.SteerModes.Stable;

        Debug.Log("Adding Preset Cameras");
        Transform pspTranform = airship.transform.Find("PresetCameras");
        for (int i = 0; i < pspTranform.childCount; i++) {
            SpectatorPresetPosition psp = pspTranform.GetChild(0).gameObject.AddComponent<SpectatorPresetPosition>();
        }

        root.SetActive(true);

        return root;
    }
}
