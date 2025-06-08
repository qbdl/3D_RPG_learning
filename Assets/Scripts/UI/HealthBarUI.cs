using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;// 血量条预设体
    public Transform barPoint;// 原始血量条位置
    public bool alwaysVisible;// 是否始终可见血量条
    public float visibleTime;// 可见时间（如果alwaysVisible为false则使用此时间控制血量条的显示时长）
    private float timeLeft;// 剩余可见时间

    Image healthSlider;// 滑动条
    Transform UIbar;//血量条位置
    Transform cam;// 摄像机位置（使用摄像机位置来使得血量条始终面向玩家）

    CharacterStats currentStats;// 当前角色的数据

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        cam = Camera.main.transform; // 获取主摄像机

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform; // 在屏幕空间画布上实例化血量条
                healthSlider = UIbar.GetChild(0).GetComponent<Image>(); // 获取血量条的滑动条组件
                UIbar.gameObject.SetActive(alwaysVisible); // 根据alwaysVisible设置血量条这个UIbar在场景中初始是否可见

            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0) Destroy(UIbar.gameObject);// 如果血量为0则销毁血量条

        UIbar.gameObject.SetActive(true);//被攻击时一定显示血量条
        timeLeft = visibleTime; // 重置可见时间

        float sliderPercent = (float)currentHealth / maxHealth; // 计算血量条的百分比
        healthSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()//在上一帧渲染之后才执行这条指令
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position; // 设置血量条位置
            UIbar.forward = -cam.forward; // 使血量条始终面向摄像机

            if (timeLeft <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }

}
