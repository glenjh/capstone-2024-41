using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    public void ClearSignal()
    {
        StartCoroutine(Clear());
    }

    IEnumerator Clear()
    {
        yield return new WaitForSecondsRealtime(1f);
        
        MySceneManager.instance.GameClear();
    }
}
