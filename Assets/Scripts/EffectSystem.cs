using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour, IPoolObject
{
    [SerializeField] private string effectName;
    public void ReturnEffect()
    {
        PoolManager.Instance.TakeToPool<EffectSystem>(effectName, this);
    }


    public void OnCreatedInPool()
    { }

    public void OnGettingFromPool()
    {
        //방향 랜덤 설정
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        //스케일 랜덤 설정
        transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f), 1);
    }
}
