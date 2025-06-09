using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void NewGame()
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
