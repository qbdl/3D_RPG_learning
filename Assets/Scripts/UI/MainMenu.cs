using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;// 用于播放开始动画

    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame; // 动画播放完毕后调用NewGame方法
    }

    void PlayTimeline()
    {
        director.Play(); // 播放开始动画
    }
    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll(); // 清除所有存档数据
        SceneController.Instance.TransitionToFirstLevel(); // 转换到第一个场景
    }
    void ContinueGame()
    {
        //转换场景,读取进度
        SceneController.Instance.TransitionToLoadGame(); // 转换到保存的场景
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }
}
