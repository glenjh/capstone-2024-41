using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.IO;

public enum GameMode
{
    StoryMode,
    BossRushMode
}

public class PlayerData
{
    public float playTime = 0;
    public float score = 0;
    public string currStage = "";
    public int life = 10;
    public float charge = 0f;
    public bool[] skillUnlock = {false, false, false, false}; // Q, W, E, R
    public bool[] additionalStates = {false, false}; // parrying, wallSlide

    public PlayerData(string stage)
    {
        currStage = stage;
    }
}

public class DataManager : MonoBehaviour
{
    public GameMode currMode;
    public static DataManager instance = null;
    public PlayerData storyData = new PlayerData("Stage 0");
    public PlayerData rushData = new PlayerData("Rush 1");
    public string storyPath;
    public string rushPath;

    #region Singleton
    public void Init()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        storyPath = Application.persistentDataPath + "/story";
        rushPath = Application.persistentDataPath + "/rush";
    }
    
    void Awake()
    {
        Init();
    }
    #endregion

    public string GetCurrSavePath()
    {
        if (currMode == GameMode.StoryMode)
        {
            return storyPath;
        }
        else
        {
            return rushPath;
        }
    }

    public PlayerData GetCurrData()
    {
        if (currMode == GameMode.StoryMode)
        {
            return storyData;
        }
        else
        {
            return rushData;
        }
    }

    public void SaveData()
    {
        Debug.Log("Saved");

        if (FindObjectOfType<Player>())
        {
            Player.player._playerData.playTime = MySceneManager.instance.timer;
        }
        string path = GetCurrSavePath();
        string data = JsonUtility.ToJson(GetCurrData());
        
        File.WriteAllText(path, data);
    }

    public void LoadStoryData()
    {
        MySceneManager.instance.countAble = true;
        if (File.Exists(storyPath))
        {
            Debug.Log("Old Story");
            string data = File.ReadAllText(storyPath);
            storyData = JsonUtility.FromJson<PlayerData>(data);
            
            MySceneManager.instance.ChangeScene(storyData.currStage);
        }
        else
        {
            Debug.Log("New Story");
            storyData = new PlayerData("Stage 0");
            SaveData();
            MySceneManager.instance.ChangeScene("Stage 0");
        }
    }

    public void LoadRushData()
    {
        MySceneManager.instance.countAble = true;
        if (File.Exists(rushPath))
        {
            Debug.Log("Old rush");
            string data = File.ReadAllText(rushPath);
            rushData = JsonUtility.FromJson<PlayerData>(data);
            
            MySceneManager.instance.ChangeScene(rushData.currStage);
        }
        else
        {
            Debug.Log("New rush");
            rushData = new PlayerData("Rush 1");
            SaveData();
            MySceneManager.instance.ChangeScene("Rush 1");
        }
    }

    public void InitData()
    {
        if (currMode == GameMode.StoryMode)
        {
            storyData = new PlayerData("Stage 0");
        }
        else if(currMode == GameMode.BossRushMode)
        {
            rushData = new PlayerData("Rush 1");
        }
        
        SaveData();
    }

    public void ClearData()
    {
        if (currMode == GameMode.StoryMode)
        {
            storyData = new PlayerData("Stage 0");
        }
        else if(currMode == GameMode.BossRushMode)
        {
            rushData = new PlayerData("Rush 1");
        }
        
        SaveData();
    }
}
