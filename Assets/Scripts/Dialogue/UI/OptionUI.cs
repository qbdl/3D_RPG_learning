using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText; // 选项文本
    private Button optionButton; // 选项按钮
    private DialoguePiece currentPiece; // 当前对话Piece
    private string nextPieceID;// 下一条对话的ID

    void Awake()
    {
        optionButton = GetComponent<Button>(); // 获取按钮组件
        optionButton.onClick.AddListener(OnOptionClicked); // 添加option button的点击事件
    }

    //option被点击时触发
    void OnOptionClicked()
    {
        if (nextPieceID == "")
            DialogueUI.Instance.dialoguePanel.SetActive(false); // 如果没有下一条对话，隐藏对话面板
        else
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentDialogue.dialogueIndex[nextPieceID]); // 更新option的下一条对话
    }


    //更新option的UI
    public void UpdateOptionUI(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece; // 保存当前对话Piece
        optionText.text = option.text; // 设置选项文本
        nextPieceID = option.nextDialogueID; // 保存下一条对话的ID
    }
}
