using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text levelText; // 角色等级文本
    Image healthSlider; // 血量条滑动条
    Image expSlider; // 经验条滑动条

    void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>(); // 获取角色等级文本
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>(); // 获取血量条滑动条
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>(); // 获取经验条滑动条
    }

    void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}
