// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEditorInternal;
// using System;
// using System.IO;

// // 扩展 Inspector 窗口，为 DialogueData_SO 类型的对象提供额外的功能
// [CustomEditor(typeof(DialogueData_SO))]
// public class DialogueCustomEditor : Editor
// {
//     //在Inspector窗口不停绘制
//     public override void OnInspectorGUI()
//     {
//         //在原有内容的上方绘制内容（看在base.OnInspectorGUI();代码的上方还是下方）
//         if (GUILayout.Button("Open in My Editor"))
//             DialogueEditor.InitWindow((DialogueData_SO)target);
//         base.OnInspectorGUI();
//     }
// }

// // 独立的编辑器窗口，用于专门编辑 DialogueData_SO 数据
// public class DialogueEditor : EditorWindow
// {
//     DialogueData_SO currentData;//当前编辑的DialogueData_SO数据
//     ReorderableList pieceList = null;//对话片段列表
//     Vector2 scrollPos = Vector2.zero;//滚动位置

//     Dictionary<string, ReorderableList> optionListDict = new Dictionary<string, ReorderableList>();//存储每个DialoguePiece的option列表

//     [MenuItem("qbdl/Dialogue Editor")]

//     public static void Init()
//     {
//         DialogueEditor editorWindow = GetWindow<DialogueEditor>("My Dialogue Editor");
//         editorWindow.autoRepaintOnSceneChange = true;//使得该editor窗口持续刷新
//     }

//     public static void InitWindow(DialogueData_SO data)
//     {
//         DialogueEditor editorWindow = GetWindow<DialogueEditor>("My Dialogue Editor");
//         editorWindow.currentData = data;
//     }

//     [OnOpenAsset]//双击点击打开(拦截资源的双击事件)
//     public static bool OpenAsset(int instanceID, int line)
//     {
//         DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;//如果写在前面（DialogueData_SO）这样形式会报错

//         if (data != null)//点击的是DialogueData_SO类型的资源且不为空
//         {
//             DialogueEditor.InitWindow(data);//初始化编辑器窗口并传入数据
//             return true; //返回true表示该编辑器处理了打开事件
//         }
//         return false;
//     }

//     //选择改变时调用一次
//     void OnSelectionChange()
//     {
//         var newData = Selection.activeObject as DialogueData_SO;//获取当前选中的对象并转换为DialogueData_SO类型
//         if (newData != null)
//         {
//             currentData = newData;//更新当前数据
//             SetupReorderableList();//设置ReorderableList
//         }
//         else//若为空，则表示不是该类型数据
//         {
//             currentData = null; //如果选中的不是DialogueData_SO类型，则将currentData设置为null
//             pieceList = null; //清除pieceList
//         }
//         Repaint(); //重绘窗口
//     }

//     //Editor中的类Update函数
//     void OnGUI()
//     {
//         if (currentData != null)
//         {
//             EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);//显示(画出）当前数据的名称
//             GUILayout.Space(10);//添加间隔

//             scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));//开始一个可滚动的视图

//             if (pieceList == null)
//                 SetupReorderableList();//如果pieceList为空，设置它
//             pieceList.DoLayoutList();//绘制对话片段列表

//             GUILayout.EndScrollView();//结束滚动视图
//         }
//         else
//         {
//             if (GUILayout.Button("Create New Dialogue"))
//             {
//                 //如果没有选中数据，显示创建新对话按钮
//                 string dataPath = "Assets/Game Data/Dialogue Data/";
//                 if (!Directory.Exists(dataPath))
//                     Directory.CreateDirectory(dataPath); //如果目录不存在，创建它

//                 DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();
//                 AssetDatabase.CreateAsset(newData, dataPath + "/" + "New Dialogue.asset");//创建新的DialogueData_SO文件
//                 currentData = newData;
//             }
//             GUILayout.Label("No Dialogue Data Selected", EditorStyles.boldLabel);//如果没有数据，显示提示信息
//         }
//     }

//     //在窗口关闭时调用
//     void OnDisable()
//     {
//         optionListDict.Clear(); //清除字典中的所有元素
//     }

//     private void SetupReorderableList()
//     {
//         pieceList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);

//         pieceList.drawHeaderCallback += OnDrawPieceHeader;
//         pieceList.drawElementCallback += OnDrawPieceListElement;
//         pieceList.elementHeightCallback += OnHeightChanged;
//     }

//     #region ReorderableList 一层 DialoguePiece 绘画
//     //绘制标题栏内容
//     private void OnDrawPieceHeader(Rect rect)
//     {
//         GUI.Label(rect, "Dialogue Pieces");
//     }

