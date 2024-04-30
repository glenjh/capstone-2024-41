using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    
    public GameObject[] poolPerfabs;
    public int poolingCnt;

    private Dictionary<object, Queue<GameObject>> pooledObjs
        = new Dictionary<object, Queue<GameObject>>();

    public void Init()
    {
        for (int i = 0; i < poolPerfabs.Length; i++)
        {
            if (!pooledObjs.ContainsKey(poolPerfabs[i].name))
            {
                Queue<GameObject> newQueue = new Queue<GameObject>();
                pooledObjs.Add(poolPerfabs[i].name, newQueue);
            }
            for (int j = 0; j < poolingCnt; j++)
            {
                var newObj = Instantiate(poolPerfabs[i], transform);
                newObj.SetActive(false);
                pooledObjs[poolPerfabs[i].name].Enqueue(newObj);
            }
        }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Init();
    }

    public GameObject GetObject(string name)
    {
        if (instance.pooledObjs[name].Count > 0)
        {
            var obj = instance.pooledObjs[name].Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var obj = Instantiate(poolPerfabs.First(obj => obj.name == name), transform);
            obj.transform.SetParent(null);
            obj.SetActive(true);
            return obj;
        }
    }

    public void ReturnObject(string name, GameObject obj)
    {
        if (pooledObjs.ContainsKey(name))
        {
            obj.SetActive(false);
            obj.transform.SetParent(instance.transform);
            instance.pooledObjs[name].Enqueue(obj);
        }
    }
}
