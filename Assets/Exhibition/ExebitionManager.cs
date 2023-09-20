using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Runtime;
using Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
public enum Season { Summer,  Winter}
public enum TimeOfDay {Day,Night }
public enum Preset {Cooking,Garden,Sleeping}
public enum Location {City,Forest}
public class ExebitionManager : MonoBehaviour
{
    private int locationCount;
    private int seasonCount;
    private int lightingCount;
    private int presetCount;
    private SeasonManager seasonManager;
    private LocationManager locationManager;
    private Light sun;
    public Color dayColour ;
    public Color nightColour;
    private PresetAnimator animator;
    private TimeOfDayManager timeOfDayManager;

    public Location currentLocation;
    public Season currentSeason;
    public Preset currentPreset;
    public TimeOfDay currentTimeOfDay;
    
    public TextMeshPro[] pictureTexts;

    public TestConductor testConductor;

    public bool fuckItCheck;
    private void Start()
    {
        locationManager = FindObjectOfType<LocationManager>().GetComponent<LocationManager>();
        timeOfDayManager = FindObjectOfType<TimeOfDayManager>().GetComponent<TimeOfDayManager>();
        seasonManager = FindObjectOfType<SeasonManager>().GetComponent<SeasonManager>();
        animator = FindObjectOfType<PresetAnimator>().GetComponent<PresetAnimator>();
        sun = GameObject.Find("Directional Light").GetComponent<Light>();
        locationCount = Enum.GetNames(typeof(Location)).Length;
        seasonCount = Enum.GetNames(typeof(Season)).Length;
        lightingCount = Enum.GetNames(typeof(TimeOfDay)).Length;
        presetCount = Enum.GetNames(typeof(Preset)).Length;
        
        
        pictureTexts[2].text = currentSeason.ToString();
        pictureTexts[1].text = currentLocation.ToString();
        pictureTexts[0].text = currentTimeOfDay.ToString();

        Invoke(nameof(ResetSetting),0);
    }


    private void Update()
    {
        if (!fuckItCheck)
        {
            
            locationManager.ChangeLocation(Location.Forest);
            fuckItCheck = true;
            
            locationManager.ChangeLocation(Location.City);
        }
    }

    public void ResetSetting()
    {
        seasonManager.ChangeToSeason(Season.Summer);
        locationManager.ChangeLocation(Location.Forest);
        timeOfDayManager.UpdateTimeOfDay(TimeOfDay.Day, currentLocation);
    }

    

    #region Buttons

    [Button(ButtonMode.EnabledInPlayMode)] public void NextSeason() => ChangeSeason(true);
    [Button(ButtonMode.EnabledInPlayMode)] public void NextPreset() => ChangePreset(true);
    [Button(ButtonMode.EnabledInPlayMode)] public void NextLocation() => ChangeLocation(true);
    [Button(ButtonMode.EnabledInPlayMode)] public void NextLight() => ChangeTimeOfDay(true);
    [Button(ButtonMode.EnabledInPlayMode)] public void PreviousSeason() => ChangeSeason(false);
    [Button(ButtonMode.EnabledInPlayMode)] public void PreviousPreset() => ChangePreset(false);
    [Button(ButtonMode.EnabledInPlayMode)] public void PreviousLocation() => ChangeLocation(false);
    [Button(ButtonMode.EnabledInPlayMode)] public void PreviosLight() => ChangeTimeOfDay(false);

    #endregion
    public void Activate(GestureType predictedGesture,bool isRight,int hand)
    {
        switch (predictedGesture)
        {
            case GestureType.None1:
                break;
            case GestureType.Season:
                ChangeSeason(isRight);
                break;
            case GestureType.Location:
                ChangeLocation(isRight);
                break;
            case GestureType.None2:
                break;
            case GestureType.TimeOfDay:
                ChangeTimeOfDay(isRight);
                break;
            case GestureType.Activation:
                break;
            case GestureType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(predictedGesture), predictedGesture, null);
        }

        testConductor.LogChange(predictedGesture,hand);
    }

    private void ChangeTimeOfDay(bool next)
    {
        int sign = next ? 1 : -1;
        currentTimeOfDay = (TimeOfDay)(((int)currentTimeOfDay+lightingCount + sign) % lightingCount);
        timeOfDayManager.UpdateTimeOfDay(currentTimeOfDay,currentLocation);
        
        pictureTexts[0].text = currentTimeOfDay.ToString();
    }

    private void ChangePreset(bool next)
    {
        if (animator.animate)
        {
            return;
        }
        int sign = next ? 1 : -1;
        currentPreset = (Preset)(((int)currentPreset+presetCount + sign) % presetCount);

        animator.animatingTo = currentPreset;
    }

    private void ChangeLocation(bool next)
    {
        int sign = next ? 1 : -1;
        currentLocation = (Location)(((int)currentLocation+locationCount + sign) % locationCount);
        locationManager.ChangeLocation(currentLocation);
        timeOfDayManager.UpdateTimeOfDay(currentTimeOfDay,currentLocation);
        
        pictureTexts[1].text = currentLocation.ToString();
    }

    private void ChangeSeason(bool next)
    {
        int sign = next ? 1 : -1;
        currentSeason = (Season)(((int)currentSeason+seasonCount + sign) % seasonCount);
        seasonManager.ChangeToSeason(currentSeason);
        
        pictureTexts[2].text = currentSeason.ToString();
    }

    
}

