using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements")]
    public Image icon; // 对话框图标
    public Text dialogueText; // 对话文本
    public Button nextButton; // 下一步按钮
    public GameObject dialoguePanel; // 对话panel

    [Header("Data")]
    public DialogueData_SO currentDialogue; // 当前对话数据
    int currentIndex = 0;// 当前对话索引


    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue); // 添加下一步按钮的点击事件
    }

    //继续对话
    void ContinueDialogue()
    {
        if (currentIndex < currentDialogue.dialoguePieces.Count)
            UpdateMainDialogue(currentDialogue.dialoguePieces[currentIndex]); // 更新对话内容
        else
            dialoguePanel.SetActive(false); // 如果没有更多对话，隐藏对话面板
    }

    //Dialogue的 UI部分 更新数据
    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentDialogue = data;
        currentIndex = 0; // 重置对话索引
    }

    //Dialogue的 UI部分(初始进入部分) 更新数据
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());//强制刷新布局防止 对话的Next按钮被Text文本框遮挡以至于无法被点按
        dialoguePanel.SetActive(true); // 显示对话面板

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image; // 设置对话框图标
        }
        else
            icon.enabled = false;

        dialogueText.text = ""; // 清空文本
        // dialogueText.text = piece.text; // 设置对话文本
        dialogueText.DOText(piece.text, 1.0f); // 使用DOTween实现文本逐字显示

        //是否显示next button
        if (piece.options.Count == 0 && currentDialogue.dialoguePieces.Count > 0)
        {
            nextButton.gameObject.SetActive(true); // 显示下一步按钮
            currentIndex++;
        }
        else
            nextButton.gameObject.SetActive(false); // 隐藏下一步按钮
    }
}
