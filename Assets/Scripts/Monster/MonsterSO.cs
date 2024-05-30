using UnityEngine;


[CreateAssetMenu(fileName = "Monster", menuName = "Monster/MonsterSO", order = 0)]
public class MonsterSO : ScriptableObject
{
    public int maxHealth;
    public int attackDamage;
    public float attackRange;
    public float chaseRange;
    public MoveType moveType;
    public float attackDelay;
    public float maxMoveSpeed;
}
