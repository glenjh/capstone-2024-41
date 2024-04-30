using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
internal struct PoolData
{
    [SerializeField] private string _name;

    public string Name => _name;

    [SerializeField] private Component _component;

    public Component Component => _component;

    [SerializeField] [Min(0)] private int _count;

    public int Count => _count;

    [SerializeField] private Transform _container;

    public Transform Container => _container;
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolData> _pools;

    private readonly List<IPool<Component>> _poolsObjects = new();

    private void Awake()
    {
        var genericPoolType = typeof(Pool<>);

        foreach (var poolData in _pools)
        {
            // var poolType = genericPoolType.MakeGenericType(poolData.Component.GetType());
            // var createMethod = poolType.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
            
            // var pool = createMethod.Invoke(null,
            //     new object[] { poolData.Component, poolData.Count, poolData.Container });
            
            var pool = Pool<Component>.Create(poolData.Component, poolData.Count, poolData.Container);
            _poolsObjects.Add(pool);
        }
    }

    #region Get pool

    public IPool<T> GetPool<T>(int index) where T : Component => (IPool<T>)_poolsObjects[index];

    public IPool<T> GetPool<T>() where T : Component => (IPool<T>)_poolsObjects.Find(p => p.Source is T);

    public IPool<T> GetPool<T>(string name) where T : Component =>
        (IPool<T>)_poolsObjects[_pools.FindIndex(p => p.Name == name)];

    #endregion

    #region Get from pool

    public T GetFromPool<T>(int index) where T : Component => GetPool<T>(index).Get();

    public T GetFromPool<T>() where T : Component => GetPool<T>().Get();

    public T GetFromPool<T>(string name) where T : Component => GetPool<T>(name).Get();

    #endregion

    #region Take to pool

    public void TakeToPool(int index, Component component) => _poolsObjects[index].Take(component);

    public void TakeToPool<T>(Component component) where T : Component => GetPool<T>().Take(component);

    public void TakeToPool<T>(string name, Component component) where T : Component =>
        GetPool<T>(name).Take(component);

    #endregion
}
    