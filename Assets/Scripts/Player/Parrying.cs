using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrying : MonoBehaviour
{
    public Player _player;

    public void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            var effect = PoolManager.Instance.GetFromPool<Effects>("Parrying_Effect");
            effect.transform.localScale = _player.transform.localScale;
            effect.transform.position = col.transform.position;
            StartCoroutine(ParryingSys(1.0f));
            _player.ParryingSuccess();
        }
    }

    public IEnumerator ParryingSys(float time)
    {
        CameraManager.instance.CamShake();
        _player.shockWave.CallShockWave(_player.transform.position);
        _player.isHit = true;
        
        yield return new WaitForSeconds(time);
        
        _player.isHit = false;
    }
}
