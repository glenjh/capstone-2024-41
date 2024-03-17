using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseSetter : MonoBehaviour {
    Monster _mon;
    private void Start()
    {
        _mon = transform.parent.GetComponent<Monster>();
    }

    //콜라이더에 플레이어가 닿으면 부모 오브젝트의 상태를 Chase로 바꿔준다.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _mon.Chase(other.transform);
        }
    }
    
    //콜라이더에서 플레이어가 벗어나면 부모 오브젝트의 상태를 Idle로 바꿔준다.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _mon.stateMachine.SetState(MonStateType.Idle);
        }
    }
}