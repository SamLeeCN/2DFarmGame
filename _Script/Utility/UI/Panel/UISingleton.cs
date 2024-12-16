using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class UISingleton<T> : MonoBehaviour where T : UISingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];

                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = string.Format("_{0}", typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] protected GameObject uiGameobject;
    protected RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = uiGameobject.GetComponent<RectTransform>();
    }

    public bool IsOpen
    {
        get { return uiGameobject.activeInHierarchy; }
        set
        {
            if (value) Open();
            else Close();
            GameManager.Instance.JudgeAndUpdateGameRunningStateByPanel();
        }
    }

    public event Action OpenEvent;
    public event Action CloseEvent;
    private void Open()
    {
        uiGameobject.SetActive(true);
        OpenEvent?.Invoke();
    }

    private void Close()
    {
        uiGameobject.SetActive(false);
        CloseEvent?.Invoke();
    }
}
