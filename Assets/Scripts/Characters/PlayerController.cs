using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private GameObject attackTarget; //记录传入的攻击目标对象
    private float lastAttackTime; //上次攻击时间(为了攻击有间隔)



    void Awake()//在对象被加载时调用
    {
        agent = GetComponent<NavMeshAgent>();//获取前端的NavMeshAgent
        anim = GetComponent<Animator>();//获取前端的Animator
    }

    void Start()//在对象启用后、第一次更新帧之前调用
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

    }

    void Update()
    {
        SwitchAnimation(); //每帧更新动画状态
        lastAttackTime -= Time.deltaTime; // 攻击冷却时间的衰减
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); //设置前端的动画速度参数
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines(); //停止所有协程，使得自己可以打断自己刚选择的攻击
        agent.isStopped = false; //允许移动
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget()); //开始协程移动、攻击目标
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false; //开始移动

        transform.LookAt(attackTarget.transform); //面向目标

        //TODO:修改攻击范围
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > 1.0f)//还没到目标面前
        {
            agent.destination = attackTarget.transform.position;
            yield return null; //等待下一帧
        }

        agent.isStopped = true; //停止移动

        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack"); //触发攻击动画
            //重置冷却时间 
            //TODO:冷却时间的调整
            lastAttackTime = 0.5f; //设置攻击间隔时间
        }
    }
}
