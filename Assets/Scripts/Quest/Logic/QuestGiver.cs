using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    DialogueController controller; // QuestGiver挂载的人物身上的 DialogueController
    QuestData_SO currentQuest; // 当前任务

    public DialogueData_SO startDialogue; // 开始任务时的对话
    public DialogueData_SO progressDialogue; // 任务进行中的对话
    public DialogueData_SO completeDialogue; // 任务完成时的对话
    public DialogueData_SO finishDialogue; // 任务完成终止后的对话

    #region 获得任务状态
    public bool IsStarted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))//当前任务是否被接取
                return QuestManager.Instance.GetTask(currentQuest).IsStarted;
            else
                return false;
        }
    }
    public bool IsCompleted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))//当前任务是否被接取
                return QuestManager.Instance.GetTask(currentQuest).IsCompleted;
            else
                return false;
        }
    }
    public bool IsFinished
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))//当前任务是否被接取
                return QuestManager.Instance.GetTask(currentQuest).IsFinished;
            else
                return false;
        }
    }
    #endregion


    void Awake()
    {
        controller = GetComponent<DialogueController>();
    }

    void Start()
    {
        controller.currentDialogueData = startDialogue; // 初始化对话数据为开始任务时的对话
        currentQuest = controller.currentDialogueData.GetQuest(); // 获取当前对话数据中的最早任务
    }

    void Update()
    {
        if (IsStarted)
            controller.currentDialogueData = IsCompleted ? completeDialogue : progressDialogue; // 如果任务已完成，更新对话数据为完成任务时的对话，否则更新为进行中的对话
        if (IsFinished)
            controller.currentDialogueData = finishDialogue; // 如果任务已完成终止，更新对话数据为完成终止后的对话
    }
}
