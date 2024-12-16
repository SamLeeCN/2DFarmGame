using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class LightControl : MonoBehaviour
{
    public LightPatternListSO lightPatternListSO;
    private Light2D currentLight;
    private LightDetails sunOnHorizonDetails;
    private LightDetails noonOrNightDetails;
    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    public void ChangeLight(LightShift sunOnHorizonShift, LightShift noonOrNightShift, float sunOnHorizonPercent)
    {
        sunOnHorizonDetails = lightPatternListSO.GetLightDetails(sunOnHorizonShift);
        noonOrNightDetails = lightPatternListSO.GetLightDetails(noonOrNightShift);
        
        Color currentColor = sunOnHorizonDetails.color * sunOnHorizonPercent + noonOrNightDetails.color *(1 - sunOnHorizonPercent);
        float currentIntensity = sunOnHorizonDetails.intensity * sunOnHorizonPercent + noonOrNightDetails.intensity * (1 - sunOnHorizonPercent);
        currentLight.color = currentColor;
        currentLight.intensity = currentIntensity;
    }

}
