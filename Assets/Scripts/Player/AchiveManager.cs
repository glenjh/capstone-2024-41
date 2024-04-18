using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public delegate void BossDefeatedEvent(string bossType);
    public static event BossDefeatedEvent onBossDefeated;

    public static void BossDefeated(string bosstype)
    {
        if (onBossDefeated != null)
        {
            onBossDefeated(bosstype);
        }
    }
}
