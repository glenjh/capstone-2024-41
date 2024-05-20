using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineSignal : MonoBehaviour
{
    public Animator anim;
    public bool flag;
    public string parameter;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void BoolSignal()
    {
        anim.SetBool(parameter, flag);
    }
}
