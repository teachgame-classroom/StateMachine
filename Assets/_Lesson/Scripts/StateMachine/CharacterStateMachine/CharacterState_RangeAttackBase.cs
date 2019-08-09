using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CharaacterState_GunAttack00 : CharacterState_RangeAttackBase
{
    public CharaacterState_GunAttack00(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharaacterState_GunAttack01), 0f, 0.95f, InputList.FIRE1);
    }
}

public class CharaacterState_GunAttack01 : CharacterState_RangeAttackBase
{
    public CharaacterState_GunAttack01(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharaacterState_GunAttack02), 0f, 0.95f, InputList.FIRE1);
    }
}

public class CharaacterState_GunAttack02 : CharacterState_RangeAttackBase
{
    public CharaacterState_GunAttack02(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharaacterState_GunAttack03), 0.0f, 0.95f, InputList.FIRE1);
    }
}

public class CharaacterState_GunAttack03 : CharacterState_RangeAttackBase
{
    public CharaacterState_GunAttack03(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(null, 0f, 0.95f, InputList.FIRE1);
    }
}

public class CharacterState_RangeAttackBase : CharacterState_AttackBase
{
    protected bool isTriggerSet;

    public CharacterState_RangeAttackBase(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        isTriggerSet = false;

        //if (!controller.hasTarget)
        //{
        //    SetAttackTrigger();
        //}
    }

    private void SetAttackTrigger()
    {
        anim.SetTrigger("Attack");
        isTriggerSet = true;
    }

    public override void Update()
    {
        base.Update();

        if(!isTriggerSet)
        {
            if (character.hasTarget)
            {
                if (character.IsAimAtTarget() == false)
                {
                    character.RotateTowardTarget();
                }
                else
                {
                    SetAttackTrigger();
                }
            }
            else
            {
                if (character.IsAimAtTarget() == false)
                {
                    character.RotateTowardAimDirection();
                }
                else
                {
                    SetAttackTrigger();
                }
            }
        }

        //Debug.LogWarning("Attack02 Combo:" + canReceiveComboInput);

        float attackCurve = anim.GetFloat("AttackCurve");

        //Debug.Log("Attack02 AttackCurve:" + attackCurve);

        Debug.DrawLine(character.transform.position, character.transform.position + character.transform.forward * 5, Color.red);
        Debug.DrawLine(character.transform.position, character.targetDirection * 5, Color.green);
    }


    protected override void OnAttackAnimationEvent(string eventName)
    {
        if(eventName == "WpnPullTrigerLeft")
        {
            character.Shoot(ThirdPersonCharacterController.LEFT);
        }
        else if (eventName == "WpnPullTrigerRight")
        {
            character.Shoot(ThirdPersonCharacterController.RIGHT);
        }
        else if (eventName == "ComboEnd")
        {
            Debug.Log("结束连招");
            stateMachine.SetState<CharacterState_GroundMovement_Gun>();
        }

        //if (eventName == "LeftCollider_On")
        //{
        //    Debug.Log("打开左武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        //}
        //else if (eventName == "RightCollider_On")
        //{
        //    Debug.Log("打开右武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
        //}
        //else if (eventName == "LeftCollider_Off")
        //{
        //    Debug.Log("关闭左武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        //}
        //else if (eventName == "RightCollider_Off")
        //{
        //    Debug.Log("关闭右武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        //}
        //else if (eventName == "BothCollider_Off")
        //{
        //    Debug.Log("关闭左右武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        //}
        //else if (eventName == "BothCollider_On")
        //{
        //    Debug.Log("打开左右武器碰撞体");
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
        //    controller.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        //}
        //else if (eventName == "ComboEnd")
        //{
        //    Debug.Log("结束连招");
        //    stateMachine.SetState<CharacterState_GroundMovement_Axe>();
        //}
    }
}
