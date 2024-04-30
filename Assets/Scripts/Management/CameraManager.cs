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
    
    [Header("Zoom In / Out")]
    [SerializeField] 
    CinemachineStateDrivenCamera cvCam;

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
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CamShake()
    {
        impulseSource.GenerateImpulse();
    }

    public void ChangeZoom(float zoomAmount)
    {
        foreach (CinemachineVirtualCamera virtualCamera in cvCam.ChildCameras)
        {
            virtualCamera.m_Lens.OrthographicSize += zoomAmount;
        }
    }
}
