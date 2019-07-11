using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterState_AttackBase : CharacterState
{
    protected Type nextStateType;
    protected float comboStartTime;
    protected float comboEndTime;

    protected bool canReceiveComboInput = false;

    protected Transform camTrans;

    public CharacterState_AttackBase(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    protected void Init(Type nextStateType, float comboStartTime, float comboEndTime)
    {
        this.nextStateType = nextStateType;
        this.comboStartTime = comboStartTime;
        this.comboEndTime = comboEndTime;

        if(this.nextStateType != null)
        {
            Debug.Log("NextState:" + this.nextStateType);
        }
        else
        {
            Debug.Log("NextState: NULL");
        }
    }

    public override void Enter()
    {
        Debug.Log("AttackState:" + GetType());
        base.Enter();
        anim.applyRootMotion = true;
        anim.SetTrigger("Attack");
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdateAnimator()
    {
        float attackCurve = anim.GetFloat("AttackCurve");

        Debug.Log(GetType() + "," + attackCurve);

        if (attackCurve > 0.99f)
        {
            stateMachine.SetState<CharacterState_GroundMovement>();
        }

        canReceiveComboInput = attackCurve > comboStartTime && attackCurve < comboEndTime;
    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        base.OnInputButton(buttonName, eventType);

        if (buttonName == InputList.FIRE1 && eventType == ButtonEventType.Down)
        {
            if (nextStateType != null && canReceiveComboInput)
            {
                for(int i = 0; i < stateMachine.states.Length; i++)
                {
                    if(nextStateType == stateMachine.states[i].GetType())
                    {
                        Debug.Log("Entering:" + stateMachine.states[i].GetType());
                        stateMachine.SetState(stateMachine.states[i]);
                    }
                }
            }
        }
    }
}
