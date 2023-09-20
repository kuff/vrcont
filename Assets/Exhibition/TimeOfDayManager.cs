using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    public Material daySky;
    public Material sunsetSky;
    public Material[] nightSkyes = new Material[3];
    public Light lig;

    public void UpdateTimeOfDay(TimeOfDay timeOfDay,Location location)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Day:
                lig.intensity = 2;
                lig.colorTemperature = 6000;
                RenderSettings.skybox = daySky;
                break;
            // case TimeOfDay.Sunset:
            //     lig.intensity = 0.5f;
            //     lig.colorTemperature = 2500;
            //     RenderSettings.skybox = sunsetSky;
            //     break;
            case TimeOfDay.Night:
                lig.intensity = 0;
                lig.colorTemperature = 20000;
                RenderSettings.skybox = nightSkyes[(int)location];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null);
        }
    }

}
