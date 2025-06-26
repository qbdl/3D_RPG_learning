using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name; //要收集东西/杀掉怪 的名字
        public int requireAmount; // 需要的数量
        public int currentAmount; // 当前完成的数量
    }

    public string questName; // 任务名称
    [TextArea]
    public string description; // 任务描述

    public bool isStarted; // 任务是否已开始
    public bool isCompleted; // 任务是否已完成
    public bool isFinished; // 任务是否已结束

    public List<QuestRequire> questRequires = new List<QuestRequire>(); // 任务需求列表
    public List<InventoryItem> rewards = new List<InventoryItem>(); // 任务奖励列表

    //检查 任务是否完成
    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.currentAmount >= r.requireAmount);
        isCompleted = finishRequires.Count() == questRequires.Count;

        if (isCompleted) Debug.Log($"Quest '{questName}' is completed!");
    }

    //给予奖励，扣除对应物品
    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.itemAmount < 0) // 需要上交的物品
            {
                int requireCount = Mathf.Abs(reward.itemAmount); // 需要扣除的数量
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null) // 从背包栏中扣除对应物品
                {
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).itemAmount >= requireCount) // 背包中对应物品的数量足够
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).itemAmount -= requireCount;
                        requireCount = 0; // 已经扣除所需数量
                    }
                    else
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).itemAmount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).itemAmount = 0;
                    }
                }
                if (requireCount > 0) // 如果还有未扣除的数量
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).itemAmount -= requireCount; // 从快捷栏中扣除对应物品
            }
            else // 给予奖励的物品
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.itemAmount); // 将奖励物品添加到背包
        }

        InventoryManager.Instance.inventoryUI.RefreshUI(); // 刷新背包UI
        InventoryManager.Instance.actionUI.RefreshUI(); // 刷新快捷栏UI
    }

    //当前任务中需要 收集/消灭的目标名字列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequires)
            targetNameList.Add(require.name);

        return targetNameList;
    }
}
