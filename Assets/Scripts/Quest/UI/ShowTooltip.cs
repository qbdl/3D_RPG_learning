using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemUI currentItemUI; // 当前reward格子里的 物品UI

    void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    // 鼠标进入时显示提示
    public void OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(true); // 确保tooltip面板是激活的
        QuestUI.Instance.tooltip.SetupTooltip(currentItemUI.currentItemData); // 设置tooltip内容
    }

    // 鼠标离开时隐藏提示
    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(false); // 隐藏tooltip面板
    }
}
