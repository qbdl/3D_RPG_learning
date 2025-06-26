using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;//物品图标
    public Text amount = null;//物品数量
    public ItemData_SO currentItemData; //当前格子UI对应的物品信息

    public InventoryData_SO Bag { get; set; } //对应的背包数据库
    public int Index { get; set; } = -1; //对应背包数据库里的索引

    // 设置格子里的物品UI
    public void SetupItemUI(ItemData_SO item, int itemAmount)
    {
        //物品数量等于0，则不显示物品图标和数量——用于背包中物品使用
        if (itemAmount == 0)
        {
            Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);//关闭icon（含text）这个部分
            return;
        }

        //物品数量小于0——用于任务面板不显示扣除的物品
        if (itemAmount < 0)
            item = null;

        //物品数量大于0
        if (item != null)
        {
            currentItemData = item;
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();

            icon.gameObject.SetActive(true);//启动icon（含text）这个部分
        }
        else
            icon.gameObject.SetActive(false);//关闭icon（含text）这个部分
    }

    // 获取当前格子UI对应 背包 里面的物品数据
    public ItemData_SO GetItemData()
    {
        return Bag.items[Index].itemData; //返回对应格子里的物品数据
    }
}
