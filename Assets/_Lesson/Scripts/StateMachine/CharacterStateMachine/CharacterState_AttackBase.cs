using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterState_AttackBase : CharacterState
{
    protected string attackButton;

    protected Type nextStateType;
    protected float comboStartTime;
    protected float comboEndTime;

    protected bool canReceiveComboInput = false;

    protected Transform camTrans;

    public CharacterState_AttackBase(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    protected void Init(Type nextStateType, float comboStartTime, float comboEndTime, string attackButton)
    {
        this.nextStateType = nextStateType;
        this.comboStartTime = comboStartTime;
        this.comboEndTime = comboEndTime;
        this.attackButton = attackButton;

        if (this.nextStateType != null)
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
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdateAnimator()
    {
        float attackCurve = anim.GetFloat("AttackCurve");
        canReceiveComboInput = attackCurve > comboStartTime && attackCurve < comboEndTime;
    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        base.OnInputButton(buttonName, eventType);

        if (buttonName == attackButton && eventType == ButtonEventType.Down)
        {
            if (nextStateType != null && canReceiveComboInput)
            {
                for (int i = 0; i < stateMachine.states.Length; i++)
                {
                    if (nextStateType == stateMachine.states[i].GetType())
                    {
                        Debug.Log("Entering:" + stateMachine.states[i].GetType());
                        stateMachine.SetState(stateMachine.states[i]);
                    }
                }
            }
        }
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        OnAttackAnimationEvent(eventName);
    }

    protected virtual void OnAttackAnimationEvent(string eventName)
    {

    }
}
