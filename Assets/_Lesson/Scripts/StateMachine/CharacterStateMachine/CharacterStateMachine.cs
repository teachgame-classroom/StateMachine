using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class CharacterState : State
{
    protected ThirdPersonCharacterController controller;
    protected ThirdPersonCharacter character;
    protected Rigidbody body;
    protected Animator anim;

    public CharacterState() : base()
    {

    }

    public CharacterState(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        character = stateMachine.character;
        controller = stateMachine.controller;

        anim = character.GetComponent<Animator>();
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
    public ThirdPersonCharacter character;
    public ThirdPersonCharacterController controller;

    public CharacterStateMachine(ThirdPersonCharacter character)
    {
        this.character = character;
        this.controller = character.GetComponent<ThirdPersonCharacterController>();
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
        currentState.OnInputAxis(h, v);
    }

    public virtual void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        currentState.OnInputButton(buttonName, eventType);
    }
}
