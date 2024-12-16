using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/LightPatternListSO")]
public class LightPatternListSO : ScriptableObject
{
    public bool ignoreWeather = true;
    public bool ignoreSeason = true;
    public List<LightDetails> lightPatternDetailsList = new List<LightDetails>();

    public LightDetails GetLightDetails(LightShift lightShift, Season season = Season.Spring, Weather weather = Weather.Sunny)
    {
        if (ignoreWeather) weather = Weather.Sunny;
        if (ignoreSeason) season = Season.Spring;
        return lightPatternDetailsList.Find(l => l.season == season && l.lightShift == lightShift && l.weather == weather);
    }
}
[System.Serializable]
public class LightDetails
{
    public Season season;
    public Color color;
    public float intensity;
    public LightShift lightShift;
    public Weather weather;
}