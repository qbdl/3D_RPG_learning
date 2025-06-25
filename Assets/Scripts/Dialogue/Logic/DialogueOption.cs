using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一条对话下面对应的选项
[System.Serializable]
public class DialogueOption
{
    public string text;//选项内容
    public string nextDialogueID;//下一条对话的ID
    public bool takeQuest;//是否是任务选项
}
