using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_GroundMovement_Gun : CharacterState
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


    public CharacterState_GroundMovement_Gun(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        camTrans = Camera.main.transform;
    }

    public override void Enter()
    {
        base.Enter();
        anim.applyRootMotion = true;
        anim.SetInteger("WeaponMode", 2);
    }

    public override void Update()
    {
        base.Update();

        Vector3 moveDirectionZ = Vector3.Scale(camTrans.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirectionX = camTrans.right;

        moveDirection = moveDirectionX * movementInput.x + moveDirectionZ * movementInput.z;
        //Debug.Log("jump:" + jump);
        Move(moveDirection);

        //jump = false;
        //crouch = false;
    }

    public void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = character.transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        //if (m_IsGrounded)
        //{
        //    HandleGroundedMovement();
        //}

        //ScaleCapsuleForCrouching(crouch);
        //PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        //UpdateAnimator(move);
    }

    void HandleGroundedMovement()
    {
        // check whether conditions are right to allow a jump:
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            body.velocity = new Vector3(body.velocity.x, m_JumpPower, body.velocity.z);
            m_IsGrounded = false;
            anim.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, moveDirection.z);
        character.transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(character.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            //anim.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            //anim.applyRootMotion = false;
        }
    }

    public override void UpdateAnimator()
    {
        // update the animator parameters
        anim.SetFloat("Forward", moveDirection.magnitude, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", false);
        anim.SetBool("OnGround", true);
        anim.SetFloat("Jump", 0);

        float runCycle =
            Mathf.Repeat(
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        if (m_IsGrounded)
        {
            anim.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && moveDirection.magnitude > 0)
        {
            anim.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            anim.speed = 1;
        }
    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void OnInputAxis(float h, float v)
    {
        base.OnInputAxis(h, v);
        movementInput.Set(h, 0, v);
    }

    public override void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        Debug.Log(buttonName);

        base.OnInputButton(buttonName, eventType);

        //jump = false;
        //crouch = false;

        if (eventType == ButtonEventType.Down)
        {
            if (buttonName == InputList.R1)
            {
                stateMachine.SetState<CharacterState_GroundMovement_NoWeapon>();
            }

            if (buttonName == InputList.FIRE1)
            {
                stateMachine.SetState<CharaacterState_GunAttack00>();
            }

            if (buttonName == InputList.FIRE2)
            {
                stateMachine.SetState<CharacterState_Jump>();
            }

            if (buttonName == InputList.FIRE3)
            {
                controller.SwitchTarget();
                //stateMachine.SetState<CharacterState_Crouch>();
            }

            if (buttonName == InputList.FIRE4)
            {
                controller.CancelLockOn();
            }
        }
    }
}
