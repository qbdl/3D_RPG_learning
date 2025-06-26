using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText; // 任务名称文本
    public QuestData_SO currentQuestData; // 对应的任务数据

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    // 将该button对应的任务内容 显示在右侧
    void UpdateQuestContent()
    {
        QuestUI.Instance.SetupRequireList(currentQuestData);// 设置任务Requirement列表
        QuestUI.Instance.SetupRewardList(currentQuestData);// 设置任务Reward列表
    }

    // 设置任务名称button
    public void SetupNameButton(QuestData_SO questData)
    {
        currentQuestData = questData;
        // questNameText.text = questData.isCompleted ? questData.questName + " (已完成)" : questData.questName;
        if (questData.isCompleted || questData.isFinished)
            questNameText.text = questData.questName + " (已完成)";
        else if (questData.isStarted)
            questNameText.text = questData.questName + " (进行中)";
        else
            questNameText.text = questData.questName;
    }

}
