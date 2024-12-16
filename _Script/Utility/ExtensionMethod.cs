using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public static class ExtensionMethod 
{
    public static Vector3 GetCumulativeScale(Transform trans)
    {
        Vector3 cumulativeScale = Vector3.one; // Start with no scale
        Transform currentGameObject = trans.parent;//Start from parent

        // Traverse up the hierarchy and multiply the scale factors
        while (currentGameObject != null)
        {
            cumulativeScale = Vector3.Scale(cumulativeScale, currentGameObject.localScale);
            currentGameObject = currentGameObject.parent;
        }
        return cumulativeScale;
    }
}
