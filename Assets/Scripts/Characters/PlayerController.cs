using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Awake()//在对象被加载时调用
    {
        agent = GetComponent<NavMeshAgent>();//获取前端的NavMeshAgent
        anim = GetComponent<Animator>();//获取前端的Animator
    }

    void Start()//在对象启用后、第一次更新帧之前调用
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }

    void Update()
    {
        SwitchAnimation(); //每帧更新动画状态
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); //设置前端的动画速度参数
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }
}
