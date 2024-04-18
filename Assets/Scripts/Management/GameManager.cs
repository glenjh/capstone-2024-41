using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player _player;
    public TextMeshProUGUI playerHealthText;

    public GameObject qSkillImage;

    void LateUpdate()
    {
        playerHealthText.text = _player.life + " / " + _player.maxLife;

        if (_player.pulseUnlocked)
        {
            qSkillImage.SetActive(true);
        }
    }
}
