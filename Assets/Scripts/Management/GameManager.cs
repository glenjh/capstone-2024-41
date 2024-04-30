using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Player _player;

    public GameObject qSkillImage;
    
    public void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void LateUpdate()
    {
        if (_player.pulseUnlocked)
        {
            qSkillImage.SetActive(true);
        }
    }
}
