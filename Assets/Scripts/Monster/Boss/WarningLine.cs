using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningLine : MonoBehaviour, IPoolObject {
    private LineRenderer _lineRenderer;
    private BoxCollider2D _boxCollider;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Init(Vector2 start, Vector2 end)
    {
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
        var direction = (end - start).normalized;
        var pos = (start + end) / 2;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        _boxCollider.size = new Vector2(Vector2.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1)), 0.2f);
        _boxCollider.enabled = false;
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
        _boxCollider.enabled = true;
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        //콜라이더를 라인렌더러 크기와 위치로 변경
        yield return new WaitForSeconds(0.5f);
        PoolManager.Instance.TakeToPool<WarningLine>(this);
    }

    public void OnGettingFromPool()
    {
        StartCoroutine(Warning());
    }
}