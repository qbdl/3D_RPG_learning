using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()//在对象被加载时调用
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()//在对象启用后、第一次更新帧之前调用
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }
}
