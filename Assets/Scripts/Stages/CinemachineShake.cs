using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    
    private CinemachineVirtualCamera cvCam;
    private float shakeTimer;
    private float shakeTimeTotal;
    private float startingIntenity;
    
    void Awake()
    {
        Instance = this;
        cvCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void CVCamShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cvCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        startingIntenity = intensity;
        shakeTimeTotal = time;
        shakeTimer = time;
    }

    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    cvCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntenity, 0, 1 - (shakeTimer/shakeTimeTotal));
            }
        }
    }
}
