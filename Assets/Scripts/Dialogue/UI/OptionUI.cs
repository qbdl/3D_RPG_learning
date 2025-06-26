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
    private bool takeQuest;// 是否接取任务

    void Awake()
    {
        optionButton = GetComponent<Button>(); // 获取按钮组件
        optionButton.onClick.AddListener(OnOptionClicked); // 添加option button的点击事件
    }

    //option被点击时触发
    void OnOptionClicked()
    {
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask { questData = Instantiate(currentPiece.quest) }; // 使用当前quest创建一个任务实例
            if (takeQuest)
            {
                //添加到任务列表

                //--判断是否有重复任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断是否完成给予奖励
                }
                else
                {
                    //没有任务，接受新任务
                    QuestManager.Instance.tasks.Add(newTask); // 添加任务到任务列表
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true; // 设置任务状态为已开始(找到QuestManaager里的任务而不是临时变量)

                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem); // 检查背包中是否有任务所需的物品
                    }
                }
            }
        }

        if (nextPieceID == "")
            DialogueUI.Instance.dialoguePanel.SetActive(false); // 如果没有下一条对话，隐藏对话面板
        else
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentDialogue.dialogueIndex[nextPieceID]); // 更新option的下一条对话
    }


    //更新option的UI
    public void UpdateOptionUI(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece; // 传入当前对话Piece
        optionText.text = option.text; // 设置选项文本
        nextPieceID = option.nextDialogueID; // 下一条对话的ID
        takeQuest = option.takeQuest; // 是否接取任务状态
    }
}
