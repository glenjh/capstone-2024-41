using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Start()
    {
        text.DOFade(0f, 1).SetLoops(-1, LoopType.Yoyo);
    }
}
