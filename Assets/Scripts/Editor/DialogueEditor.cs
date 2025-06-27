using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

//指定自定义编辑器的目标类型
[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor : Editor
{
    //在Inspector窗口不停绘制
    public override void OnInspectorGUI()
    {
        //在原有内容的上方绘制内容
        if (GUILayout.Button("Open in My Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }
        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    DialogueData_SO currentData;//当前编辑的DialogueData_SO数据

    [MenuItem("qbdl/Dialogue Editor")]

    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("My Dialogue Editor");
        editorWindow.autoRepaintOnSceneChange = true;//使得该editor窗口持续刷新
    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("My Dialogue Editor");
        editorWindow.currentData = data;
    }

    [OnOpenAsset]
    public static bool OpenAsset(int instanceID, int line)
    {
        DialogueData_SO data = (DialogueData_SO)(EditorUtility.InstanceIDToObject(instanceID));

        if (data != null)//点击的是DialogueData_SO类型的资源且不为空
        {
            DialogueEditor.InitWindow(data);//初始化编辑器窗口并传入数据
            return true; //返回true表示该编辑器处理了打开事件
        }
        return false;
    }
}
