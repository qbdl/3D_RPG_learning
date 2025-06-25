using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
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
}
