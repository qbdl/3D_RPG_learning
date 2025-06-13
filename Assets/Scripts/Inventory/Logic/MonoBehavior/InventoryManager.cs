using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originSlotHolder; // 原始的SlotHolder
        public RectTransform originalParent; // 原始的parent
    }

    //TODO:最后添加模板用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData; // 背包的数据库
    public InventoryData_SO equipmentData; // 装备的数据库
    public InventoryData_SO actionData;// 快捷栏的数据库

    [Header("Containers")]
    public ContainerUI inventoryUI; // 背包对应的UI
    public ContainerUI equipmentUI; // 装备对应的UI
    public ContainerUI actionUI; // 快捷栏对应的UI

    [Header("Drag Canvas")]
    public Canvas dragCanvas; // 拖拽物品时使用的canvas
    public DragData currentDrag;// 当前拖拽的DragData


    void Start()
    {
        inventoryUI.RefreshUI(); // 初始化背包UI
        equipmentUI.RefreshUI(); // 初始化装备UI
        actionUI.RefreshUI(); // 初始化快捷栏UI
    }

    #region 检查拖拽物品是否在每一个Slot范围内
    //背包栏
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)inventoryUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
            //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }
    //装备栏
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)equipmentUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
            //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }
    //快捷栏
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)actionUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
            //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }

    #endregion

}
