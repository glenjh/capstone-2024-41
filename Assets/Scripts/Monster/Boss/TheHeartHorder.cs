using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHeartHorder : Boss
{
    protected override void Start()
    {
        base.Start();
        
        StateMachine.SetState(BossStateType.Sleep);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            WakeUp();
           
        if(Input.GetKeyDown(KeyCode.M))
            StateMachine.SetState(BossStateType.Dead);
    }
}
