using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BezierMissile : MonoBehaviour
{
    Vector2[] m_points = new Vector2[4];

    private float m_timerMax = 0;
    private float m_timerCurrent = 0;
    private float m_speed;
    private string[] m_targetTag;

    public void Init(Transform _startTr, Transform _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr, string[] _targetTag)
    {
        m_speed = _speed;

        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // 시작 지점.
        m_points[0] = _startTr.position; 

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = _startTr.position +
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * _startTr.right) + 
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * _startTr.up);

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        m_points[2] = _endTr.position +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.right) +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.up);

        // 도착 지점.
        m_points[3] = _endTr.position;
        
        m_targetTag = _targetTag;

        transform.position = _startTr.position;
    }

    void Update()
    {
        if (m_timerCurrent > m_timerMax)
        {
            Destroy(this.gameObject);
            return;
        }

        // 경과 시간 계산.
        m_timerCurrent += Time.deltaTime * m_speed;

        // 베지어 곡선으로 X,Y,Z 좌표 얻기.
        transform.position = new Vector2(
            CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
            CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y));
    }

    /// <summary>
    /// 3차 베지어 곡선.
    /// </summary>
    /// <param name="a">시작 위치</param>
    /// <param name="b">시작 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="c">도착 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="d">도착 위치</param>
    /// <returns></returns>
    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        float t = m_timerCurrent / m_timerMax; // (현재 경과 시간 / 최대 시간)
        
        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);

        float abbc = Mathf.Lerp(ab, bc, t);
        float bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tag in m_targetTag)
        {
            if (other.CompareTag(tag))
            {
                other.GetComponent<IDamageAble>()?.TakeHit(1,this.transform);
                Destroy(this.gameObject);
            }
        }
    }
}