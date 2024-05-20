using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WebGenerator : MonoBehaviour {
    [SerializeField] private WebPos[] _webPos;
    public void OnEnable()
    {
        for (int i = 0; i < 5; i++)
        {
            var temp = Random.Range(0, _webPos.Length);
            WarningLine web = PoolManager.Instance.GetFromPool<WarningLine>();
            web.Init(_webPos[temp].webPos[0], _webPos[temp].webPos[1]);
        }
    }
}

[System.Serializable]
public class WebPos
{
    public Vector2[] webPos;
}