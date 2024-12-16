using Farm.AStar;
using Farm.InventoryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public NpcSO currentNpcSO;
    public string npcName;
    public ScheduleDataListSO currentScheduleListSO;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;

    [SerializeField] public string currentScene;
    [SerializeField] public string targetScene;
    [SerializeField] private Vector3Int currentGridCoordinate;
    [SerializeField] private Vector3Int targetGridCoordinate;
    [SerializeField] private Vector3Int nextGridCoordinate;

    public string StartScene { set => currentScene = value; }
    [Header("Movement Properties")]
    public float normalSpeed = 2f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 20f;
    private Vector2 dir;
    public bool isMovingPhysics;
    public bool isMovingAnimation;
    private bool isInitialized;
    private float stopAnimationBreakTimer;
    private bool canAnimationBreak;
    private AnimationClip stopAnimationClip;
    [SerializeField] private AnimationClip blankAnimationClip;
    private AnimatorOverrideController animatorOverride;

    private Vector3 nextWorldPos;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;
    private SpriteRenderer spriteRenderer;
    private Stack<MovementStep> movementSteps;
    private Transform shadowTrans;
    private Grid grid;
    private TimeSpan GameTime => TimeManager.Instance.GameTime;
    public bool isInteractable;
    public bool IsScheduling => movementSteps.Count > 0;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowTrans = transform.Find("Shadow");
        movementSteps = new Stack<MovementStep>();

        animatorOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animatorOverride;

        currentScheduleListSO = currentNpcSO.regularSchedule;

        scheduleSet = new SortedSet<ScheduleDetails>();

        foreach (ScheduleDetails schedule in currentScheduleListSO.scheduleDataList)
        {
            scheduleSet.Add(schedule);
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
        if (!IsScheduling && currentSchedule != null)
        {
            if (currentSchedule.isSpecial)
                scheduleSet.Remove(currentSchedule);
            currentSchedule = null;
        }

        int time = hour * 60 + minute;
        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {

            if (schedule.Time > time) break;

            if (schedule.day != day && schedule.day != 0)
                continue;

            if (schedule.season != season && !schedule.isSpecial)
                continue;

            if (currentSchedule != null && schedule.priority < currentSchedule.priority)
                continue;

            matchSchedule = schedule;
        }
        if (matchSchedule != null && matchSchedule!=currentSchedule 
            && 
            Vector3.Distance
            (transform.position, GetWorldPosByCoordinate((Vector3Int)matchSchedule.targetGridCoordinate)) > Settings.pixelSize)
        {
            
            BuiltPath(matchSchedule);
        }
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool isFirstLoad)
    {
        
        grid = FindObjectOfType<Grid>();
        CheckVisible();
        if (!isInitialized)
        {
            InitNPC();
            isInitialized = true;
        }

        AfterSceneLoadSpecialScheduleCheck(isFirstLoad);
    }

    void Start()
    {

    }

    void Update()
    {

        if (!SceneLoadManager.Instance.isSceneLoading)
            SwitchAnimation();
        if (stopAnimationBreakTimer >= 0)
        {
            stopAnimationBreakTimer -= Time.deltaTime;
            canAnimationBreak = false;
        }
        else
        {
            canAnimationBreak = true;
        }
    }
    private void FixedUpdate()
    {
        if (!SceneLoadManager.Instance.isSceneLoading) 
            Movement();
    }
    private void CheckVisible()
    {
        if (currentScene == SceneLoadManager.Instance.currentScene.sceneName)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
    }

    #region Set Npc Visibility
    public void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        //shadowTrans.gameObject.SetActive(true);
    }

    public void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        //shadowTrans.gameObject.SetActive(false);
    }
    #endregion


    private void InitNPC()
    {
        targetScene = currentScene;
        currentGridCoordinate = grid.WorldToCell(transform.position);
        //transform.position = new Vector3(currentGridCoordinate.x + Settings.gridCellSize / 2f, currentGridCoordinate.y + Settings.gridCellSize / 2, 0);
        transform.position = GetWorldPosByCoordinate(currentGridCoordinate);
        targetGridCoordinate = currentGridCoordinate;

    }

    private void Movement()
    {
        if (TimeManager.Instance.IsGameTimePause) return;
        if (!isMovingPhysics) {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();

                currentScene = step.sceneName;

                CheckVisible();

                nextGridCoordinate = (Vector3Int)step.gridCoordinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridCoordinate(nextGridCoordinate, stepTime);

            }
            else if (!isMovingAnimation && canAnimationBreak)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridCoordinate(Vector3Int gridCoordinate, TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridCoordinate, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridCoordinate, TimeSpan stepTime)
    {
        isMovingPhysics = true;
        nextWorldPos = GetWorldPosByCoordinate(gridCoordinate);

        if(GameTime < stepTime)
        {
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            float distance = Vector3.Distance(transform.position, GetWorldPosByCoordinate(gridCoordinate));
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.gameTimeThreshold));
            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPos) > Settings.pixelSize)
                {
                    dir = (nextWorldPos - transform.position).normalized;
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // when GameTime has exceed step time
        rb.position = nextWorldPos;
        currentGridCoordinate = gridCoordinate;
        nextGridCoordinate = currentGridCoordinate;
        isMovingPhysics = false;
    }

    public void BuiltPath(ScheduleDetails schedule)
    {
        isInteractable = schedule.isInteractable;
        movementSteps.Clear();
        currentSchedule = schedule;
        targetScene = schedule.targetScene;
        currentGridCoordinate = grid.WorldToCell(transform.position);
        targetGridCoordinate = (Vector3Int) schedule.targetGridCoordinate;
        stopAnimationClip = schedule.animationClipAtStop;


        if (targetScene == currentScene)
        {
            AStar.Instance.BuildPath(targetScene, (Vector2Int)currentGridCoordinate, (Vector2Int)targetGridCoordinate, movementSteps);
        }
        else
        {
            
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, targetScene);

            if (sceneRoute != null)
            {
                for (int i = sceneRoute.scenePathList.Count -1; i >= 0; i--)
                {
                    Vector2Int fromCoordinate, toCoordinate;
                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    if (scenePath.activeFromGridCell)
                    {
                        fromCoordinate = scenePath.fromGridCell;
                    }
                    else
                    {
                        fromCoordinate = (Vector2Int) currentGridCoordinate;
                    }

                    if (scenePath.activeGoToGridCell)
                    {
                        toCoordinate = scenePath.goToGridCell;
                    }
                    else
                    {
                        toCoordinate = (Vector2Int) targetGridCoordinate;
                    }

                    AStar.Instance.BuildPath(scenePath.sceneName, fromCoordinate, toCoordinate, movementSteps);
                }
            }
        }
        if (movementSteps.Count > 0)
        {
            UpdateTimeOnPath();
            //PrintMovementStep();
        }

    }
    private void PrintMovementStep()
    {
        if (currentNpcSO == NPCManager.Instance.npcGirl01) // Specify NPC
        {
            for (int i = 0; i < movementSteps.Count; i++)
            {
                Debug.Log($"Step {i} : {movementSteps.ToArray()[i].hour} {movementSteps.ToArray()[i].minute} {movementSteps.ToArray()[i].second} \n {movementSteps.ToArray()[i].gridCoordinate}");
            }
        }
    }

    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;
        TimeSpan currentGameTime = GameTime; 
        // a property(GameTime) has no reference, so currentGameTime is a new TimeSpan

        foreach (var step in movementSteps)
        {
            if (previousStep == null)
            {
                previousStep = step;
            }

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;
            if (JudgeMoveDiagonal(step, previousStep))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.gameTimeThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.gameTimeThreshold));

            currentGameTime = currentGameTime.Add(gridMovementStepTime);

            previousStep = step;
        }
    }

    private bool JudgeMoveDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentGridCoordinate.x!=previousStep.gridCoordinate.x) && (currentGridCoordinate.y!=previousStep.gridCoordinate.y);
    }

    private Vector3 GetWorldPosByCoordinate(Vector3Int gridCoordinate)
    {
        Vector3 worldPos = grid.CellToWorld(gridCoordinate);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }


    private void SwitchAnimation()
    {
        isMovingAnimation = Vector3.Distance(transform.position, GetWorldPosByCoordinate(targetGridCoordinate)) > Settings.gridCellSize/5f;
        anim.SetBool("IsMoving", isMovingAnimation);
        if (isMovingAnimation)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        stopAnimationBreakTimer = Settings.npcAnimationBreakTime;

        if (stopAnimationClip != null)
        {
            animatorOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animatorOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }

    public void AddSpecialSchedule(ScheduleDetails schedule)
    {
        scheduleSet.Add(schedule);
    }

    private void AfterSceneLoadSpecialScheduleCheck(bool isFirstLoad)
    {
        if (SceneLoadManager.Instance.currentScene == SceneLoadManager.Instance.startScene 
            && isFirstLoad &&
            currentNpcSO == NPCManager.Instance.npcGirl01)
        {
            ScheduleDetails schedule = new ScheduleDetails(NPCManager.Instance.specialScheduleSO.girl01StartSchedule);
            schedule.UpdateStartTime();
            AddSpecialSchedule(schedule);
            //PrintScheduleSet();
        }


    }

    public void PrintScheduleSet()
    {
        string str = currentNpcSO.npcName + "\n";
        int i = 0;
        foreach(var schedule in scheduleSet)
        {
            str += i + "\n";
            str += "day " + schedule.day + " hour " + schedule.hour + " minute " + schedule.minute + "\n";
            str += "targetScene " + schedule.targetScene + " targetCoordinate " + schedule.targetGridCoordinate + "\n";
        }
        Debug.Log(str);
    }

    #region Save and Load
    

    public NpcData GetSaveData()
    {
        NpcData npcData = new NpcData();
        npcData.npcName = npcName;
        npcData.currentScene = currentScene;
        npcData.position = new SerilizableVector3(transform.position);
        if (currentSchedule != null)
            npcData.currentSchedule = currentSchedule.GetSaveData();
        NPCFunction npcFunction = GetComponent<NPCFunction>();
        if (npcFunction != null)
            npcData.npcFunction = npcFunction.GetSaveData();
        npcData.isInteractable = isInteractable;
        npcData.dirX = dir.x;
        npcData.dirY = dir.y;
        

        return npcData;
    }

    public void LoadNpcData(NpcData npcData)
    {
        movementSteps.Clear();
        npcName = npcData.npcName;
        currentScene = npcData.currentScene;
        transform.position = npcData.position.ToVector3();
        if (npcData.currentSchedule != null)
            currentSchedule = new ScheduleDetails(npcData.currentSchedule);
        NPCFunction npcFunction = GetComponent<NPCFunction>();
        if (npcFunction != null && npcData.npcFunction != null)
            npcFunction.LoadSaveData(npcData.npcFunction);
        isInteractable = npcData.isInteractable;
        dir = new Vector2(npcData.dirX, npcData.dirY);
        

        isMovingPhysics = false;
        isMovingAnimation = false;
        if (currentSchedule != null)
            BuiltPath(currentSchedule);
    }
    #endregion
}

public class NpcData
{
    public string npcName;
    public string currentScene;
    public SerilizableVector3 position;
    public List<InventoryItem> inventory;
    public ScheduleDetaildsSaveData currentSchedule;
    public NpcFunctionSaveData npcFunction;
    public bool isInteractable;
    public float dirX;
    public float dirY;
}
