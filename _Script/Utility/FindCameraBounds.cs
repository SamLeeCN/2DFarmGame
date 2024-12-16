using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class FindCameraBounds : MonoBehaviour
{
    void Start()
    {
        SwitchConfinerShape();
    }

    /// <summary>
    /// Call when a scene is loaded
    /// </summary>
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;
        confiner.InvalidatePathCache();
    }
}
