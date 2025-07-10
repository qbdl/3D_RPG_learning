using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;// 攻击时更新血量条事件

    public CharacterData_SO templateData; // 角色template数据
    public CharacterData_SO characterData; // 角色数据
    public AttackData_SO templateAttackData; // 攻击template数据
    public AttackData_SO attackData; // 攻击数据
    private RuntimeAnimatorController baseAnimator; // 基础无装备时动画（用于重置动画）
    private NavMeshAgent agent; // 用于控制移动的 NavMeshAgent


    [Header("Weapon & Armor")]
    public Transform weaponSlot; // 武器位置（用于对应的装上武器） 
    public Transform armorSlot; // 盔甲位置（用于对应的装上盔甲）
    private int equipmentNum; // 已装备的装备数量


    [Header("Regeneration")]
    public float regenInterval;       // 每隔多久回血
    public int regenAmount;            // 每次回血多少点
    private float regenTimer = 0;         // 计时器


    [HideInInspector]
    public bool isCritical;  // 是否暴击
    [HideInInspector]
    public bool isDefend; // 是否盾反

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData); // 克隆模板数据

            agent = GetComponent<NavMeshAgent>(); // 获取 NavMeshAgent
            if (agent != null && GetComponent<PlayerController>() != null)
                agent.speed = characterData.currentMoveSpeed; // 仅为Player设置 NavMeshAgent 的速度
        }

        attackData = Instantiate(templateAttackData); // 克隆基础攻击数据
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController; // 获取基础无装备时动画
        equipmentNum = 0;// 初始化已装备的装备数量
    }

    void Update()
    {
        // 缓慢回血逻辑（仅限玩家）
        if (GetComponent<PlayerController>() != null && CurrentHealth > 0 && CurrentHealth < MaxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                regenTimer = 0f;
                CurrentHealth = Mathf.Min(CurrentHealth + regenAmount, MaxHealth);
                UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth); // 更新血条
            }
        }
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
        //New : 加入内测无敌模式
        if (GetComponent<PlayerController>() != null && GetComponent<PlayerController>().Invincibility)
            return;

        //New : 加入盾反逻辑
        // 判断是否为玩家，并且是否处于盾反状态
        if (defender.GetComponent<PlayerController>() != null && defender.isDefend) //玩家盾反逻辑
        {
            // Debug.Log("Player is defending, applying shield rebound logic.");

            //计算伤害
            int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 1); //盾反至少造成1点伤害
            int damage_rebound = Mathf.Max(Mathf.CeilToInt(damage * 0.5f), 1); // 反弹攻击者伤害的50%
            int damage_take = Mathf.Max(Mathf.CeilToInt(damage * 0.35f), 1); // 受到攻击者伤害的35%

            //伤害->生命
            attacker.CurrentHealth = Mathf.Max(attacker.CurrentHealth - damage_rebound, 0);
            CurrentHealth = Mathf.Max(CurrentHealth - damage_take, 0);
            //动画
            attacker.GetComponent<Animator>().SetTrigger("Hit");// 触发攻击者的 被盾反 动画
            //血量UI
            attacker.UpdateHealthBarOnAttack?.Invoke(attacker.CurrentHealth, attacker.MaxHealth);// 更新攻击者血量UI
            UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);// 更新防御者血量UI
            //经验值
            if (attacker.CurrentHealth <= 0)
                characterData.UpdateExp(attacker.characterData.killPoint);// 更新防御者经验值
            if (CurrentHealth <= 0)
                attacker.characterData.UpdateExp(characterData.killPoint);// 更新攻击者经验值
        }
        else // 普通伤害逻辑（非盾反）
        {
            // Debug.Log("Character is not defending, applying normal damage logic.");

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
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        //New : 加入内测无敌模式
        if (GetComponent<PlayerController>() != null && GetComponent<PlayerController>().Invincibility)
            return;

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

        // Debug.Log("Weapon Slot position: " + weaponSlot.position);
        // Debug.Log("Weapon Slot rotation: " + weaponSlot.rotation);

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
