using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLine : MonoBehaviour, IPoolObject {
    private LineRenderer _lineRenderer;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(Vector2 start, Vector2 end)
    {
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }

    public void OnCreatedInPool()
    {
        StartCoroutine(Warning());
    }

    IEnumerator Warning()
    {
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
        yield return new WaitForSeconds(0.5f);
        PoolManager.Instance.TakeToPool<WarningLine>(this);
    }

    public void OnGettingFromPool()
    {
        StartCoroutine(Warning());
    }
}