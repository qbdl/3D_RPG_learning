using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]

    public float kickForce = 25.0f; // 击力
    public GameObject rockPrefab; // 石头预制体
    public Transform handPos; // 手位置(石头生成位置)

    /* ---------- Animation Event ---------- */
    public void KickOff()//这里的KickOff造成伤害,且击飞，没有使用Hit的Event
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>(); //获取目标的角色属性脚本

            Vector3 directon = (attackTarget.transform.position - transform.position).normalized;

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true; // 停止目标的NavMeshAgent
            attackTarget.GetComponent<NavMeshAgent>().velocity = directon * kickForce; // 应用击力
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy"); // 触发晕眩动画

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity); // 在手位置生成石头
            rock.GetComponent<Rock>().target = attackTarget; // 设置目标为攻击对象
        }
    }
}
