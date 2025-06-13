using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Useable, Weapon, Armor }
//可使用物品（药），武器，盔甲

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/It")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType; //物品类型
    public string itemName; //物品名称
    public Sprite itemIcon; //物品图标
    public int itemAmount; //物品数量
    [TextArea]
    public string itemDescription = ""; //物品描述
    public bool itemStackable; //物品是否可堆叠

    [Header("Useable Item")]
    public UseableItemData_SO useableItemData; //可使用物品数据

    [Header("Weapon & Armor")]
    public GameObject weaponPrefab; //武器prefab
    public AttackData_SO weaponData; //武器攻击数据

    public GameObject armorPrefab; //盔甲prefab
    public CharacterData_SO armorData; //盔甲数据


}
