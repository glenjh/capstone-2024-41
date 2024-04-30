using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPauser : MonoBehaviour
{
    public float amount;
    bool isFrozen;
    float freezeDuration = 0f;
    
    
    void Update()
    {
        if (freezeDuration > 0 && !isFrozen)
        {
            StartCoroutine(DoStop());
        }
    }

    public void HitStop(float x)
    {
        amount = x;
        freezeDuration = amount;
    }

    IEnumerator DoStop()
    {
        isFrozen = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(amount);

        Time.timeScale = 1f;
        freezeDuration = 0f;
        isFrozen = false;
    }
}
