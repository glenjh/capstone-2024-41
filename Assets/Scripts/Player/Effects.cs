using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Effects : MonoBehaviour, IPoolObject
{
    public string effectName;
    public float deleteTime;

    public void OnCreatedInPool()
    {
        
    }

    public void OnGettingFromPool()
    {
        
    }

    public void ReturnEffect()
    {
        PoolManager.Instance.TakeToPool<Effects>(effectName, this);
    }
}
