using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]

    public float kickForce = 10f; // 击力

    public void KickOff()//这里的KickOff不造成伤害,只是击飞，仍是Hit造成伤害
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform); // 面向目标

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized; // 计算方向

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true; // 停止目标的NavMeshAgent
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce; // 应用击力
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy"); // 触发晕眩动画
        }
    }

}
