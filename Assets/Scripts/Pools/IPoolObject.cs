using UnityEngine;

public interface IPoolObject
{
    void OnCreatedInPool();

    void OnGettingFromPool();
}
