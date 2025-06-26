using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    //该类 功能类似于模板，防止直接更改到源数据
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData; // 任务数据
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsCompleted { get { return questData.isCompleted; } set { questData.isCompleted = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
    }

    public List<QuestTask> tasks = new List<QuestTask>(); // 任务列表

    //更新任务进度——敌人死亡/拾取物品/初始接任务 时更新
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in tasks)
        {
            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount; // 更新当前数量
            task.questData.CheckQuestProgress(); // 检查任务是否完成
        }
    }


    //判断是否有重复任务
    public bool HaveQuest(QuestData_SO data)
    {
        if (data != null)
            return tasks.Any(q => q.questData.questName == data.questName);
        else
            return false;
    }

    //找到QuestData_SO对应的QuestTask
    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(q => q.questData.questName == data.questName);
    }

}
