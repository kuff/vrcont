using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    
    public List<GameObject> Locations;
    public Location currentLocation;

    private void Start()
    {
        Invoke(nameof(ResetLocations),0);
    }

    private void ResetLocations()
    {
        foreach (GameObject location in Locations)
        {
            location.SetActive(false);
        }

        Locations[0].SetActive(true);
    }

    public void ChangeLocation(Location locationToSwitchTo)
    {
        
        Locations[(int)currentLocation].SetActive(false);
        currentLocation = locationToSwitchTo;
        Locations[(int)currentLocation].SetActive(true);
    }
}
