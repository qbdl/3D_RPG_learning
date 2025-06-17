using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "qbdl"; // 当前场景名称

    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this); // 仅保护当前脚本实例（SaveManager）不被销毁。
    }

    /* -------- Used for test -------- */
    void Update()
    {
        // This method can be used for periodic saving if needed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Exit the game when 'Escape' is pressed

            SaveManager.Instance.SavePlayerData();  // 保存玩家数据
            InventoryManager.Instance.SaveData();   // 保存背包和装备栏数据

            SceneController.Instance.TransitionToMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Save the current game state when 'S' is pressed
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Load the game state when 'L' is pressed
            LoadPlayerData();
        }
    }
    public void SavePlayerData()
    {
        var playerData = GameManager.Instance.playerStats.characterData;
        Save(playerData, playerData.name);
        // Debug.Log("Player data saved for: " + playerData.name); // 调试输出
    }

    public void LoadPlayerData()
    {
        var playerData = GameManager.Instance.playerStats.characterData;
        Load(playerData, playerData.name);
        // Debug.Log("Player data loaded for: " + playerData.name); // 调试输出
    }

    /* -------- -------- */
    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name); // 保存当前场景名称
        PlayerPrefs.Save();
    }
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var jsonData = PlayerPrefs.GetString(key);
            JsonUtility.FromJsonOverwrite(jsonData, data);
        }
    }
}
