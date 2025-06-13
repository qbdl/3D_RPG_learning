using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum SlotType { BAG, WEAPON, ARMOR, ACTION }
//背包栏格子，武器栏格子，盔甲栏格子，动作栏格子

public class SlotHolder : MonoBehaviour, IPointerClickHandler
{
    public SlotType slotType; //格子类型
    public ItemUI itemUI; // slotHolder(格子)上对应的ItemUI

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
            UseItem(); //双击使用物品
    }
    public void UseItem()
    {
        if (itemUI.GetItemData().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].itemAmount > 0)//防止连续点击时小于0
        {
            //使用物品
            int healthPoint = itemUI.GetItemData().useableItemData.healthPoint; //获取使用物品回复的生命值
            int defencePoint = itemUI.GetItemData().useableItemData.defencePoint; //获取使用物品增加的防御点
            GameManager.Instance.playerStats.ApplyUseableEffect(healthPoint, defencePoint); //应用使用物品的效果

            itemUI.Bag.items[itemUI.Index].itemAmount--; //减少物品数量
        }
        UpdateItem(); //更新物品UI显示
    }

    // 对该SlotHolder对应的ItemUI进行更新
    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData; //设置其中Bag指向对应的背包数据库
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData; //设置其中Bag指向对应的装备数据库
                //装备武器 切换武器
                if (itemUI.GetItemData() != null)
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.GetItemData());
                else
                    GameManager.Instance.playerStats.UnEquipWeapon();//耐久度问题(降为0则卸下武器)
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData; //设置其中Bag指向对应的装备数据库
                //装备盔甲 切换盔甲
                if (itemUI.GetItemData() != null)
                    GameManager.Instance.playerStats.ChangeArmor(itemUI.GetItemData());
                else
                    GameManager.Instance.playerStats.UnEquipArmor(); //耐久度问题(降为0则卸下盔甲)
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData; //设置其中Bag指向对应的快捷栏数据库
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index]; //从这一格的UI获取到对应的物品数据(InventoryItem)
        itemUI.SetupItemUI(item.itemData, item.itemAmount); //使用从数据库里获得的物品数据(InventoryItem) 来 设置这一格的UI显示的物品图标和数量
    }
}
