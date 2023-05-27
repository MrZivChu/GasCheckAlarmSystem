using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MyEditorWindow : EditorWindow
{
    [MenuItem("Window/ChangeFont")]
    static void ChangeFont()
    {
        MyEditorWindow myEditorWindow = (MyEditorWindow)EditorWindow.GetWindow(typeof(MyEditorWindow), false, "设置字体", true);
        myEditorWindow.Show();
    }

    UnityEngine.Object fontObj;
    void OnGUI()
    {
        GUILayout.Label("请添加字体");
        fontObj = EditorGUILayout.ObjectField(fontObj, typeof(Font), true);
        if (GUILayout.Button("确定") && fontObj != null)
        {
            Text[] textArray = Resources.FindObjectsOfTypeAll<Text>();
            Text currentText = null;
            for (int i = 0; i < textArray.Length; i++)
            {
                currentText = textArray[i];
                Undo.RecordObject(currentText, currentText.name);
                textArray[i].font = (Font)fontObj;
                EditorUtility.SetDirty(currentText);
            }
            EditorUtility.DisplayDialog("提示", "设置字体完成", "确定");
            Debug.Log("共设置字体" + textArray.Length + "个");
        }
    }
}
