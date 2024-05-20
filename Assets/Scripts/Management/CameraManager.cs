using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;
    
    [Header("Shake")]
    CinemachineImpulseSource impulseSource;

    public void Init()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void Awake()
    {
        Init(); 
        //_poolManager = GetComponent<PoolManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CamShake()
    {
        impulseSource.GenerateImpulse();
    }
}
