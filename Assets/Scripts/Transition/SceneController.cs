using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab; // 玩家对象prefab
    GameObject player;// 玩家实例
    NavMeshAgent playerAgent;// 玩家Agent

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {

        //TODO:保存数据（如玩家状态、物品等）
        // Debug.Log($"Starting transition to scene: {sceneName}, destinationTag: {destinationTag}");

        //检测当前场景与目标场景是否相同
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            // Debug.Log("Loading new scene...");
            // 异步加载新场景
            yield return SceneManager.LoadSceneAsync(sceneName); // yield return 表示 等待这段代码完成才运行这段代码之后的内容

            // Debug.Log("Instantiating player in new scene...");
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);// 实例化玩家对象

            // Debug.Log($"Player instantiated");
            yield break;// 结束协程，避免后续代码执行
        }
        else
        {
            // Debug.Log("Staying in the same scene...");
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            // if (player == null || playerAgent == null)
            // {
            //     Debug.LogError("Player or NavMeshAgent not found!");
            //     yield break;
            // }

            playerAgent.enabled = false; // 禁用NavMeshAgent以避免传送时的移动
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);// 设置玩家位置和旋转
            playerAgent.enabled = true; // 重新启用NavMeshAgent
            yield return null;
        }
    }

    // 获取传送目的地
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var entrance in entrances)
        {
            if (entrance.destinationTag == destinationTag)
                return entrance;
        }
        return null;
    }
}
