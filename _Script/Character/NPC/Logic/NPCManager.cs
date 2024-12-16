using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class NPCManager : Singleton<NPCManager>
{
    public SpecialScheduleSO specialScheduleSO;
    public SceneRouteDataListSO sceneRouteData;
    //public List<NPCPosition> npcPositionList;
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();
    public Dictionary<string, NpcData> npcDataDict = new Dictionary<string, NpcData>();
    public NpcSO npcGirl01;
    public NpcSO npcMayor;
    
    void Start()
    {
        
    }

    void Update()
    {

    }

    public void UpdateNpcDataDict()
    {
        NPCMovement[] npcs = FindObjectsOfType<NPCMovement>();
        foreach (var npc in npcs)
        {
            string npcName = npc.currentNpcSO.npcName;
            NpcData npcData = npc.GetSaveData();
            if (!npcDataDict.ContainsKey(npcName))
                npcDataDict.Add(npcName, npcData);
            else
                npcDataDict[npcName] = npcData;
        }
    }

    public void RestoreNpcDataFromDict()
    {
        NPCMovement[] npcs = FindObjectsOfType<NPCMovement>();
        foreach(var npc in npcs)
        {
            string npcName = npc.currentNpcSO.npcName;
            NpcData npcData = npcDataDict[npcName];
            if (npcData != null)
                npc.LoadNpcData(npcData);
        }
    }


    private void Awake()
    {
        InitSceneRouteDict();
    }

    private void InitSceneRouteDict()
    {
        foreach (SceneRoute sceneRoute in sceneRouteData.sceneRouteList)
        {
            string key = GetSceneRouteKey(sceneRoute.fromSceneName, sceneRoute.toSceneName);

            if (sceneRouteDict.ContainsKey(key))
                continue;
            else
            {
                sceneRouteDict.Add(key, sceneRoute);
            }
        }
    }
    private string GetSceneRouteKey(string fromSceneName, string toSceneName)
    {
        return "from " + fromSceneName + " to " + toSceneName;
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        string key = GetSceneRouteKey(fromSceneName, toSceneName);
        return sceneRouteDict[key];
    }
}
