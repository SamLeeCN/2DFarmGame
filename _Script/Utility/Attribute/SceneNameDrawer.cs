using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;
    GUIContent[] sceneNames;
    readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //FIXME: List中同样字段会错误同步更改
        if (EditorBuildSettings.scenes.Length == 0) return;
        if (sceneIndex == -1)//没有预设值则进行初始化
        {
            InitSceneNameArray(property);
        }
        //newIndex为用户点击选择时返回的index
        int newIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if (newIndex != sceneIndex)
        {
            sceneIndex = newIndex;
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }

    /// <summary>
    /// 初始化sceneNames，并且根据已有的预设值情况进行初始化
    /// </summary>
    /// <param name="property"></param>
    private void InitSceneNameArray(SerializedProperty property)
    {
        //获取所有sceneNames
        var scenes = EditorBuildSettings.scenes;
        if (scenes.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Build Settings Scene is None") };
            return;
        }
        sceneNames = new GUIContent[scenes.Length];
        for (int i = 0; i < sceneNames.Length; i++)
        {
            string[] splits = scenes[i].path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);
            string name = splits.Length > 0 ? splits[splits.Length - 1] : "Deleted File";
            sceneNames[i] = new GUIContent(name);
        }

        //如果没有预设值 则设为0号scene
        if (string.IsNullOrEmpty(property.stringValue))
        {
            sceneIndex = 0;
        }
        else //已有预设值 自动选择对应scene
        {
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (property.stringValue == sceneNames[i].text)
                {
                    sceneIndex = i;
                    break;
                }
            }
            if (sceneIndex == -1)
                sceneIndex = 0;
        }
    }
}
#endif


