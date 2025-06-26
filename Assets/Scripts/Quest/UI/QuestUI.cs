using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//整个任务面板UI
public class QuestUI : Singleton<QuestUI>
{
    [Header("UI Elements")]
    public GameObject questPanel; // 任务面板
    public ItemTooltip tooltip;// 物品提示
    bool isOpen;// 任务面板是否打开

    [Header("Quest Name")]
    public RectTransform questListTransform; // 任务列表的父级RectTransform
    public QuestNameButton questNameButtonPrefab;// 任务名称按钮prefab

    [Header("Text Content")]
    public Text questContentText; // 任务内容文本

    [Header("Requirement")]
    public RectTransform requireTransform; // 任务需求的父级RectTransform
    public QuestRequirement questRequirementPrefab; // 任务需求prefab

    [Header("Reward Panel")]
    public RectTransform rewardTransform; // 奖励的父级RectTransform
    public ItemUI rewardItemPrefab; // 奖励格子的UI prefab

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContentText.text = "";

            //显示面板内容
            SetupQuestList();

            if (!isOpen)
                tooltip.gameObject.SetActive(false); // 如果关闭面板，则隐藏tooltip
        }
    }

    //设置 左侧任务列表
    public void SetupQuestList()
    {
        //清除之前 左侧信息
        foreach (Transform item in questListTransform)
            Destroy(item.gameObject);
        //清除之前 右侧信息
        foreach (Transform item in requireTransform)
            Destroy(item.gameObject);
        foreach (Transform item in rewardTransform)
            Destroy(item.gameObject);

        //使用QuestManager里的task来生成任务内容
        foreach (var task in QuestManager.Instance.tasks)
        {
            var newTask = Instantiate(questNameButtonPrefab, questListTransform);// 实例化 任务名称button
            newTask.SetupNameButton(task.questData); // 设置 任务名称button
        }
    }

    //设置 某个任务对应的右侧任务需求列表
    public void SetupRequireList(QuestData_SO questData)
    {
        //清除之前 右侧信息
        foreach (Transform item in requireTransform)
            Destroy(item.gameObject);

        //使用 某个任务 里的questRequires来生成任务需求内容
        questContentText.text = questData.description; // 设置任务描述文本
        foreach (var require in questData.questRequires)
        {
            var q = Instantiate(questRequirementPrefab, requireTransform); // 实例化 任务Requirement
            q.SetupRequirement(require.name, require.requireAmount, require.currentAmount); // 设置 任务Requirement
        }
    }

    //设置 某个任务对应的单种奖励
    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardItemPrefab, rewardTransform); // 实例化 奖励格子UI
        item.SetupItemUI(itemData, amount); // 设置 奖励格子UI
    }

    //设置 某个任务对应的奖励列表
    public void SetupRewardList(QuestData_SO questData)
    {
        //清除之前 奖励信息
        foreach (Transform item in rewardTransform)
            Destroy(item.gameObject);

        //使用 某个任务 里的rewards来生成奖励内容
        foreach (var item in questData.rewards)
            SetupRewardItem(item.itemData, item.itemAmount); // 设置 奖励格子UI
    }

}
