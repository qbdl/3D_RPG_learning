using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius; // 可视范围
    public bool isGuard;// 是否为守卫
    private float speed; //原有速度
    protected GameObject attackTarget; // 攻击对象  
    public float lookAtTime;// 观察时间
    private float remainLookAtTime; //剩余的观察时间
    private float lastAttackTime; // 攻击冷却时间
    private Quaternion guardRotation; // 原始旋转角度

    [Header("Patrol State")]
    public float patrolRange; // 巡逻范围
    private Vector3 wayPoint; // 巡逻点
    private Vector3 guardPos;// 守卫初始位置

    //后端bool值配合前端bool动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    #region Basic
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed; // 获取原有速度
        guardPos = transform.position; // 记录守卫初始位置
        guardRotation = transform.rotation; // 记录原始旋转角度
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
        //TODO:场景切换后修改掉
        GameManager.Instance.AddObserver(this); // 注册观察者
    }

    //切换场景时启用
    // void OnEnable()//启用时调用
    // {
    //     GameManager.Instance.AddObserver(this); // 注册观察者
    // }

    void OnDisable()//销毁时调用
    {
        if (!GameManager.IsInitialized) return; // 如果GameManager未初始化，则不执行注销操作

        GameManager.Instance.RemoveObserver(this); // 注销观察者

        if (GetComponent<LootSpawner>() && isDead)
            GetComponent<LootSpawner>().Spawnloot(); // 如果有LootSpawner组件且敌人已死亡，则生成掉落物品

        //死亡时更新任务进度
        if (QuestManager.IsInitialized && isDead)
            QuestManager.Instance.UpdateQuestProgress(this.name, 1); // 更新任务进度，传入敌人名称和数量（1）
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth <= 0;
        // 更新状态
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime; // 更新攻击冷却
        }
    }

    #endregion

    #region Logic
    void SwitchAnimation()//切换动画
    {
        //用后端数值设置到前端的变量 来控制动画切换
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()//切换Enemy状态
    {
        //死亡，切换到DEAD状态
        if (isDead)
            enemyStates = EnemyStates.DEAD;

        //如果发现player，则切换到CHASE状态
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            // Debug.Log("Player found.");
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (Vector3.Distance(transform.position, guardPos) <= agent.stoppingDistance)
                {
                    isWalk = false; // 如果在守卫位置，则设置为不行走状态
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f); // 恢复原始旋转角度
                }
                else
                {
                    isWalk = true; // 如果不在守卫位置，则设置为行走状态
                    agent.isStopped = false; // 恢复移动
                    agent.destination = guardPos; // 返回守卫位置
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f; // 巡逻时速度减半
                agent.isStopped = false; // <-- 保险起见

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
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())//脱战
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
                        agent.isStopped = false;//恢复移动
                        enemyStates = isGuard ? EnemyStates.GUARD : EnemyStates.PATROL;//恢复之前状态
                    }
                }
                else//追击·
                {
                    isFollow = true;
                    agent.isStopped = false;// 恢复移动
                    agent.destination = attackTarget.transform.position;
                }

                //执行动画（攻击范围内则攻击）
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false; // 停止跟随
                    agent.isStopped = true; // 停止移动

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown; // 重置攻击冷却时间
                                                                             //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        // 执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false; // 禁用碰撞体
                // agent.enabled = false; // 停止NavMeshAgent
                agent.radius = 0; // 设置NavMeshAgent半径为0，防止其他角色碰撞
                Destroy(gameObject, 2f); // 2秒后销毁敌人对象
                break;
            default:
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform); // 面向攻击目标
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack"); // 播放攻击动画
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill"); // 播放技能动画
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

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        return false;
    }
    #endregion

    #region utils
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

    void OnDrawGizmosSelected()
    {
        // 在编辑器中显示可视范围
        // Debug.Log("OnDrawGizmosSelected is called");
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    #endregion

    #region Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))//攻击的时候敌人仍在攻击范围 & 面向目标
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>(); //获取目标的角色属性脚本
            targetStats.TakeDamage(characterStats, targetStats); //调用目标的TakeDamage方法，传入攻击者和防御者的角色属性脚本
        }
        else
        {
            // Debug.Log("Hit not triggered: Target is not in range or not facing the target.");
        }
    }
    #endregion

    #region Interface
    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        anim.SetBool("Win", true);//动画
        playerDead = true; // 玩家死亡

        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
    #endregion
}
