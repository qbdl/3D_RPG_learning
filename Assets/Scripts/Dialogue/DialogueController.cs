using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentDialogueData; // 当前对话数据
    bool canTalk = false;//是否可以对话

    void Update()
    {
        if (canTalk && Input.GetMouseButtonDown(1)) // 按下鼠标右键触发对话
        {
            OpenDialogue();
        }
    }

    //触发npc对话
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("触发对话区域");
        if (other.CompareTag("Player") && currentDialogueData != null)
            canTalk = true; // 玩家进入触发器区域，可以对话
    }

    //离开npc对话区域
    void OnTriggerExit(Collider other)
    {
        // Debug.Log("离开对话区域");
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false); // 关闭对话面板
            canTalk = false; // 玩家离开触发器区域，不能对话
        }
    }

    void OpenDialogue()
    {
        //打开UI面板 , 传输对话内容

        DialogueUI.Instance.UpdateDialogueData(currentDialogueData); // 更新对话数据
        DialogueUI.Instance.UpdateMainDialogue(currentDialogueData.dialoguePieces[0]); // 更新主对话内容
    }
}
