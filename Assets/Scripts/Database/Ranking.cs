using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

using Firebase;
using Firebase.Database;
using Firebase.Unity;



public class Ranking : MonoBehaviour
{
    public GameObject rankingUI;
    public TMP_InputField nameInput;

    public TextMeshProUGUI[] Rank;
    public List<User> users = new List<User>();
    private long len;
    
    public bool readDone = false;
    public class User
    {
        public string name;
        public string score;
        public string playTime;

        public User(string name, string score, string playTime)
        {
            this.name = name;
            this.score = score;
            this.playTime = playTime;
        }
    }
    
    DatabaseReference reference;
    
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClickSave()
    {
        WriteNewUSer("Rank", nameInput.text, DataManager.instance.rushData.score.ToString(), MySceneManager.instance.timerText.text);
        ReadRank();
    }

    public void LateUpdate()
    {
        if (readDone)
        {
            TextLoad();
        }
    }

    public void LoadBut()
    {
        ReadRank();
    }

    public void WriteNewUSer(string rank, string name, string score, string playTime)
    {
        User user = new User(name, score, playTime);
        string json = JsonUtility.ToJson(user);
        reference.Child(rank).Child(name).SetRawJsonValueAsync(json);
    }

    public void ReadRank()
    {
        reference.Child("Rank").OrderByChild("score").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("error");
                ReadRank();
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                len = snapshot.ChildrenCount;
                
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary personInfo = (IDictionary)data.Value;
                    users.Add(new User(personInfo["name"].ToString(), personInfo["score"].ToString(), personInfo["playTime"].ToString()));
                }
                
                readDone = true;
            }
        });
    }

    public void TextLoad()
    {
        rankingUI.SetActive(true);
        readDone = false;
        List<User> sortedUsers;
        
        try
        {
            sortedUsers = users.OrderByDescending(item => item.score).
                ThenBy(item => item.playTime).ToList();
            
            foreach (var user in sortedUsers)
            {
                Debug.Log($"이름: {user.name}, 점수: {user.score}, 시간{user.playTime}");
            }
        }
        catch (NullReferenceException)
        {
            return;
        }

        for (int i = 0; i < 10 ; i++)
        {
            if (len <= i)
            {
                Rank[i].text = "----" + "  " + "Score: " + "----" + "  "
                               + "PlayTime: " + "--:--:--";
            }
            else
            {
                Rank[i].text = sortedUsers[i].name + "  " + "Score: " + sortedUsers[i].score + "  "
                               + "PlayTime: " + sortedUsers[i].playTime;
            }
        }
    }

    public void Reset()
    {
        users.Clear();
        len = 0;
        nameInput.text = "";
        for (int i = 0; i < 10 ; i++)
        {
            Rank[i].text = "----" + "  " + "Score: " + "----" + "  "
                           + "PlayTime: " + "--:--:--";
        }
    }
}
