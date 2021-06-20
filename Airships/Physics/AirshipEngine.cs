using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AirshipEngine : MonoBehaviour
{
    public ModuleEngine engine;

    public float throttleResponse;
    public float yawResponse;

    public void UpdateEngine(float throttle, float yaw) {
        engine.SetThrottle(throttle * throttleResponse + yaw * yawResponse);
    }
}
