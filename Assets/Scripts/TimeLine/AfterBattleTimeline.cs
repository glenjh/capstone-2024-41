using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AfterBattleTimeline : MonoBehaviour
{
    public List<Monster> mons;
    public PlayableDirector cutScene;
    private int deathCnt;
    private bool played;

    void Awake()
    {
        deathCnt = 0;
        played = false;
    }

    void Update()
    {
        deathCnt = 0;
        for (int i = 0; i < mons.Count; i++)
        {
            if (mons[i].stateType == MonStateType.Die)
            {
                deathCnt++;
            }
        }

        if (deathCnt == mons.Count && !played)
        {
            cutScene.Play();
            played = true;
        }
        // Debug.Log((deathCnt));
        // Debug.Log((played));
    }

    public void NextStage()
    {
        MySceneManager.instance.ChangeScene("Stage 1");
    }
}
