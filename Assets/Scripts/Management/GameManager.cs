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

    void LateUpdate()
    {
        playerHealthText.text = _player.life + " / " + _player.maxLife;
    }
}
