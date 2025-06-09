using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene, DifferentScene }

    [Header("Transition Info")]

    public string sceneName;// 场景名称
    public TransitionType transitionType; // 传送类型
    public TransitionDestination.DestinationTag destinationTag; // 传送目的地标签

    private bool canTrans;// 是否可以传送

    void Update()
    {
        // if (canTrans)
        //     Debug.Log("Can Trans");

        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            if (transitionType == TransitionType.SameScene)
                SceneController.Instance.TransitionToDestination(this);
            else if (transitionType == TransitionType.DifferentScene)
            {
                // 切换到不同的场景

            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true; // 玩家进入传送点时允许传送
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false; // 玩家离开传送点时禁止传送
    }
}
