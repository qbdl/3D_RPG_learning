using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats; // 玩家角色属性

    private CinemachineFreeLook followCamera; // 玩家摄像机

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // 确保GameManager在场景切换时不会被销毁
    }

    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>(); // 查找场景中的CinemachineFreeLook组件
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2); // 摄像机跟随玩家的LookAtPoint
            followCamera.LookAt = playerStats.transform.GetChild(2); // 摄像机注视玩家的LookAtPoint
            Debug.Log("playerStats.transform.GetChild(2).name: " + playerStats.transform.GetChild(2).name); // 输出LookAtPoint的名称
            Debug.Log("playerStats.transform.GetChild(2).position: " + playerStats.transform.GetChild(2).position); // 输出LookAtPoint的位置
        }
        else
        {
            Debug.LogError("CinemachineFreeLook component not found in the scene. Please ensure it is present for camera follow functionality.");
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//广播
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var entrance in FindObjectsOfType<TransitionDestination>())
        {
            if (entrance.destinationTag == TransitionDestination.DestinationTag.ENTER) // 检查传送目的地标签是否为ENTER
                return entrance.transform;
        }
        return null;
    }
}
