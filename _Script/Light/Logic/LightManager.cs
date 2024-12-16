using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class LightManager : Singleton<LightManager>
{
    private LightControl[] sceneLights;
    private LightShift sunOnHorizonLightShift;
    private LightShift noonOrNightLightShift;
    private float sunOnHorizonPercent;


    private void UpdateLightStatus()
    {
        TimeSpan midNight = Settings.midNight;
        TimeSpan dawn = TimeManager.Instance.GetDawnTime();
        TimeSpan noon = Settings.noon;
        TimeSpan dusk = TimeManager.Instance.GetDuskTime();
        TimeSpan currentTime = TimeManager.Instance.GameTime;

        if (currentTime >= midNight && currentTime < dawn)
        {
            sunOnHorizonLightShift = LightShift.Dawn;
            noonOrNightLightShift = LightShift.MidNight;
        }
        else if (currentTime >= dawn && currentTime < noon)
        {
            sunOnHorizonLightShift = LightShift.Dawn;
            noonOrNightLightShift = LightShift.Noon;
        }
        else if (currentTime >= noon && currentTime < dusk)
        {
            sunOnHorizonLightShift = LightShift.Dusk;
            noonOrNightLightShift = LightShift.Noon;
        }
        else
        {
            sunOnHorizonLightShift = LightShift.Dusk;
            noonOrNightLightShift = LightShift.MidNight;
        }

        float timeDistToDawn = (float)Math.Abs((dawn - currentTime).TotalMinutes);
        float timeDistToDusk = (float)Math.Abs((dusk - currentTime).TotalMinutes);
        float sunOnHorizonTimeFloat = (float)Settings.sunOnHorizonTime.TotalMinutes;
        if (timeDistToDawn < sunOnHorizonTimeFloat)
        {
            sunOnHorizonPercent = (sunOnHorizonTimeFloat - timeDistToDawn) / sunOnHorizonTimeFloat;
        }
        else if (timeDistToDusk < sunOnHorizonTimeFloat)
        {
            sunOnHorizonPercent = (sunOnHorizonTimeFloat - timeDistToDusk) / sunOnHorizonTimeFloat;
        }
        else
        {
            sunOnHorizonPercent = 0;
        }

    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        UpdateLightStatus();
        foreach (LightControl lightControl in sceneLights)
        {
            lightControl.ChangeLight(sunOnHorizonLightShift, noonOrNightLightShift, sunOnHorizonPercent);
        }
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool arg2)
    {
        UpdateLightStatus();
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl lightControl in sceneLights)
        {
            lightControl.ChangeLight(sunOnHorizonLightShift, noonOrNightLightShift, sunOnHorizonPercent);
        }
    }

}
