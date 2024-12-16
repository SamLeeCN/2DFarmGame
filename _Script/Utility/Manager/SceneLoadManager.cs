using DG.Tweening.Core.Easing;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class SceneLoadManager : Singleton<SceneLoadManager>
{//Not naming it SceneManager to avoid conflict with Unity pre-set class
    public bool isSceneLoading;
    public List<GameSceneSO> sceneList;
    public GameSceneSO mainMenuScene;
    public GameSceneSO startScene;
    public GameSceneSO natureScene;
    public GameSceneSO insideScene;

    public GameSceneSO currentScene;
    [SerializeField] private GameSceneSO sceneToLoad;

    [SerializeField] private TeleportType teleportType;
    [SerializeField] private Vector3 posToGo;
    [SerializeField] private int idToGo;
    [SerializeField] private bool isSameSceneTeleport = true;

    [SerializeField] private float screenFadeDuration = 1;

    public Dictionary<string, bool> isSceneFirstLoadDict = new Dictionary<string, bool>();
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.SceneLoadEvent += OnSceneLoadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.NewGameEvent += OnNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.SceneLoadEvent -= OnSceneLoadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.NewGameEvent -= OnNewGameEvent;
    }

    public GameSceneSO GetSceneSOBySceneName(string sceneName)
    {
        return sceneList.Find(s => s.sceneName == sceneName);
    }

    private void OnNewGameEvent()
    {
        EventHandler.CallSceneLoadEvent(natureScene, false, false);
    }

    void Awake()
    {
        //SetFileName();
        //FIXME:Load main menu after main menu is created

    }
    private void Start()
    {
        EventHandler.CallSceneLoadEvent(mainMenuScene, false, true);
        // FIXME: Need to be Save, instead of reset every time when player start the game
        foreach (MapDataSO mapData in GridMapManager.Instance.mapDataList)
        {
            isSceneFirstLoadDict.Add(mapData.gameScene.sceneName, true);
        }

    }
    public void TeleportFromEntrance(TeleportEntrance entrance)
    {
        teleportType = entrance.teleportType;
        sceneToLoad = entrance.sceneToGo;
        isSameSceneTeleport = entrance.isSameSceneTeleport;
        switch (entrance.teleportType)
        {
            case TeleportType.Pos:
                posToGo = entrance.posToGo;
                TeleportPos();
                break;
            case TeleportType.Trans:
                idToGo = entrance.teleportDestinationIdToGo;
                TeleportTrans();
                break;
        }
    }
    private void TeleportTrans()
    {
        Transform player = GameManager.Instance.playerCharacter.transform;
        NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>(); ;

        if (isSameSceneTeleport)
        {
            TeleportDestination destination = GetTeleportDestination(idToGo);
            playerAgent.enabled = false;
            player.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
            playerAgent.enabled = true;
        }
        else
        {
            EventHandler.CallSceneLoadEvent(sceneToLoad, true);
        }
    }
    private void TeleportPos()
    {
        Transform player = GameManager.Instance.playerCharacter.transform;
        if (isSameSceneTeleport)
        {
            player.SetPositionAndRotation(posToGo, player.rotation);
        }
        else
        {
            EventHandler.CallSceneLoadEvent(sceneToLoad, true);
        }
    }
    private TeleportDestination GetTeleportDestination(int transportDestinationIdToGo)
    {
        TeleportDestination[] destinations;
        destinations = FindObjectsOfType<TeleportDestination>();
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i].id == transportDestinationIdToGo)
            {
                return destinations[i];
            }
        }
        return null;
    }

    private void OnBeforeSceneUnloadEvent(GameSceneSO sO, bool isLoadData)
    {
        isSceneLoading = true;
    }

    private void OnSceneLoadEvent(GameSceneSO sceneToLoad, bool doTeleport, bool doFadeScreen, bool isLoadData)
    {
        StartCoroutine(SceneLoadEnumerator(sceneToLoad, doTeleport, doFadeScreen, isLoadData));
    }
    private IEnumerator SceneLoadEnumerator(GameSceneSO sceneToLoad, bool doTeleport, bool doFadeScreen, bool isLoadData)
    {
        if (doFadeScreen)
        {
            EventHandler.CallScreenFadeInEvent(screenFadeDuration);
        }
        if (currentScene != null)
        {
            ResetSceneSettings(currentScene);
            EventHandler.CallBeforeSceneUnloadEvent(currentScene, isLoadData);
            yield return new WaitForSeconds(screenFadeDuration / 2);
            GameManager.Instance.playerControler.gameObject.SetActive(false);
            if (currentScene.sceneReference != sceneToLoad.sceneReference)
                yield return currentScene.sceneReference.UnLoadScene();
        }
        if (currentScene == null || currentScene.sceneReference != sceneToLoad.sceneReference)
        {
            //var handle = SceneManager.LoadSceneAsync(sceneToLoad.sceneName, LoadSceneMode.Additive);
            var handle = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
            //true means activate the scene after it is loaded
            yield return handle;
            SceneManager.SetActiveScene(handle.Result.Scene);
            //if it's not active,active by this line
            currentScene = sceneToLoad;
        }
        SetSceneSettings(currentScene);

        EventHandler.CallAfterSceneLoadEvent(doTeleport, isSceneFirstLoadDict[currentScene.sceneName]);
        isSceneFirstLoadDict[currentScene.sceneName] = false;
    }
    private void ResetSceneSettings(GameSceneSO sceneToUnload)
    {

        //if (sceneToUnload.isOverrideCamera) CameraManager.Instance.SetPlayerCameraEnablity(true);
        if (sceneToUnload.isPlyerDisabled) GameManager.Instance.SetPlayerEnability(true);

        if (sceneToUnload.isWorldUIDisabled) UIManager.Instance.SetUIEnability(true);

        if (sceneToUnload.isInputDisabled) InputManager.Instance.SetInputEnability(true);

        if (sceneToUnload == mainMenuScene) MainMenuPanel.Instance.IsOpen = false;
    }
    private void SetSceneSettings(GameSceneSO sceneLoaded)
    {

        //if (sceneLoaded.isOverrideCamera) CameraManager.Instance.SetPlayerCameraEnablity(false);
        if (sceneLoaded.isPlyerDisabled) GameManager.Instance.SetPlayerEnability(false);

        if (sceneLoaded.isWorldUIDisabled) UIManager.Instance.SetUIEnability(false);

        if (sceneLoaded.isInputDisabled) InputManager.Instance.SetInputEnability(false);

        if (sceneLoaded == mainMenuScene) MainMenuPanel.Instance.IsOpen = true;
    }
    private void OnAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
    {
        if (currentScene.isPlyerDisabled)
        {
            isSceneFirstLoadDict[currentScene.sceneName] = false;
            isSceneLoading = false;
            return;
        }
        GameManager.Instance.SetPlayerEnability(true);
        if (!doTeleport)
        {
            isSceneFirstLoadDict[currentScene.sceneName] = false;
            isSceneLoading = false;
            return;
        }
        Transform player = GameManager.Instance.playerCharacter.transform;
        if (teleportType == TeleportType.Trans)
        {
            TeleportDestination destination = GetTeleportDestination(idToGo);
            player.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
        }
        else
        {
            player.SetPositionAndRotation(posToGo, player.rotation);
        }
        EventHandler.CallScreenFadeOutEvent(screenFadeDuration);

        isSceneFirstLoadDict[currentScene.sceneName] = false;
        isSceneLoading = false;
    }

    public string GetCurrentSceneName()
    {
        return currentScene.sceneName;
    }


    /*public void SetFileName()
    {
        currentSceneSOKeyName = "savedScene";
    }
    public DataDefination GetID()
    {
        return GetComponent<DataDefination>();
    }

    public void SaveData()
    {
        //Save SO
        DataManager.Instance.SaveSO(currentScene, currentSceneSOKeyName);
    }
    public void LoadData()
    {
        //Load SO
        if (GameManager.Instance.IsPlayerEnable())
        {
            GameManager.Instance.playerControler.agent.enabled = false;
            GameManager.Instance.SetPlayerEnablity(false);
        }
        GameSceneSO savedScene = new GameSceneSO();
        if (DataManager.Instance.LoadSO(savedScene, currentSceneSOKeyName))
        {
            EventManager.Instance.RaiseSceneLoadEvent(savedScene, false, true);
        }
        else
        {
            EventManager.Instance.RaiseSceneLoadEvent(natureScene, false, true);//Load First Scene(not menu!)
        }
    }
    public void DeleteData()
    {
        //DeleteSO
        PlayerPrefs.DeleteKey(currentSceneSOKeyName);
    }*/
}
