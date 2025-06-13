using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;//物品图标
    public Text amount = null;//物品数量

    public InventoryData_SO Bag { get; set; }//对应的背包数据库
    public int Index { get; set; } = -1;//对应背包数据库里的索引

    // 设置格子里的物品UI
    public void SetupItemUI(ItemData_SO item, int itemAmount)
    {
        if (item != null)
        {
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();

            icon.gameObject.SetActive(true);//启动icon（含text）这个部分
        }
        else
            icon.gameObject.SetActive(false);//关闭icon（含text）这个部分
    }
}
