using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : BaseState
{
    public override void Act()
    {
        EventMgr.Instance.EventTrigger("PlayerMove");
    }
}public class Player_Idle : BaseState
{
    public override void Act()
    {
        EventMgr.Instance.EventTrigger("PlayerIdle");
    }
}

