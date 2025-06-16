using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    RectTransform rectTransform; // 当前拖拽panel的RectTransform
    Canvas canvas; // 当前拖拽panel所在的Canvas

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();// 获取当前拖拽panel的RectTransform
        canvas = InventoryManager.Instance.GetComponent<Canvas>(); // 获取当前拖拽panel所在的Canvas
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // 更新位置
        // Debug.Log(rectTransform.GetSiblingIndex()); // 输出当前拖拽panel的SiblingIndex
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 设置当前拖拽panel的SiblingIndex为2，确保无论它是人物栏还是背包栏都会在另一个的上方显示（取决于实际UI设计）
        rectTransform.SetSiblingIndex(2);
    }
}
