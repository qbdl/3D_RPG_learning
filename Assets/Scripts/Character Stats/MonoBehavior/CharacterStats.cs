using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;// 攻击时更新血量条事件

    public CharacterData_SO templateData; // 角色模板数据脚本化对象
    public CharacterData_SO characterData; // 角色数据脚本化对象
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData); // 克隆模板数据
        }
        // else
        // {
        //     Debug.LogError("Template data is not assigned in CharacterStats.");
        // }

        // if (attackData == null)
        // {
        //     Debug.LogError("Attack data is not assigned in CharacterStats.");
        // }
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit"); //触发暴击动画
        }
        //update血量UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);


        //TODO:人物属性升级
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        //update血量UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }


    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        //是否暴击
        // Debug.Log($"Core Damage: {coreDamage}, Critical: {isCritical}");
        return (int)(coreDamage * (isCritical ? attackData.criticalMultiplier : 1));
    }

    #endregion
}
