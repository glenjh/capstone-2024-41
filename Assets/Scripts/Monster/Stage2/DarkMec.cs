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
    
    public override void Init(string name)
    {
        monsterName = name;
        //attackCollider.enabled = false;
        health = maxHealth;
        StateMachine.SetState(MonStateType.Idle);
        attackable = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override bool DecideAttack()
    {
        if (bullet.activeSelf)
            return false;
        return base.DecideAttack();
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