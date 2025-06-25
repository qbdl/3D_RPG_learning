using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
