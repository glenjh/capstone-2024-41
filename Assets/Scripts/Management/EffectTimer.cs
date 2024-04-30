using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTimer : MonoBehaviour
{
    private float lifeTime;
    public float timeToReturn;
    public string effectName;

    void Start()
    {
        lifeTime = 0f;
    }
    
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= timeToReturn)
        {
            lifeTime = 0f;
            ObjectPoolManager.instance.ReturnObject(effectName, this.gameObject);
        }
    }
}