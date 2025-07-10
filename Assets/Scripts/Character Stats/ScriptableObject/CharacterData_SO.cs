using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;// 击杀获得的经验点数


    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;

    [Header("Movement")]
    public float currentMoveSpeed;
    public float maxMoveSpeed;
    public float baseMoveSpeed;

    [Header("Growth Rate")]
    [Tooltip("经验需求每级增加比例，例如 0.06 = +6%")]
    [Range(0f, 1f)] public float expGrowRate;
    [Tooltip("最大生命每级增加比例，例如 0.12 = +12%")]
    [Range(0f, 1f)] public float hpGrowRate;
    [Tooltip("基础防御每级增加比例，例如 0.08 = +8%")]
    [Range(0f, 1f)] public float defGrowRate;
    [Tooltip("移动速度每级增加比例，例如 0.04 = +4%")]
    [Range(0f, 1f)] public float speedGrowRate;

    public void UpdateExp(int point)
    {
        if (currentLevel >= maxLevel) return;

        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        if (currentLevel == 0) return;  // 确保人物死后不再意外变为0级

        //所有提升的数据方法
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel); // 确保当前等级不超过最大等级

        baseExp = Mathf.CeilToInt(baseExp * (1f + expGrowRate)); // 下一阶段升级需要的经验值
        maxHealth = Mathf.CeilToInt(maxHealth * (1f + hpGrowRate)); // 最大生命值提升
        currentHealth = maxHealth;
        baseDefence = Mathf.CeilToInt(baseDefence * (1f + defGrowRate)); // 基础防御值提升 
        currentDefence = baseDefence;

        float newSpeed = baseMoveSpeed * (1.0f + speedGrowRate * (currentLevel - 1)); // 新的移动速度
        currentMoveSpeed = Mathf.Min(maxMoveSpeed, newSpeed);

        // TODO: 如果有攻击力成长，可在此处补充

        Debug.Log($"Level Up! New Level: {currentLevel}, Max Health: {maxHealth}, Base Exp: {baseExp}");
    }

}
