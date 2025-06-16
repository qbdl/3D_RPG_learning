using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;// 攻击时更新血量条事件

    public CharacterData_SO templateData; // 角色template数据
    public CharacterData_SO characterData; // 角色数据
    public AttackData_SO templateAttackData; // 攻击template数据
    public AttackData_SO attackData; // 攻击数据
    private RuntimeAnimatorController baseAnimator; // 基础无装备时动画（用于重置动画）


    [Header("Weapon & Armor")]
    public Transform weaponSlot; // 武器位置（用于对应的装上武器） 
    public Transform armorSlot; // 盔甲位置（用于对应的装上盔甲）
    private int equipmentNum; // 已装备的装备数量

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData); // 克隆模板数据
        attackData = Instantiate(templateAttackData); // 克隆基础攻击数据
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController; // 获取基础无装备时动画
        equipmentNum = 0;// 初始化已装备的装备数量
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
            defender.GetComponent<Animator>().SetTrigger("Hit"); //触发暴击动画

        //update血量UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //人物属性升级
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);// 更新击杀者经验值
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);

        //update血量UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //人物属性升级
        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);// 更新击杀者经验值
    }


    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        // //是否暴击
        // // Debug.Log($"Core Damage: {coreDamage}, Critical: {isCritical}");
        return (int)(coreDamage * (isCritical ? attackData.criticalMultiplier : 1));
    }

    #endregion

    #region Equip Weapon & Armor
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon(); // 卸下当前武器
        EquipWeapon(weapon); // 装备新武器
    }
    public void ChangeArmor(ItemData_SO armor)
    {
        UnEquipArmor(); // 卸下当前盔甲
        EquipArmor(armor); // 装备新盔甲
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        // 将weapon prefab生成到 weaponSlot位置
        if (weapon.weaponPrefab != null && weapon.itemType == ItemType.Weapon)
            Instantiate(weapon.weaponPrefab, weaponSlot);
        else
            Debug.LogError($"EquipWeapon: weapon prefab is null or item type is not Weapon for weapon {weapon.name}");

        //装备武器后更新属性
        attackData.ApplyWeaponData(weapon.weaponData);
        //切换动画
        equipmentNum++;
        // Debug.Log($"EquipWeapon: Current equipment count is {equipmentNum} for weapon {weapon.itemName}");
        if (equipmentNum == 1)
        {
            //换成持盾与剑的动画
            GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
        }
    }
    public void UnEquipWeapon()
    {
        // 销毁武器
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
                Destroy(weaponSlot.transform.GetChild(i).gameObject);

            // 只有在实际卸下武器时才减少 equipmentNum
            equipmentNum = Mathf.Max(0, equipmentNum - 1);
            // Debug.Log($"UnEquipWeapon: Current equipment count is {equipmentNum}");
        }

        //卸下武器后更新属性
        attackData.ApplyWeaponData(templateAttackData); // 重置为基础攻击数据
        //切换动画
        if (equipmentNum == 0)
        {
            //换成空手动画
            GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        }
    }
    public void EquipArmor(ItemData_SO armor)
    {
        // 将armor prefab生成到 armorSlot位置
        if (armor.armorPrefab != null && armor.itemType == ItemType.Armor)
            Instantiate(armor.armorPrefab, armorSlot);
        else
            Debug.LogError($"EquipArmor: armor prefab is null or item type is not Armor for armor {armor.name}");

        // 装备盔甲后更新属性
        characterData.baseDefence = armor.armorData.baseDefence;
        characterData.currentDefence = armor.armorData.currentDefence;
        //切换动画
        equipmentNum++;
        // Debug.Log($"EquipArmor: Current equipment count is {equipmentNum} for armor {armor.itemName}");
        if (equipmentNum == 1)
        {
            //换成持盾与剑的动画
            GetComponent<Animator>().runtimeAnimatorController = armor.armorAnimator;
        }
    }
    public void UnEquipArmor()
    {
        // 销毁盔甲
        if (armorSlot.transform.childCount != 0)
        {
            for (int i = 0; i < armorSlot.transform.childCount; i++)
                Destroy(armorSlot.transform.GetChild(i).gameObject);

            // 只有在实际卸下武器时才减少 equipmentNum
            equipmentNum = Mathf.Max(0, equipmentNum - 1);
            // Debug.Log($"UnEquipArmor: Current equipment count is {equipmentNum}");
        }

        // 卸下盔甲后更新属性
        characterData.baseDefence = templateData.baseDefence; // 重置为模板数据的基础防御
        characterData.currentDefence = templateData.currentDefence; // 重置为模板数据的当前防御
        //切换动画
        if (equipmentNum == 0)
        {
            //换成空手动画
            GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        }
    }
    #endregion

    #region Apply Data Change
    public void ApplyUseableEffect(int healthPoint, int defencePoint)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + healthPoint, 0, MaxHealth);//amount有可能是负数
        CurrentDefence = Mathf.Max(CurrentDefence + defencePoint, 0); //防御不能为负数
    }
    #endregion
}
