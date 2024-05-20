using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMissileShooter : MonoBehaviour
{
    public GameObject m_missilePrefab; // 미사일 프리팹.

    public String[] targetLayer; // 타겟 레이어.
    public String[] targetTag; // 타겟 태그.
    public float m_range = 10.0f; // 타겟을 찾을 범위.
    
    [Header("미사일 기능 관련")]
    public float m_speed = 2; // 미사일 속도.
    [Space(10f)]
    public float m_distanceFromStart = 6.0f; // 시작 지점을 기준으로 얼마나 꺾일지.
    public float m_distanceFromEnd = 3.0f; // 도착 지점을 기준으로 얼마나 꺾일지.
    [Space(10f)]
    public int m_shotCount = 12; // 총 몇 개 발사할건지.
    [Range(0, 1)] public float m_interval = 0.15f;
    public int m_shotCountEveryInterval = 2; // 한번에 몇 개씩 발사할건지.

    public Transform[] FindTarget()
    {
        Transform[] m_target = new Transform[0];
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, m_range);

        foreach (var collider in colliders)
        {
            foreach (var layer in targetLayer)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer(layer))
                {
                    Array.Resize(ref m_target, m_target.Length + 1);
                    m_target[m_target.Length - 1] = collider.transform;
                }
            }
        }
        
        return m_target;
    }

    public void Fire()
    { 
        StartCoroutine(CreateMissile(FindTarget()));
    }
    
    public void Fire(int _shotCount)
    {
        m_shotCount = _shotCount;
        m_shotCountEveryInterval = _shotCount;
        StartCoroutine(CreateMissile(FindTarget()));
    }

    IEnumerator CreateMissile(Transform[] m_target)
    {
        int _shotCount = m_shotCount;
        while (_shotCount > 0)
        {
            for (int i = 0; i < m_shotCountEveryInterval && _shotCount > 0; i++)
            {
                foreach (var pos in m_target)
                {
                    if (_shotCount > 0)
                    {
                        GameObject missile = Instantiate(m_missilePrefab);
                        missile.GetComponent<BezierMissile>().Init(this.gameObject.transform, pos.transform,
                            m_speed, m_distanceFromStart, m_distanceFromEnd, targetTag);

                        _shotCount--;
                    }
                    else
                    {
                        break;
                    }
                }

            }

            yield return new WaitForSeconds(m_interval);
        }

        yield return null;
    }
}
