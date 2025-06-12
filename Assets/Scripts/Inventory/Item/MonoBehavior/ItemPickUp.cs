using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData; // 每个ItemPickUp都挂载在一个物体身上（每个物体都有对应的ItemData_SO）
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI(); //背包数据库修改了——>对应刷新背包UI

            //装备武器
            // GameManager.Instance.playerStats.EquipWeapon(itemData);

            Destroy(gameObject); //销毁物品
        }
    }
}
