using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AirshipBuoyancy : MonoBehaviour
{
    public Actor actor;
    public Transform liftingPoint;
    public Rigidbody rb;
    public FlightInfo flightInfo;

    public FuelTank liftingGas;
    public FuelTank ballonet;

    public FuelTank fuelTank;

    public float displacement;

    public float airDensity = 0.001225f;
    public float liftingGasDensity = 0.00017025423f;

    public float pumpSpeed = 5;
    public float pumpInput;

    public float targetBuoyancy;
    public float targetVerticalSpeed = 5;
    public float targetAltitude = 1000;

    public float leakingRate = 100;

    public PID verticalSpeedPID;

    float currentBuoyancy;

    public bool debug = false;

    private void Start() {
        flightInfo = GetComponent<FlightInfo>();
        verticalSpeedPID = new PID(0.06f, 0, 0.06f, 0, 0);
        verticalSpeedPID.updateMode = UpdateModes.Fixed;

        rb.centerOfMass = ballonet.transform.localPosition;
    }

    private void FixedUpdate() {
        UpdateAP();
        UpdateBallonet();
        CalculateBouancy();
    }

    private void UpdateAP()
    {
        targetVerticalSpeed = Mathf.Clamp((targetAltitude - flightInfo.altitudeASL) / 50, -10, 10);

        targetBuoyancy = 1 + Mathf.Clamp(verticalSpeedPID.Evaluate(flightInfo.verticalSpeed, targetVerticalSpeed) * 0.1f, -0.1f, 0.1f);

        float normalisedBuoyancy = currentBuoyancy / rb.mass;

        pumpInput = (targetBuoyancy - normalisedBuoyancy) * 500;
        pumpInput *= -1;
    }

    private void UpdateBallonet()
    {
        pumpInput = Mathf.Clamp(pumpInput, -1, 1);

        if (pumpInput > 0)
        {
            ballonet.AddFuel(pumpInput * pumpSpeed * Time.fixedDeltaTime);
        }
        else
        {
            ballonet.RequestFuel(-pumpInput * pumpSpeed * Time.fixedDeltaTime);
        }
    }

    private void CalculateBouancy() {
        if (actor.alive == false)
        {
            liftingGas.RequestFuel(leakingRate * Time.fixedDeltaTime);
        }

        displacement = liftingGas.fuel;
        airDensity = AerodynamicsController.fetch.AtmosDensityAtPositionMetric(liftingPoint.position)/1000;

        currentBuoyancy = airDensity * displacement;
        rb.AddForceAtPosition(currentBuoyancy * -Physics.gravity, liftingPoint.position);
    }

    private void OnGUI() {
        if (debug)
        {
            string temp = "";

            temp += "Airship Debug\n";

            temp += "\n";

            temp += "Displacement: " + Mathf.Round(displacement) + "\n";
            temp += "Lifting Gas Mass: " + Mathf.Round(liftingGas.GetMass() * 1000) + "\n";
            temp += "Ballonet Mass: " + Mathf.Round(ballonet.GetMass() * 1000) + "\n";
            temp += "Total Mass: " + Mathf.Round(rb.mass * 1000) + "\n";
            temp += "Buoyancy: " + Mathf.Round(currentBuoyancy * 1000) + "\n";
            temp += "Weight: " + Mathf.Round((rb.mass - currentBuoyancy) * 1000) + "\n";

            temp += "\n";

            temp += "Bouancy (Normalised): " + Mathf.Round((currentBuoyancy / rb.mass) * 100) / 100 + "\n";
            temp += "Vertical Speed: " + Mathf.Round(flightInfo.verticalSpeed * 100) / 100 + "\n";
            temp += "Altitude: " + Mathf.Round(flightInfo.altitudeASL) + "\n";

            temp += "\n";

            temp += "Airspeed: " + Mathf.Round(flightInfo.airspeed) + "\n";
            temp += "AoA: " + Mathf.Round(flightInfo.aoa) + "\n";
            temp += "Pitch: " + Mathf.Round(flightInfo.pitch) + "\n";
            temp += "Yaw: " + Mathf.Round(flightInfo.heading) + "\n";
            temp += "Roll: " + Mathf.Round(flightInfo.roll) + "\n";
            temp += "Fuel: " + Mathf.Round((fuelTank.fuel/fuelTank.maxFuel) * 100) + "%\n";

            temp += "\n";

            temp += "AutoPilot\n";

            temp += "\n";

            temp += "Target Altitude: " + Mathf.Round(targetAltitude) + "\n";
            temp += "Target Vertical Speed: " + Mathf.Round(targetVerticalSpeed * 100) / 100 + "\n";
            temp += "Target Buoyancy: " + Mathf.Round(targetBuoyancy * 100) / 100 + "\n";
            temp += "Pump Input: " + Mathf.Round(pumpInput * 100) / 100 + "\n";

            GUI.TextArea(new Rect(100, 100, 400, 800), temp);

            /*float p = GUI.HorizontalSlider(new Rect(525, 25, 200, 30), verticalSpeedPID.kp, -1f, 1f);
            GUI.TextField(new Rect(800, 25, 200, 30), "p: " + (Mathf.Round(p * 100f) / 100f).ToString());
            float i = GUI.HorizontalSlider(new Rect(525, 50, 200, 30), verticalSpeedPID.ki, -1f, 1f);
            GUI.TextField(new Rect(800, 50, 200, 30), "i: " + (Mathf.Round(i * 100f) / 100f).ToString());
            float d = GUI.HorizontalSlider(new Rect(525, 75, 200, 30), verticalSpeedPID.kd, -1f, 1f);
            GUI.TextField(new Rect(800, 75, 200, 30), "d: " + (Mathf.Round(d * 100f) / 100f).ToString());

            GUI.TextField(new Rect(800, 100, 200, 30), "i: " + (Mathf.Round(verticalSpeedPID.Debug_GetPID().y * 100f) / 100f).ToString());

            verticalSpeedPID.kp = p;
            verticalSpeedPID.ki = i;
            verticalSpeedPID.kd = d;

            targetAltitude = GUI.HorizontalSlider(new Rect(525, 125, 200, 30), targetAltitude, 0f, 3000f);*/
        }
    }
}
