using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    //TODO:最后添加模板用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData; // 背包数据

}
