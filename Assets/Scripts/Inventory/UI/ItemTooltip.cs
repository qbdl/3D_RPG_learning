using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText; //物品名称文本
    public Text itemInfoText; //物品描述文本

    RectTransform rectTransform; //tooltip的RectTransform（位置）

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); //获取RectTransform组件
    }
    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName; //设置物品名称
        itemInfoText.text = item.itemDescription; //设置物品描述
    }

    void OnEnable()
    {
        UpdatePosition();//启用时更新位置
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());//强制刷新当前布局,防止UI无法快速适配高度
        //TODO:不确定是否是加在这里
    }

    void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition; //获取鼠标位置

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners); //获取RectTransform的四个角的位置

        float width = corners[3].x - corners[0].x; //提示框的宽度
        float height = corners[1].y - corners[0].y; //提示框的高度

        // Debug.Log("MousePos y: " + mousePos.y);
        //屏幕左下角为（0，0），rectTransform.position是方形的中心点（锚点）
        //TODO:可能有误for difference

        // 默认将工具提示显示在鼠标上方
        Vector3 tooltipPos = mousePos + Vector3.up * height * 0.6f;//0.5f还是会与鼠标互相覆盖，导致屏闪

        // 检测是否超出屏幕顶部
        if (mousePos.y + height > Screen.height)
            tooltipPos.y = mousePos.y - height * 0.6f; // 鼠标下方显示（y轴下侧）
        // 检测是否超出屏幕右边界
        if (mousePos.x + width > Screen.width)
            tooltipPos.x = mousePos.x - width * 0.6f; // 鼠标左侧显示（x轴左侧）
        // 检测是否超出屏幕左边界
        if (mousePos.x - width < 0)
            tooltipPos.x = mousePos.x + width * 0.6f; // 鼠标右侧显示（x轴右侧）

        rectTransform.position = tooltipPos;

    }
}
