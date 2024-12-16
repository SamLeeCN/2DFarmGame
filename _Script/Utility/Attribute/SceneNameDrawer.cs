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
        //FIXME: List��ͬ���ֶλ����ͬ������
        if (EditorBuildSettings.scenes.Length == 0) return;
        if (sceneIndex == -1)//û��Ԥ��ֵ����г�ʼ��
        {
            InitSceneNameArray(property);
        }
        //newIndexΪ�û����ѡ��ʱ���ص�index
        int newIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if (newIndex != sceneIndex)
        {
            sceneIndex = newIndex;
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }

    /// <summary>
    /// ��ʼ��sceneNames�����Ҹ������е�Ԥ��ֵ������г�ʼ��
    /// </summary>
    /// <param name="property"></param>
    private void InitSceneNameArray(SerializedProperty property)
    {
        //��ȡ����sceneNames
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

        //���û��Ԥ��ֵ ����Ϊ0��scene
        if (string.IsNullOrEmpty(property.stringValue))
        {
            sceneIndex = 0;
        }
        else //����Ԥ��ֵ �Զ�ѡ���Ӧscene
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


