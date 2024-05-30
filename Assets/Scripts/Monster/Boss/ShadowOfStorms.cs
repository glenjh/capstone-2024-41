using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class ShadowOfStorms : Boss
{
    [Serializable]
    public class SOSMonster
    {
        public string monsterNames;
        public Transform spawnPoints;
    }
    
    private List<Action> attackActions = new List<Action>();
    public SOSMonster[] sosMonsters;
    public List<Monster> _monster;
    [SerializeField]private GameObject spawnEffect; 
    [SerializeField]private GameObject healEffect;
    [SerializeField]private GameObject Beam;
    [SerializeField]private Bullet flashBullet;
    int currentAttackIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Start()
    {
        base.Start();
        
        StateMachine.SetState(BossStateType.Sleep);
        
        attackActions.Add(Heal);
        attackActions.Add(SimpleMeleeAttack);
        attackActions.Add(ComboMeleeAttack);
        attackActions.Add(ChargeBeamAttack);
        attackActions.Add(FlashAttack);
    }
    
    public override void AbleMove()
    {
        base.AbleMove();
        
        for (int i = 0; i < _monster.Count; i++)
        {
            if (_monster[i] == null) continue;
            _monster[i].EnableChase();
        }
    }

    public override void OnAttack()
    {
        //리스트내에 있는 함수를 임의로 실행
        attackActions[currentAttackIndex]();
        currentAttackIndex = (currentAttackIndex + 1) % attackActions.Count;
    }

    public override void Dead()
    {
        for (int i = 0; i < _monster.Count; i++)
        {
            if (_monster[i] == null) continue;
            _monster[i].TakeHit(_monster[i].monsterSO.maxHealth+1,player);
        }
        base.Dead();
    }

    #region Attack

    private void Heal()
    {
        animator.SetInteger("AttackType", 0);
    }
    
    public void HealAndRecovery()
    {
        for(int i = 0; i < _monster.Count; i++)
        {
            if (_monster[i] == null) continue;
            if (_monster[i].health <= 0)
            {
                var pos = _monster[i].transform.position;
                _monster[i] = PoolManager.Instance.GetFromPool<Monster>(sosMonsters[i].monsterNames);
                _monster[i].transform.position = pos;
                _monster[i].Init(sosMonsters[i].monsterNames);
                _monster[i].health = 50;
                
                Instantiate(spawnEffect, pos, Quaternion.identity);
                continue;
            }
            _monster[i].health += 50;
            Instantiate(healEffect, _monster[i].transform.position, Quaternion.identity);
            if (_monster[i].health > _monster[i].monsterSO.maxHealth)
                _monster[i].health = _monster[i].monsterSO.maxHealth;
        }
        
        health += 10;
        Instantiate(healEffect, transform.position, Quaternion.identity);
        if (health > maxHealth)
            health = maxHealth;
    }
    
    private void SimpleMeleeAttack()
    {
        animator.SetInteger("AttackType", 2);
    }
    
    private void ComboMeleeAttack()
    {
        animator.SetInteger("AttackType", 1);
    }

    private void ChargeBeamAttack()
    {
        animator.SetInteger("AttackType", 3);
    }
    
    private void FlashAttack()
    {
        animator.SetInteger("AttackType", 4);
    }
    
    public void OnFlashAttack()
    {
        //6발의 플래시 불릿을 생성후 발사
        for (int i = 0; i < 12; i++)
        {
            var bullet = Instantiate(flashBullet.gameObject).GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            bullet.SetOnHitCallBack(Flash);
            
            Vector3 dir = Quaternion.Euler(0, 0, 30 * i) * Vector3.right;
            bullet.Fire(dir, 15);
        }
    }
    
    private void Flash(Transform target)
    {
        transform.position = target.position;
        OnShakeCamera();
        OnIdle();
    }

    public void OnBeamAttack() => StartCoroutine(BeamAttack());
    
    IEnumerator BeamAttack()
    {
        Transform origin = transform;
        Instantiate(Beam, origin.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            OnShakeCamera();
            Instantiate(Beam, new Vector3(origin.position.x + 3f*(i+1), transform.position.y, transform.position.z), Quaternion.identity);
            Instantiate(Beam, new Vector3(origin.position.x - 3f*(i+1), transform.position.y, transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void SpawnMonster()
    {
        if(sosMonsters.Length == 0) return;
        
        OnShakeCamera();
        foreach (var VARIABLE in sosMonsters)
        {
            _monster.Add(PoolManager.Instance.GetFromPool<Monster>(VARIABLE.monsterNames));
            _monster[_monster.Count - 1].transform.position = VARIABLE.spawnPoints.position;
            _monster[_monster.Count - 1].Init(VARIABLE.monsterNames, true);
            _monster[_monster.Count - 1].player = player;
            Instantiate(spawnEffect, VARIABLE.spawnPoints.position, Quaternion.identity);
        }
    }

    #endregion
}
