using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonSpawner : MonoBehaviour {
    // 몬스터가 스폰되어 있는지
    public bool isSpawned;
    // 몬스터 타입
    public MonType monType;

    // 몬스터 스폰
    public void Spawn()
    {
        Monster monster = MonsterPool.GetObject(monType);
        monster.transform.position = transform.position;
        
        monster.transform.SetParent(transform);
    }
    
    public void ReSpawn()
    {
        isSpawned = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MonCol"))
        {
            if (!isSpawned)
            {
                isSpawned = true;
                Spawn();
            }
        }
    }
}