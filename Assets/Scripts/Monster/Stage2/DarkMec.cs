using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMec : Monster {
    [SerializeField] Transform originBulletPos;
    [SerializeField] Bullet bullet;
    [SerializeField] float bulletSpeed;
    
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start() {
        base.Start();
    }
    
    public override void Init()
    {
        health = maxHealth;
        StateMachine.SetState(MonStateType.Idle);
    }

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
        //공격 범위 내로 플레이어가 들어오면 공격
        RaycastHit2D rayRange = Physics2D.Raycast(rb.position, (moveSpeed >0 ? Vector3.right: Vector3.left), attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider != null && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            if(bullet.gameObject.activeSelf == false)
                StateMachine.SetState(MonStateType.Attack);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
    
    public override void OnAttack()
    {
        bullet.gameObject.SetActive(true);
        // 총알을 생성하고 발사할 방향 설정
        Vector3 bulletDirection = (player.position - originBulletPos.position).normalized;
        bullet.transform.position = originBulletPos.position;
        bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg);
        
        // 총알 발사
        bullet.Fire(bulletDirection, bulletSpeed);
    }
}