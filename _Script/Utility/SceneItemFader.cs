using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class SceneItemFader : MonoBehaviour
{
    [SerializeField]private SpriteRenderer[] spriteRenderersToFade;
    void Start()
    {
        if(spriteRenderersToFade == null)
        {
            spriteRenderersToFade[0] = gameObject.transform.parent.GetComponent<SpriteRenderer>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Color targetColor = new Color(1,1,1,Settings.sceneItemFadeAlpha);
            foreach (var item in spriteRenderersToFade)
            {
                item.DOColor(targetColor, Settings.fadeDuration);
            }
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Color targetColor = new Color(1, 1, 1,1);   
            foreach (var item in spriteRenderersToFade)
            {
                item.DOColor(targetColor, Settings.fadeDuration);
            }
        }
    }
}
