using UnityEngine;

public class AttackDummy : MonoBehaviour, IDamageAble
{
    Animator animator;
    BezierMissileShooter missileShooter;
    void Start()
    {
        animator = GetComponent<Animator>();
        missileShooter = GetComponent<BezierMissileShooter>();
    }
    public void TakeHit(int damage, Transform attacker)
    {
        EffectSystem effect;
        effect = PoolManager.Instance.GetFromPool<EffectSystem>("MonsterHitEffect");
        animator.SetTrigger("Hit");
        if(missileShooter!=null)
            missileShooter.Fire();
        effect.transform.position = transform.position;
    }
}