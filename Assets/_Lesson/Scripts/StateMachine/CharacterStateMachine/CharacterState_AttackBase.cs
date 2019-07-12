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

        controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);

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
        canReceiveComboInput = attackCurve > comboStartTime && attackCurve < comboEndTime;
    }


    public override void Exit()
    {
        base.Exit();
        controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
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

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);

        if(eventName == "LeftCollider_On")
        {
            Debug.Log("打开左武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        }
        else if (eventName == "RightCollider_On")
        {
            Debug.Log("打开右武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
        }
        else if (eventName == "LeftCollider_Off")
        {
            Debug.Log("关闭左武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        }
        else if (eventName == "RightCollider_Off")
        {
            Debug.Log("关闭右武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        }
        else if(eventName == "BothCollider_Off")
        {
            Debug.Log("关闭左右武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        }
        else if (eventName == "BothCollider_On")
        {
            Debug.Log("打开左右武器碰撞体");
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
            controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        }
        else if (eventName == "ComboEnd")
        {
            Debug.Log("结束连招");
            stateMachine.SetState<CharacterState_GroundMovement_Axe>();
        }
    }
}
