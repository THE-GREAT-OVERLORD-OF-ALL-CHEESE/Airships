using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AirshipEngineManager : MonoBehaviour
{
    public AirshipEngine[] engines;

    public void UpdateEngines(float throttle, float yaw) {
        foreach (AirshipEngine engine in engines)
        {
            engine.UpdateEngine(Mathf.Clamp(throttle,-1,1), Mathf.Clamp(yaw, -1, 1));
        }
    }
}
