using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))] //确保有ItemUI组件
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI; //当前拖拽物品的ItemUI
    SlotHolder currentSlotHolder; //当前拖拽物品的SlotHolder
    SlotHolder targetSlotHolder; //目标SlotHolder

    void Awake()
    {
        //获取当前物品的ItemUI和SlotHolder
        currentItemUI = GetComponent<ItemUI>();
        currentSlotHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //记录原始数据
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();
        InventoryManager.Instance.currentDrag.originSlotHolder = currentSlotHolder; //记录原始的SlotHolder
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent; //记录原始的parent

        //设置当前拖拽物品的parent为dragCanvas,使其在最前面显示(使其不被其他格子遮住)
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //放下物品,交换数据

        //判断鼠标放下时是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position) ||
               InventoryManager.Instance.CheckInEquipmentUI(eventData.position) ||
               InventoryManager.Instance.CheckInActionUI(eventData.position))
            {
                if (eventData.pointerEnter == null)
                {
                    Debug.LogError("My Debug OnEndDrag: pointerEnter is null!");
                    return;
                }

                //检查鼠标指针悬停的对象（pointerEnter.gameObject）是否有 SlotHolder 组件
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                    targetSlotHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                else
                    targetSlotHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();//如果没有，则尝试从该对象的父对象中获取 SlotHolder 组件

                if (targetSlotHolder == null)
                {
                    Debug.LogError("My Debug OnEndDrag: targetSlotHolder is null!");
                    return;
                }

                //当目标槽位与当前槽位不相同时，才进行交换
                if (targetSlotHolder != InventoryManager.Instance.currentDrag.originSlotHolder)
                {
                    switch (targetSlotHolder.slotType)
                    {
                        case SlotType.BAG:
                            // 检查目标槽位是否已有物品
                            var targetItem = targetSlotHolder.itemUI.Bag.items[targetSlotHolder.itemUI.Index];
                            bool canSwap = targetItem.itemData == null || //目标槽位为空
                                           targetItem.itemData.itemType == currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType;//目标槽位与当前槽位物品类型相同
                            if (canSwap)
                                SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                    }
                }
                currentSlotHolder.UpdateItem(); //更新当前SlotHolder的UI
                targetSlotHolder.UpdateItem(); //更新目标SlotHolder的UI
            }
        }

        //将物品的parent设置回原始的parent
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent, true);
        //还原位置（防止其被卡到一些奇怪的位置上）
        RectTransform t = (RectTransform)transform;
        t.offsetMax = -Vector2.one * 5; //这里的5是UI里设置的距离（需要同步修改UI数据，如果要修改的话）
        t.offsetMin = Vector2.one * 5;
    }

    public void SwapItem()
    {
        //如果直接从当前场景进入，由于没有初始化背包会报错（所以要从主菜单进入游戏）
        if (targetSlotHolder.itemUI.Bag == null || currentSlotHolder.itemUI.Bag == null)
        {
            Debug.LogError("My Debug SwapItem: Bag is null in targetSlotHolder or currentSlotHolder!");
            return;
        }

        //改变数据库
        var targetItem = targetSlotHolder.itemUI.Bag.items[targetSlotHolder.itemUI.Index];
        var tempItem = currentSlotHolder.itemUI.Bag.items[currentSlotHolder.itemUI.Index];

        bool isSameItem = (tempItem.itemData == targetItem.itemData);
        if (isSameItem && targetItem.itemData.itemStackable)//叠加
        {
            targetItem.itemAmount += tempItem.itemAmount;
            tempItem.itemData = null;
            tempItem.itemAmount = 0;
        }
        else//交换物品数据
        {
            currentSlotHolder.itemUI.Bag.items[currentSlotHolder.itemUI.Index] = targetItem;
            targetSlotHolder.itemUI.Bag.items[targetSlotHolder.itemUI.Index] = tempItem;
        }
    }
}
