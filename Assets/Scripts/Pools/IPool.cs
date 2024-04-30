using UnityEngine;


public interface IPool
{
    Component Source { get; }

    int Count { get; }

    Transform Container { get; }

    IPool SetCount(int count, bool destroyClones = true);

    IPool SetContainer(Transform container, bool worldPositionStays = true);

    IPool Clear(bool destroyClones = true);

    Component Get();

    void Take(Component clone);

    CustomYieldInstruction WaitForFreeObject();
}

public interface IPool<out T> : IPool where T : Component
{
    new T Source { get; }

    new IPool<T> SetCount(int count, bool destroyClones = true);

    new IPool<T> SetContainer(Transform container, bool worldPositionStays = true);

    new IPool<T> Clear(bool destroyClones = true);

    new T Get();
}
