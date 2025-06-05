using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }


[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius; // 可视范围
    public bool isGuard;
    private float speed; //原有速度
    private GameObject attackTarget; // 攻击对象  
    public float lookAtTime;// 观察时间
    private float remainLookAtTime; //剩余的观察时间

    [Header("Patrol State")]
    public float patrolRange; // 巡逻范围
    private Vector3 wayPoint; // 巡逻点
    private Vector3 guardPos;// 守卫初始位置



    //后端bool值配合前端bool动画
    bool isWalk;
    bool isChase;
    bool isFollow;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed; // 获取原有速度
        guardPos = transform.position; // 记录守卫初始位置
        remainLookAtTime = lookAtTime; // 初始化剩余观察时间
    }

    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint(); // 获取初始巡逻点
        }

    }
    void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    void SwitchAnimation()//切换动画
    {
        //用后端数值设置到前端的变量 来控制动画切换
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    void SwitchStates()//切换Enemy状态
    {
        //如果发现player，则切换到CHASE状态
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            // Debug.Log("Player found.");
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                // Implement guard behavior
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f; // 巡逻时速度减半

                //是否到达巡逻点
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //TODO:追player

                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())
                {
                    isFollow = false;
                    //拉脱后原地等一会儿
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        //恢复状态
                        if (isGuard) enemyStates = EnemyStates.GUARD;
                        else enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                //执行动画（攻击范围内则攻击）
                break;
            case EnemyStates.DEAD:
                // Implement dead behavior
                break;
            default:
                break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject; // 将玩家标记为攻击对象
                return true;
            }
        }
        attackTarget = null; // 如果没有找到玩家，则清除攻击目标
        return false;
    }

    /* ---------- utils ---------- */
    void GetNewWayPoint()// 获取一个新的巡逻点
    {
        remainLookAtTime = lookAtTime; // 每次新巡逻点生成时，重置观察时间

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        //不改变y值防止悬浮在空中

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        //areaMask 是一个整数值，用作位掩码（Bitmask），表示允许搜索的导航区域。第1位表示Walkable区域,即找到的点是否为Walkable。
    }

    // void OnDrawGizmosSelected()
    // {
    //     // 在编辑器中显示可视范围
    //     // Debug.Log("OnDrawGizmosSelected is called");
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(transform.position, sightRadius);
    // }
}
