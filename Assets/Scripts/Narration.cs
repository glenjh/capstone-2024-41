using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Narration
{
    [Tooltip("이름")] public string name;
    [Tooltip("나레이션 내용")] public string[] Contexts;
}

/// <summary>
/// 나레이션 이벤트
/// </summary>
[Serializable]
public class NarrationEvent
{
    public string name;

    public Vector2 line;
    public Narration[] narrations;
}