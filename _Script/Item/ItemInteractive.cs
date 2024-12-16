using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class ItemInteractive : MonoBehaviour
{
    [Header("NEED TO SET")]
    [SerializeField] private Transform itemSpriteTrans;
    [Space]
    private bool isAnimating = false;
    private WaitForSeconds pause = new WaitForSeconds(0.04f);
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if (collision.transform.position.x < transform.position.x)
            {
                // Interacter on the left
                StartCoroutine(RotateRight());
            }
            else
            {
                // Interacter on the right
                StartCoroutine(RotateLeft());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if (collision.transform.position.x < transform.position.x)
            {
                // Interacter on the left
                StartCoroutine(RotateLeft());
            }
            else
            {
                // Interacter on the right
                StartCoroutine(RotateRight());
            }
        }
    }

    private IEnumerator RotateLeft()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            itemSpriteTrans.Rotate(0, 0, 2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            itemSpriteTrans.Rotate(0, 0, -2);
            yield return pause;
        }
        itemSpriteTrans.Rotate(0, 0, 2);
        yield return pause;
        isAnimating = false;
    }


    private IEnumerator RotateRight()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            itemSpriteTrans.Rotate(0, 0, -2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            itemSpriteTrans.Rotate(0, 0, 2);
            yield return pause;
        }
        itemSpriteTrans.Rotate(0, 0, -2);
        yield return pause;
        isAnimating = false;
    }
}
