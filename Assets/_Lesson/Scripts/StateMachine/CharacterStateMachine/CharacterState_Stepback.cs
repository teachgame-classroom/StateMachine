using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_StepBack : CharacterState
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


    public CharacterState_StepBack(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    public override void Enter()
    {
        base.Enter();
        anim.applyRootMotion = true;
        anim.SetBool("StepBack", true);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdateAnimator()
    {
        float stepCurve = anim.GetFloat("StepCurve");
        Debug.Log(stepCurve);

        if(stepCurve > 0.9f)
        {
            stateMachine.SetState<CharacterState_GroundMovement>();
        }
    }


    public override void Exit()
    {
        base.Exit();
        anim.SetBool("StepBack", false);
    }
}
