using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonType
{
    Mecha,
    Drone,
    Droid
}

public class MonsterPool : MonoBehaviour
{
    public static MonsterPool Instance;

    [SerializeField]
    private List<GameObject> poolingObjectPrefabList = new List<GameObject>();

    List<Monster> _poolingObjectList = new List<Monster>();

    private void Awake()
    {
        Instance = this;

        Initialize(10);
    }

    private void Initialize(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            //프리펩 리스트 사이즈만큼 돌면서 추가
            for (int j = 0; j < poolingObjectPrefabList.Count; j++)
            {
                _poolingObjectList.Add(CreateNewObject(j));
            }
        }
    }

    private Monster CreateNewObject(int index = 0)
    {
        var newObj = Instantiate(poolingObjectPrefabList[index]).GetComponent<Monster>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        newObj.monsterType = (MonType)index;
        return newObj;
    }

    public static Monster GetObject(MonType type)
    {
        foreach (var obj in Instance._poolingObjectList)
        {
            if (obj.monsterType == type)
            {
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                obj.Init();
                return obj;
            }
        }
        var newObj = Instance.CreateNewObject((int)type);
        newObj.gameObject.SetActive(true);
        newObj.transform.SetParent(null);
        newObj.Init();
        return newObj;
    }

    public static void ReturnObject(Monster obj)
    {
        obj.StopAllCoroutines();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance._poolingObjectList.Add(obj);
    }
}