//     //绘制对话piece列表中的每个元素
//     private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
//     {
//         //编辑时使数据被标记为Dirty，以便系统保存更改
//         EditorUtility.SetDirty(currentData);

//         GUIStyle textStyle = new GUIStyle("TextField");//针对TextField的样式

//         if (index < currentData.dialoguePieces.Count)
//         {
//             //变量初始化
//             var currentPiece = currentData.dialoguePieces[index];
//             var tempRect = rect;
//             tempRect.height = EditorGUIUtility.singleLineHeight;

//             currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, currentPiece.ID);//画上折叠按钮
//             if (currentPiece.canExpand)
//             {
//                 tempRect.width = 30;
//                 tempRect.y += tempRect.height + 5;
//                 EditorGUI.LabelField(tempRect, "ID");//画上 "ID"
//                 tempRect.x += tempRect.width;

//                 tempRect.width = 100;
//                 currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID); //画上 ID 具体值(保持变量与窗口数据保持连接)
//                 tempRect.x += tempRect.width + 10;

//                 EditorGUI.LabelField(tempRect, "Quest");//画上 "Quest"
//                 tempRect.x += 45;

//                 currentPiece.quest = (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);//画上 Quest 具体值    


//                 //下一行
//                 tempRect.y += EditorGUIUtility.singleLineHeight + 5;
//                 tempRect.x = rect.x;

//                 tempRect.height = 60;
//                 tempRect.width = tempRect.height;
//                 currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);//画上图片

//                 tempRect.x += tempRect.width + 5;
//                 tempRect.width = rect.width - tempRect.x;
//                 textStyle.wordWrap = true; //开启自动换行
//                 currentPiece.text = (string)EditorGUI.TextField(tempRect, currentPiece.text, textStyle); // 画上文本框

//                 tempRect.y += tempRect.height + 5;
//                 tempRect.x = rect.x;
//                 tempRect.width = rect.width;

//                 string optionListKey = currentPiece.ID + currentPiece.text;//使用ID和文本作为key
//                 if (optionListKey != null)
//                 {
//                     if (!optionListDict.ContainsKey(optionListKey))
//                     {
//                         //在字典里创建对应的映射
//                         var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);

//                         //类似SetupReorderableList里的创建操作（多层）
//                         optionList.drawHeaderCallback += OnDrawOptionHeader;
//                         optionList.drawElementCallback += (optionRect, optionIndex, optionActive, optionFocused) =>
//                         {
//                             OnDrawOptionElement(currentPiece, optionRect, optionIndex, optionActive, optionFocused);
//                         };

//                         optionListDict[optionListKey] = optionList;
//                     }

//                     optionListDict[optionListKey].DoList(tempRect);//绘制对应的option列表
//                 }
//             }
//         }
//     }

//     //改变piece片段列表中每个元素的高度
//     private float OnHeightChanged(int index)
//     {
//         return GetPieceHeight(currentData.dialoguePieces[index]);
//     }
//     float GetPieceHeight(DialoguePiece piece)
//     {
//         var height = EditorGUIUtility.singleLineHeight;
//         var isExpand = piece.canExpand;

//         if (isExpand)
//         {
//             //Dialogue暂定高度
//             height += EditorGUIUtility.singleLineHeight * 9;
//             //根据option数量调整高度
//             var options = piece.options;
//             if (options.Count > 1)
//                 height += EditorGUIUtility.singleLineHeight * options.Count;
//         }
//         return height;
//     }
//     #endregion


//     #region ReorderableList 二层 DialogueOption 绘画
//     //绘制每个DialoguePiece下面的option
//     private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive, bool optionFocused)
//     {
//         var currentOption = currentPiece.options[optionIndex];
//         var tempRect = optionRect;

//         tempRect.width = optionRect.width * 0.5f;
//         currentOption.text = EditorGUI.TextField(tempRect, currentOption.text);//画上选项文本

//         tempRect.x += tempRect.width + 5;
//         tempRect.width = optionRect.width * 0.3f;
//         currentOption.nextDialogueID = EditorGUI.TextField(tempRect, currentOption.nextDialogueID); //画上目标ID

//         tempRect.x += tempRect.width + 5;
//         tempRect.width = optionRect.width * 0.2f;
//         currentOption.takeQuest = EditorGUI.Toggle(tempRect, currentOption.takeQuest);//画上接取任务的复选框
//     }

//     //绘制option标题栏内容
//     private void OnDrawOptionHeader(Rect rect)
//     {
//         GUI.Label(rect, "Option Text");

//         rect.x += rect.width * 0.5f + 10;
//         GUI.Label(rect, "Next Dialogue ID");

//         rect.x += rect.width * 0.3f;
//         GUI.Label(rect, "Take Quest");
//     }

//     #endregion
// }