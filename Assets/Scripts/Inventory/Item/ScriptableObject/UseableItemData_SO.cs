using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Useable Item", menuName = "Inventory/Useable Item Data")]
public class UseableItemData_SO : ScriptableObject
{
    //TODO:还可以加入攻击力，敏捷，攻击范围等
    public int healthPoint; //使用物品回复的生命值
    public int defencePoint; //使用物品增加的防御点
}
