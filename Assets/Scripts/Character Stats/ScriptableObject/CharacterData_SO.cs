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
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        if (currentLevel == 0) return;  // 确保人物死后不再意外变为0级

        //所有提升的数据方法
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel); // 确保当前等级不超过最大等级
        baseExp += (int)(baseExp * LevelMultiplier); // 下一阶段升级需要的经验值

        maxHealth = (int)(maxHealth * LevelMultiplier); // 升级后最大生命值提升
        currentHealth = maxHealth; // 升级后当前生命值重置为最大生命值

        //TODO:攻击以及防御值改变

        Debug.Log($"Level Up! New Level: {currentLevel}, Max Health: {maxHealth}, Base Exp: {baseExp}");
    }

}
