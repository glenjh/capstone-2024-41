using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonSpawner : MonoBehaviour {
    // 몬스터 타입
    public MonType monType;
    public String monName;
    
    // 몬스터 수
    public int monCount = 0;
    public int maxMonCount = 1;

    // 몬스터 스폰
    public void Spawn(Transform player)
    {
        var monster = PoolManager.Instance.GetFromPool<Monster>(monName);
        //Monster monster = MonsterPool.GetObject(monType);
        monster.transform.position = transform.position;
        monster.player = player;
        monster.Init(monName);
        monster.OnDieCallBack = ReSpawn;
        
        monCount++;
    }
    
    public void ReSpawn()
    {
        monCount--;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(monCount >= maxMonCount)
            return;
        if (other.CompareTag("MonCol"))
        {
            Spawn(other.transform);
        }
    }
}