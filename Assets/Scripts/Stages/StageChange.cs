using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChange : MonoBehaviour
{
    public string nextStage;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (DataManager.instance.currMode == GameMode.BossRushMode)
            {
                DataManager.instance.rushData.score += 1000;
            }
            DataManager.instance.SaveData();
            Destroy(this.gameObject);
            MySceneManager.instance.ChangeScene(nextStage);
        }
    }
}
