using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MoveInputAxis { Horizontal, Vertical, Both, None }

public class CharacterState : State
{
    protected ThirdPersonCharacterController character;
    protected Rigidbody body;
    protected Animator anim { get { return character.anim; } }

    public CharacterState() : base()
    {

    }

    public CharacterState(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        character = stateMachine.character;

        body = character.GetComponent<Rigidbody>();

        if(!anim)
        {
            Debug.LogError("Cannot find Animator on character:" + character.name);
        }

        if (!body)
        {
            Debug.LogError("Cannot find Rigidbody on character:" + character.name);
        }
    }

    public override void Update()
    {
        base.Update();

        if(!character.isAlive)
        {
            stateMachine.SetState<CharacterState_Die>();
        }
    }
}

public class CharacterStateMachine : StateMachine
{
    public MoveInputAxis moveInputAxis = MoveInputAxis.Both;

    public ThirdPersonCharacterController character;

    public CharacterStateMachine(ThirdPersonCharacterController character)
    {
        this.character = character;
        this.states = CreateAllStates<CharacterState>();
        for(int i = 0; i < states.Length; i++)
        {
            Debug.Log(states[i].GetType());

            if(states[i].GetType() == typeof(CharacterState_GroundMovement_NoWeapon))
            {
                currentState = states[i];
            }
        }
        //currentState = new CharacterState_GroundMovement(this, "GroundMovement");
    }

    public void OnInputAxis(float h, float v)
    {
        switch (moveInputAxis)
        {
            case MoveInputAxis.Horizontal:
                v = 0;
                break;
            case MoveInputAxis.Vertical:
                h = 0;
                break;
            case MoveInputAxis.Both:
                break;
            case MoveInputAxis.None:
                v = 0;
                h = 0;
                break;
        }

        currentState.OnInputAxis(h, v);
    }

    public virtual void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        currentState.OnInputButton(buttonName, eventType);
    }
}
