using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originSlotHolder; // 原始的SlotHolder
        public RectTransform originalParent; // 原始的parent
    }

    [Header("Inventory Data")]
    public InventoryData_SO templateInventoryData; // 背包的模板数据
    public InventoryData_SO inventoryData; // 背包的数据库
    public InventoryData_SO templateEquipmentData; // 装备的模板数据
    public InventoryData_SO equipmentData; // 装备的数据库
    public InventoryData_SO templateActionData; // 快捷栏的模板数据
    public InventoryData_SO actionData;// 快捷栏的数据库

    [Header("Containers")]
    public ContainerUI inventoryUI; // 背包对应的UI
    public ContainerUI equipmentUI; // 装备对应的UI
    public ContainerUI actionUI; // 快捷栏对应的UI

    [Header("Drag Canvas")]
    public Canvas dragCanvas; // 拖拽物品时使用的canvas
    public DragData currentDrag;// 当前拖拽的DragData

    [Header("UI Panel")]
    public GameObject inventoryPanel; // 背包栏UI面板
    public GameObject statsPanel; // 装备栏(人物栏)UI面板
    bool isOpen = false;// 背包栏与装备栏是否打开

    [Header("Stats Text")]
    public Text healthText; // 生命值文本
    public Text attackText; // 攻击力文本
    public Text defenceText; // 防御值文本

    [Header("Tooltip")]
    public ItemTooltip tooltip; // 物品提示UI

    protected override void Awake()
    {
        base.Awake();

        if (templateInventoryData != null)
            inventoryData = Instantiate(templateInventoryData); // 克隆背包模板数据
        if (templateEquipmentData != null)
            equipmentData = Instantiate(templateEquipmentData); // 克隆装备模板数据
        if (templateActionData != null)
            actionData = Instantiate(templateActionData); // 克隆快捷栏模板数据
    }

    void Start()
    {
        LoadData(); // 加载背包等数据

        inventoryUI.RefreshUI(); // 初始化背包UI
        equipmentUI.RefreshUI(); // 初始化装备UI
        actionUI.RefreshUI(); // 初始化快捷栏UI
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // 按下B键打开或关闭背包栏
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen); // 切换背包栏UI面板的显示状态
            statsPanel.SetActive(isOpen); // 切换装备栏UI面板的显示状态
        }

        // 更新角色人物栏属性文本
        UpdateStatsText(
            GameManager.Instance.playerStats.CurrentHealth,
            GameManager.Instance.playerStats.attackData.minDamage,
            GameManager.Instance.playerStats.attackData.maxDamage,
            GameManager.Instance.playerStats.CurrentDefence
        );
    }

    //保存背包等数据
    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
    }

    //加载背包等数据
    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
    }

    // 更新角色人物栏属性文本
    public void UpdateStatsText(int health, int attack_min, int attack_max, int defence)
    {
        // 更新生命值文本
        healthText.text = health.ToString();
        // 更新攻击力文本
        attackText.text = attack_min + " - " + attack_max;
        // 更新防御值文本
        defenceText.text = defence.ToString();
    }


    #region 检查拖拽物品是否在每一个Slot范围内
    //背包栏
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)inventoryUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
                                                                                  //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }
    //装备栏
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)equipmentUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
                                                                                  //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }
    //快捷栏
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = (RectTransform)actionUI.slotHolders[i].transform;//获取每个SlotHolder的RectTransform
                                                                               //检查鼠标位置是否在当前SlotHolder的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true; //如果在范围内，返回true
        }
        return false;
    }

    #endregion

}
