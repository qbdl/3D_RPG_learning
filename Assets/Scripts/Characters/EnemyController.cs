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

    [Header("Basic Settings")]
    public float sightRadius; // 可视范围



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        SwitchStates();
    }

    void SwitchStates()
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
                // Implement patrol behavior
                break;
            case EnemyStates.CHASE:
                // Implement chase behavior
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
                return true;
        }
        return false;
    }

}
