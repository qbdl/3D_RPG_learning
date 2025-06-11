using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; //攻击范围
    public float skillRange;//远程距离
    public float coolDown;
    public int minDamage;//最小攻击数值
    public int maxDamage;//最大攻击数值

    public float criticalMultiplier;//暴击伤害乘以的倍率
    public float criticalChance;//暴击几率

    public void ApplyWeaponData(AttackData_SO weapon)
    {
        //TODO:目前是重置，而不是叠加(需要在加载时加上，卸载时减去)

        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalMultiplier = weapon.criticalMultiplier;
        criticalChance = weapon.criticalChance;
    }

}
