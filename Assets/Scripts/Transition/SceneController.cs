using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab; // 玩家对象prefab
    public SceneFader sceneFaderPrefab; // 场景淡入淡出效果prefab

    bool fadeFinished; // 场景淡入淡出是否正在进行（防止重复进行）
    private float fadeTime;// 场景淡入淡出时间

    GameObject player;// 玩家实例
    NavMeshAgent playerAgent;// 玩家Agent

    void Start()
    {
        GameManager.Instance.AddObserver(this); // 注册观察者
        fadeFinished = true;
        fadeTime = 1.1f;
    }


    // 用于在游戏中传送到指定场景或位置
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

    // 用于在游戏中退回到主场景
    public void TransitionToMainMenu()
    {
        StartCoroutine(LoadMain());
    }
    // 用于在游戏进入时加载指定场景
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName)); // 加载保存的场景
    }
    // 用于在游戏开始时加载第一个关卡
    public void TransitionToFirstLevel()
    {
        // Debug.Log("Transitioning to first level...");
        StartCoroutine(LoadLevel("SimpleNature")); // 加载第一个关卡SimpleNature
    }
    // 用于在游戏结束(玩家死亡）时加载主菜单场景
    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false; // 设置淡入淡出正在进行，防止重复执行
            StartCoroutine(LoadMain());// 当游戏结束时，加载主菜单场景
        }
    }

    /* ------------- IEnumerator ------------- */

    // 用于在游戏中传送到指定场景或位置
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {

        //保存数据（如玩家状态、物品等）
        SaveManager.Instance.SavePlayerData();

        //检测当前场景与目标场景是否相同
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneFader temp_fade = Instantiate(sceneFaderPrefab); // 实例化场景淡入淡出效果
            yield return StartCoroutine(temp_fade.FadeOut(fadeTime)); // 先淡出当前场景

            // 异步加载新场景
            yield return SceneManager.LoadSceneAsync(sceneName); // yield return 表示 等待这段代码完成才运行这段代码之后的内容
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);// 实例化玩家对象
            SaveManager.Instance.LoadPlayerData(); // 加载玩家数据

            yield return StartCoroutine(temp_fade.FadeIn(fadeTime)); // 再淡入新场景

            yield break;// 结束协程，避免后续代码执行
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false; // 禁用NavMeshAgent以避免传送时的移动
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);// 设置玩家位置和旋转
            playerAgent.enabled = true; // 重新启用NavMeshAgent
            yield return null;
        }
    }
    //用于在游戏进入时加载指定场景
    IEnumerator LoadLevel(string sceneName)
    {
        fadeFinished = true; // 设置淡入淡出完成，允许后续操作

        SceneFader temp_fade = Instantiate(sceneFaderPrefab); // 实例化场景淡入淡出效果

        // 异步加载新场景
        if (sceneName != "")
        {
            yield return StartCoroutine(temp_fade.FadeOut(fadeTime)); // 先淡出当前场景

            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);// 实例化玩家对象

            //保存游戏
            SaveManager.Instance.SavePlayerData();

            yield return StartCoroutine(temp_fade.FadeIn(fadeTime)); // 再淡入新场景

            yield break;
        }
    }
    //用于在游戏中退回到主场景
    IEnumerator LoadMain()
    {
        SceneFader temp_fade = Instantiate(sceneFaderPrefab); // 实例化场景淡入淡出效果
        yield return StartCoroutine(temp_fade.FadeOut(fadeTime)); // 先淡出当前场景
        yield return SceneManager.LoadSceneAsync("SimpleNaturePack_For_MainUI"); // 异步加载主菜单场景
        yield return StartCoroutine(temp_fade.FadeIn(fadeTime)); // 再淡入新场景
        yield break;
    }
}
