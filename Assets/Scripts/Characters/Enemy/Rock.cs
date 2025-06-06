using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing } //石头状态枚举
    private Rigidbody rb; //刚体组件
    public RockStates rockStates; //石头状态

    [Header("Basic Settings")]

    public float force; //投掷力
    public int damage; //伤害值
    public GameObject target; //目标对象
    private Vector3 direction; //飞行方向
    public GameObject breakEffect; //破碎效果

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one; //初始化速度，避免石头在生成时被认为HitNothing

        rockStates = RockStates.HitPlayer; //初始状态为攻击玩家
        FlyToTarget(); //开始时向目标飞行
    }

    void FixedUpdate()
    {
        // Debug.Log("Rock velocity: " + rb.velocity.sqrMagnitude); //输出石头的速度
        if (rb.velocity.sqrMagnitude < 1.0f) //如果速度过慢，认为石头已经停止
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject; //如果没有设置目标，则默认寻找玩家

        direction = (target.transform.position - transform.position + Vector3.up).normalized;//防止直接砸向player,速度过快
        rb.AddForce(direction * force, ForceMode.Impulse); //应用冲击力
    }

    void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true; // 停止玩家的NavMeshAgent
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force; // 应用击力

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy"); // 触发晕眩动画
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>()); // 造成伤害

                    rockStates = RockStates.HitNothing; // 设置状态不再会造成伤害
                }
                break;

            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats); // 造成伤害
                    Instantiate(breakEffect, transform.position, Quaternion.identity); // 生成破碎效果
                    Destroy(gameObject); // 销毁石头
                }
                break;
        }
    }
}
