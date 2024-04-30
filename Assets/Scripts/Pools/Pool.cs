using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Pool
{
    public static Pool<T> Create<T>(T source, int count = 0, Transform container = null) where T : Component =>
        Pool<T>.Create(source, count, container);
}

public class Pool<T> : IPool<T> where T : Component
{
    #region Fields and properties

    private T _source;

    Component IPool.Source => _source;

    public T Source => _source;

    private int _count;

    public int Count => _count;

    private Transform _container;

    public Transform Container => _container;

    private List<T> _clones;

    private readonly List<T> _busyObjects = new();

    #endregion

    private Pool()
    {
    }

    internal static Pool<T> Create(T source, int count, Transform container)
    {
        var pool = new Pool<T>
        {
            _source = source,
            _count = Math.Max(count, 0),
            _container = container,
            _clones = new List<T>(count)
        };

        return pool;
    }

    #region SetCount

    IPool IPool.SetCount(int count, bool destroyClones) => SetCount(count, destroyClones);

    IPool<T> IPool<T>.SetCount(int count, bool destroyClones) => SetCount(count, destroyClones);

    public Pool<T> SetCount(int count, bool destroyClones = true)
    {
        count = Math.Max(count, 0);

        if (count == 0)
        {
            _count = count;
            return this;
        }

        if (_count != 0 && count > _count)
        {
            _clones.Capacity = _busyObjects.Capacity = _count = count;
            return this;
        }

        if (destroyClones)
        {
            for (int i = count; i < _clones.Count; i++)
                Object.Destroy(_clones[i].gameObject);
        }

        _clones.RemoveRange(count, _clones.Count - count);
        _count = count;
        _clones.Capacity = _busyObjects.Capacity = _count;

        return this;
    }

    #endregion

    #region SetContainer

    IPool IPool.SetContainer(Transform container, bool worldPositionStays) =>
        SetContainer(container, worldPositionStays);

    IPool<T> IPool<T>.SetContainer(Transform container, bool worldPositionStays) =>
        SetContainer(container, worldPositionStays);

    public Pool<T> SetContainer(Transform container, bool worldPositionStays = true)
    {
        _container = container;
        _clones.ForEach(c => c.transform.SetParent(_container, worldPositionStays));

        return this;
    }

    #endregion

    #region Clear

    IPool IPool.Clear(bool destroyClones) => Clear(destroyClones);

    IPool<T> IPool<T>.Clear(bool destroyClones) => Clear(destroyClones);

    public Pool<T> Clear(bool destroyClones = true)
    {
        if (destroyClones)
        {
            for (int i = 0; i < _clones.Count; i++)
            {
                var clone = _clones[i];

                if (clone != null)
                    Object.Destroy(clone.gameObject);
            }
        }

        _clones.Clear();

        return this;
    }

    #endregion

    #region Get

    Component IPool.Get() => Get();

    public T Get()
    {
        T clone = null;

        for (int i = 0; i < _clones.Count; i++)
        {
            if (!_busyObjects.Contains(_clones[i]))
            {
                clone = _clones[i];
                break;
            }
        }

        if (clone == null)
        {
            if (_count != 0 && _clones.Count >= _count)
                return null;

            _clones.Add(clone = Object.Instantiate(_source, _container));

            if (clone is IPoolObject obj)
                obj.OnCreatedInPool();
        }

        _busyObjects.Add(clone);

        clone.gameObject.SetActive(true);
        if (clone is IPoolObject resetable)
            resetable.OnGettingFromPool();

        return clone;
    }

    #endregion

    #region Take

    void IPool.Take(Component clone) => Take((T)clone);

    public void Take(T clone)
    {
        if (!_clones.Contains(clone))
        {
            if (_count != 0 && _clones.Count >= _count)
                return;

            clone.transform.SetParent(_container, true);
            _clones.Add(clone);
        }
        else
        {
            if (!_busyObjects.Contains(clone))
                return;

            _busyObjects.Remove(clone);
        }

        clone.gameObject.SetActive(false);
    }

    #endregion

    public CustomYieldInstruction WaitForFreeObject() => new WaitWhile(() => _busyObjects.Count == _clones.Count);
}
