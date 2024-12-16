using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D),typeof(Character))]
public class PlayerControler : MonoBehaviour
{

    public IInteractable currentInteractable;
    public bool isInInteractArea;
    private bool isUsingTool = false;
    public int coins;

    [Header("Reference")]
    public BoxCollider2D coll;
    public Rigidbody2D rb;
    public Character character;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        character = GetComponent<Character>();
    }
    private void Update()
    {
        JudgeInteractInput();
    }
    private void FixedUpdate()
    {
        Movement();
        
    }


    private Vector2 MoveInput { get { return InputManager.Instance.MoveInput; } }
    private void Movement()
    {
        if (TimeManager.Instance.IsGameTimePause)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = character.currentWalkingSpeed * MoveInput;
    }

    private void JudgeInteractInput()
    {
        if (InputManager.Instance.InteractInput && isInInteractArea)
        {
            currentInteractable.TriggerAction();
        }
    }



    public void SetIsUsingTool(bool isUsingTool)
    {
        this.isUsingTool = isUsingTool;
    }

    public PlayerSaveData GetSaveData()
    {
        PlayerSaveData playerSaveData = new PlayerSaveData();
        playerSaveData.position = new SerilizableVector3(transform.position);
        playerSaveData.coins = coins;
        Inventory inventory = GetComponent<Inventory>();
        if (inventory != null && inventory.bagData != null)
            playerSaveData.inventoryData = inventory.bagData.items;
        return playerSaveData;
    }

    public void LoadSaveData(PlayerSaveData playerSaveData)
    {
        transform.position = playerSaveData.position.ToVector3();
        coins = playerSaveData.coins;
        Inventory inventory = GetComponent<Inventory>();
        if (inventory != null && playerSaveData.inventoryData != null)
            inventory.bagData.items = playerSaveData.inventoryData;
    }
}

public class PlayerSaveData
{
    public SerilizableVector3 position;
    public int coins;
    public List<InventoryItem> inventoryData;
}
