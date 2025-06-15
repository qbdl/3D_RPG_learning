using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用数字键来使用快捷栏的物品
public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey; // 触发按键
    private SlotHolder currentSlotHolder; // 当前快捷栏格子

    void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItemData())
            currentSlotHolder.UseItem();
    }
}
