using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_Jump : CharacterState
{
    protected Transform camTrans;
    protected Vector3 movementInput;
    protected Vector3 moveDirection;

    protected bool crouch;
    protected bool jump;

    float m_MovingTurnSpeed = 360;
    float m_StationaryTurnSpeed = 180;
    float m_JumpPower = 12f;
    float m_GravityMultiplier = 2f;
    float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    float m_MoveSpeedMultiplier = 1f;
    float m_AnimSpeedMultiplier = 1f;
    float m_GroundCheckDistance = 0.1f;

    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;


    public CharacterState_Jump(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    public override void Enter()
    {
        base.Enter();
        body.velocity = new Vector3(body.velocity.x, m_JumpPower, body.velocity.z);
        anim.applyRootMotion = false;
        m_GroundCheckDistance = 0.1f;
    }

    public override void Update()
    {
        base.Update();

        if(CheckGroundStatus())
        {
            stateMachine.SetState<CharacterState_GroundMovement_NoWeapon>(); // (stateMachine as CharacterStateMachine, "CharacterState_GroundMovement"));
        }
    }

    bool CheckGroundStatus()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(character.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            return true;
            //anim.applyRootMotion = true;
        }
        else
        {
            return false;
            //anim.applyRootMotion = false;
        }
    }

    public override void UpdateAnimator()
    {
        // update the animator parameters
        anim.SetBool("Crouch", false);
        anim.SetBool("OnGround", false);
        anim.SetFloat("Jump", body.velocity.y);

        anim.speed = 1;
    }


    public override void Exit()
    {
        base.Exit();
    }
}
