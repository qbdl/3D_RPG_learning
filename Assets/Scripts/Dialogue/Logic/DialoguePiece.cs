using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一条语句
[System.Serializable]
public class DialoguePiece
{
    public string ID;
    public Sprite image;//DialoguePiece中可以包含图片
    [TextArea]
    public string text;//语句内容

    public QuestData_SO quest;//这条对话的任务数据

    [HideInInspector]
    public bool canExpand;//是否可以展开（用于扩展编辑器的折叠功能）

    public List<DialogueOption> options = new List<DialogueOption>();//选项列表

}
