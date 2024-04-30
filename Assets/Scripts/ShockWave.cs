using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    private SpriteRenderer sr;
    private float spriteX;
    private float spriteY;
    
    [SerializeField] private float speed = 1f;
    private Coroutine _coroutine;
    private Material _material;
    
    static int _waveDisFromCenter = Shader.PropertyToID("_WaveDisFromCenter");
    private static readonly int RingSpawnPosition = Shader.PropertyToID("_RingSpawnPosition");

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        _material = sr.material;
        spriteX = sr.sprite.bounds.size.x;
        spriteY = sr.sprite.bounds.size.y;
    }
    
    public void CallShockWave(Vector3 pos)
    {
        if (Camera.main != null)
        {
            //word position to screen position
            Vector3 screenPos = Camera.main.WorldToViewportPoint(pos);
            
            _material.SetVector(RingSpawnPosition, screenPos);
        }


        _coroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }
    
    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDisFromCenter, startPos);
        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        
        while(elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            
            lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / speed);
            _material.SetFloat(_waveDisFromCenter, lerpedAmount);
            
            yield return null;
        }
    }

    // Update is called once per frame
    void Update() {
        float screenY = Camera.main.orthographicSize * 2;
        float screenX = screenY / Screen.height * Screen.width;
        
        transform.localScale = new Vector2(MathF.Ceiling(screenX / spriteX), MathF.Ceiling(screenY / spriteY));
    }
}