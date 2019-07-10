using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class CharacterState : State
{
    protected ThirdPersonCharacter owner;
    protected Rigidbody body;
    protected Animator anim;

    public CharacterState() : base()
    {

    }

    public CharacterState(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        owner = stateMachine.owner;
        anim = owner.GetComponent<Animator>();
        body = owner.GetComponent<Rigidbody>();

        if(!anim)
        {
            Debug.LogError("Cannot find Animator on character:" + owner.name);
        }

        if (!body)
        {
            Debug.LogError("Cannot find Rigidbody on character:" + owner.name);
        }
    }
}

public class CharacterStateMachine : StateMachine
{
    public ThirdPersonCharacter owner;

    public CharacterStateMachine(ThirdPersonCharacter owner)
    {
        this.owner = owner;
        this.states = CreateAllStates<CharacterState>();
        for(int i = 0; i < states.Length; i++)
        {
            Debug.Log(states[i].GetType());

            if(states[i].GetType() == typeof(CharacterState_GroundMovement))
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
