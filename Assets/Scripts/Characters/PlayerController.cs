using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats; //角色属性脚本
    private GameObject attackTarget; //记录传入的攻击目标对象
    private float lastAttackTime; //上次攻击时间(为了攻击有间隔)

    bool isDead; //是否死亡

    private float stopDistance; //默认的停止距离

    /* ---------- Basic Function ---------- */
    void Awake()//在对象被加载时调用
    {
        agent = GetComponent<NavMeshAgent>();//获取前端的NavMeshAgent
        anim = GetComponent<Animator>();//获取前端的Animator
        characterStats = GetComponent<CharacterStats>(); //获取角色属性脚本

        stopDistance = agent.stoppingDistance; //记录默认的停止距离
    }

    void Start()//在对象启用后、第一次更新帧之前调用
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RigisterPlayer(characterStats); //注册玩家角色属性到GameManager
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth <= 0;
        if (isDead)//进行广播
            GameManager.Instance.NotifyObservers(); //通知所有观察者

        SwitchAnimation(); //每帧更新动画状态
        lastAttackTime -= Time.deltaTime; // 攻击冷却时间的衰减
    }

    /* ---------------- --- ------------------- */

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); //设置前端的动画速度参数
        anim.SetBool("Death", isDead); //设置死亡状态
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines(); //停止所有协程，使得自己可以打断自己刚选择的攻击
        if (isDead) return; //如果已经死亡，则不允许移动

        agent.stoppingDistance = stopDistance; //正常移动时：停止距离为默认值
        agent.isStopped = false; //允许移动
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (isDead) return; //如果已经死亡，则不允许移动

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;//是否暴击

            StartCoroutine(MoveToAttackTarget()); //开始协程移动、攻击目标
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false; //开始移动
        agent.stoppingDistance = characterStats.attackData.attackRange; //攻击时：设置停止距离为攻击范围(防止其一直为一个较小值，而敌人较大，始终无法到达那个位置开始攻击)

        transform.LookAt(attackTarget.transform); //面向目标

        while (Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)//还没到目标面前
        {
            agent.destination = attackTarget.transform.position;
            yield return null; //等待下一帧
        }

        agent.isStopped = true; //停止移动

        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack"); //触发攻击动画

            //重置冷却时间 
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    /* ---------- Animation Event ---------- */
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable")) //如果攻击目标是Attackable类型
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)//攻击目标为石头
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy; //设置其状态为攻击敌人
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one; //避免其被认为是HitNothing状态
                //TODO:这里暂时冲击力使用常数用于查看效果
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20.0f, ForceMode.Impulse); //应用冲击力
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>(); //获取目标的角色属性脚本
            targetStats.TakeDamage(characterStats, targetStats); //调用目标的TakeDamage方法，传入攻击者和防御者的角色属性脚本
        }
    }
}
