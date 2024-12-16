using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator[] animators;

    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    void Update()
    {
        MoveInputAnimation();
    }

    private void OnEnable()
    {
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, InventoryDataSO inventoryData, int slotIndex)
    {
        //Perform Animation
        ItemDetail itemDetail = inventoryData.items[slotIndex].Deatail;
        if (itemDetail.itemType != ItemType.Seed && itemDetail.itemType != ItemType.Commodity && itemDetail.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - transform.position.y;

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }

            StartCoroutine(UseToolRoutine(mouseWorldPos, inventoryData, slotIndex));
        }
        else
        {
            EventHandler.CallExcuteClickWorldActionEvent(mouseWorldPos, inventoryData, slotIndex);
        }
        

    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, InventoryDataSO inventoryData, int slotIndex)
    {
        GameManager.Instance.playerControler.SetIsUsingTool(true);
        yield return null;
        foreach (Animator animator in animators)
        {
            animator.SetFloat("MouseX", mouseX);
            animator.SetFloat("MouseY", mouseY);
            animator.SetTrigger("UseTool");
        }

        yield return new WaitForSeconds(0.45f);
        //Execute Action
        EventHandler.CallExcuteClickWorldActionEvent(mouseWorldPos, inventoryData, slotIndex);
        yield return new WaitForSeconds(0.25f);

        GameManager.Instance.playerControler.SetIsUsingTool(false);
    }

    void MoveInputAnimation()
    {
        float xInput = InputManager.Instance.MoveInput.x;
        float yInput = InputManager.Instance.MoveInput.y;
        foreach (Animator animator in animators)
        {
            if (xInput != 0 || yInput != 0)
            {
                animator.SetFloat("InputX", xInput);
                animator.SetFloat("InputY", yInput);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }


}
