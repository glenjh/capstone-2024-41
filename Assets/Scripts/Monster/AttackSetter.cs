using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSetter : MonoBehaviour {
    Boss _boss;
    private void Start()
    {
        _boss = transform.parent.GetComponent<Boss>();
    }

    //콜라이더에 플레이어가 닿으면 부모 오브젝트의 상태를 Attack으로 바꿔준다.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _boss.StateMachine.SetState(BossStateType.Attack);
        }
    }
}