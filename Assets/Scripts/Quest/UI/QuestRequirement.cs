using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text requireName; // 任务需求名称
    private Text progressNumber;// 任务需求进度数字

    void Awake()
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }
}
