using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class GameInput
{
    public InputAction action;
    public float downTimer;
    public float upTimer;
    public bool ignoreDoubleClick;

    public bool isDown;
    public bool isUp;
    public bool isClick;
    public bool isLongPress;
    public bool isHold;
    public bool isDoubleClick;

    private bool isDoingInput;
    private bool hasLongPressedThisInput;
    private bool isThisInputDoubleClick;
    public void UpdateFrame()
    {   
        isClick = false;
        isLongPress = false;
        isDoubleClick = false;

        isDown = action.WasPressedThisFrame();
        isHold = action.WasPerformedThisFrame();
        isUp = action.WasReleasedThisFrame();
        

        
        if (isHold&&downTimer < Settings.longPressThreshold) downTimer += Time.deltaTime;
        if (!isHold&&upTimer < Settings.doubleClickInterval) upTimer += Time.deltaTime;

        if (isDown)
        {
            isDoingInput = true;
            downTimer = 0;
            hasLongPressedThisInput = false;
            isThisInputDoubleClick = false;
            if (upTimer < Settings.longPressThreshold)
            {
                isDoubleClick = true;
                isThisInputDoubleClick = true;
            }
        }
        if (isUp)
        {
            upTimer = 0;
        }
        
        if (upTimer > Settings.doubleClickInterval && downTimer < Settings.longPressThreshold && isDoingInput)
        {
            if (!isThisInputDoubleClick) isClick = true;

            isDoingInput = false;
        }

        if (isHold && downTimer > Settings.longPressThreshold && !hasLongPressedThisInput)
        {
            isLongPress = true;
            hasLongPressedThisInput = true;
        }

        if (upTimer > Settings.doubleClickInterval)
        {
            isDoingInput = false;
        }
    }
}
public class InputManager : Singleton<InputManager>
{
    private bool isInputDisabled = false;

    public PlayerInputControler inputControler;

    public Vector2 MoveInput { get; private set; }

    public bool InteractInput { get; private set; }
    public bool TalkWithNPCInput { get; private set; }


    public bool ConfirmInput { get; private set; }

    public bool CancelInput { get; private set; }

    public bool PageUpInput { get; private set; }

    public bool PageDownInput { get; private set; }

    public bool BagToggleInput { get; private set; }

    public bool DropEachItemInput { get; private set; }
    public bool ShiftHoldingInput { get; private set; }
    public bool LeftMouseLongPressInput { get; private set; }

    public bool SettingsToggleInput { get; private set; }
    private float leftMousePressTimer = 0;
    
    public InputAction moveAction;
    public InputAction interactAction;
    public InputAction talkWithNPCAction;

    public InputAction confirmAction;
    public InputAction cancelAction;
    public InputAction pageUpAction;
    public InputAction pageDownAction;
    public InputAction bagToggleAction;
    public InputAction dropEachItemAction;
    public InputAction shiftHoldingAction;
    public InputAction settingsToggleAction;


    protected void Awake()
    {
        if (inputControler == null) inputControler = new PlayerInputControler();
        SetUpInputAction();
    }
    private void OnEnable()
    {
        inputControler.Enable();
    }
    private void OnDisable()
    {
        inputControler.Disable();
    }
    void Update()
    {
        UpdateInputs();
        if (Input.GetMouseButton(0))
        {
            leftMousePressTimer += Time.deltaTime;
        }
        if(Input.GetMouseButtonUp(0)) leftMousePressTimer = 0;

    }
    public void SetUpInputAction()
    {
        moveAction = inputControler.GamePlay.Move;
        interactAction = inputControler.GamePlay.Interact;
        talkWithNPCAction = inputControler.GamePlay.TalkWithNPC;

        confirmAction = inputControler.UI.Confirm;
        cancelAction = inputControler.UI.Cancel;
        pageUpAction = inputControler.UI.PageUp;
        pageDownAction = inputControler.UI.PageDown;
        bagToggleAction = inputControler.UI.BagToggle;
        dropEachItemAction = inputControler.UI.DropEachItem;
        shiftHoldingAction = inputControler.UI.ShiftHolding;
        settingsToggleAction = inputControler.UI.SettingsToggle;

    }
    private void UpdateInputs()
    {
        // Inputs that can't be disabled
        ConfirmInput = confirmAction.WasPressedThisFrame();
        CancelInput = cancelAction.WasPressedThisFrame();
        PageUpInput = pageUpAction.WasPressedThisFrame();
        PageDownInput = pageDownAction.WasPressedThisFrame();

        if (isInputDisabled) return;

        // Inputs that can be disabled
        MoveInput = moveAction.ReadValue<Vector2>();
        if (MoveInput.sqrMagnitude > 1)
        {
            MoveInput = MoveInput.normalized;
        }
        InteractInput = interactAction.WasPressedThisFrame();
        TalkWithNPCInput = talkWithNPCAction.WasPressedThisFrame();
        
        BagToggleInput = bagToggleAction.WasPressedThisFrame();
        DropEachItemInput = dropEachItemAction.WasPressedThisFrame();
        ShiftHoldingInput = shiftHoldingAction.WasPressedThisFrame();
        
        LeftMouseLongPressInput = Input.GetMouseButton(0) && leftMousePressTimer > Settings.longPressThreshold;
        SettingsToggleInput = settingsToggleAction.WasPressedThisFrame();

    }

    public void SetInputEnability(bool enability)
    {
        isInputDisabled = !enability;
    }
}
