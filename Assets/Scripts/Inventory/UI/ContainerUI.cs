using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders; //slotHolder数组

    public void RefreshUI()//更新UI使其与数据库里的数据对应
    {
        for (int i = 0; i < slotHolders.Length; i++)
        {
            slotHolders[i].itemUI.Index = i; //初始化设置每个slotHolder的itemUI的Index
            slotHolders[i].UpdateItem(); //更新每个slotHolder的UI
        }
    }
}
