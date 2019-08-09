using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterState_MeleeAttackBase : CharacterState_AttackBase
{
    public CharacterState_MeleeAttackBase(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetTrigger("Attack");
        character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
    }

    public override void Exit()
    {
        base.Exit();
        character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
    }

    protected override void OnAttackAnimationEvent(string eventName)
    {
        if (eventName == "LeftCollider_On")
        {
            Debug.Log("打开左武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        }
        else if (eventName == "RightCollider_On")
        {
            Debug.Log("打开右武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
        }
        else if (eventName == "LeftCollider_Off")
        {
            Debug.Log("关闭左武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        }
        else if (eventName == "RightCollider_Off")
        {
            Debug.Log("关闭右武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
        }
        else if (eventName == "BothCollider_Off")
        {
            Debug.Log("关闭左右武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, false);
            character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, false);
        }
        else if (eventName == "BothCollider_On")
        {
            Debug.Log("打开左右武器碰撞体");
            character.ToggleWeaponCollider(ThirdPersonCharacterController.RIGHT, true);
            character.ToggleWeaponCollider(ThirdPersonCharacterController.LEFT, true);
        }
        else if (eventName == "ComboEnd")
        {
            Debug.Log("结束连招");
            stateMachine.SetState<CharacterState_GroundMovement_Axe>();
        }
    }
}
