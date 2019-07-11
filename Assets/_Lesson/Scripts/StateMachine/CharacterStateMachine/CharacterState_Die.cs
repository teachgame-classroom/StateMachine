using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_Die : CharacterState
{
    protected Transform camTrans;

    public CharacterState_Die(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    public override void Enter()
    {
        base.Enter();
        anim.applyRootMotion = true;
        anim.SetTrigger("Die");
    }
}
