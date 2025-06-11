using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]

public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>(); // 背包物品列表

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        //note:这里的数组存放中间可以是空着的，不一定是连续的有物体

        //可堆叠{存在-合并/不存在-新增}
        //不可堆叠{新增}

        //1、合并-{可堆叠_存在}
        bool found = false;
        if (newItemData.itemStackable)
        {
            // 遍历现有物品列表，查找是否有相同的物品
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemData == newItemData) // 找到相同的物品
                {
                    items[i].itemAmount += amount; // 增加数量
                    found = true;
                    break; // 找到后退出循环
                }
            }
        }
        //2、新增
        if (!found)//1没有完成（那找一个空位放进去即可）
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemData == null) // 找到空位
                {
                    items[i].itemData = newItemData;
                    items[i].itemAmount = amount;
                    break;
                }
            }
        }
    }
}


[System.Serializable]
//在原来ItemData_SO基础上加上整体的数量（而非单次物品的数量）
public class InventoryItem
{
    public ItemData_SO itemData; // 物品数据
    public int itemAmount; // 物品数量
}

