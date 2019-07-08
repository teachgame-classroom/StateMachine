using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Jump : State
{
    private CubeController cube;

    public State_Jump(StateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        cube = stateMachine.brain.cube;
    }

    public override void Update()
    {
        base.Update();
        cube.Jump();
    }
}


public class State_Move : State
{
    private CubeController cube;

    public State_Move(StateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        cube = stateMachine.brain.cube;
    }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Debug.Log(string.Format("h:{0}, v:{1}", h, v));

        cube.Move(h, v);
    }
}

public class State
{
    public StateMachine stateMachine;
    public string stateName;

    public State(StateMachine stateMachine, string stateName)
    {
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Update()
    {
        Debug.Log("正在运行状态：" + stateName);
    }

    public virtual void Enter()
    {
        Debug.Log("进入状态：" + stateName);
    }

    public virtual void Exit()
    {
        Debug.Log("离开状态：" + stateName);
    }
}

public class StateMachine
{
    public Brain brain;

    public State[] states;
    public State currentState;

    public StateMachine(Brain brain)
    {
        this.brain = brain;
        SetState(new State(this, "Idle"));
    }

    public StateMachine(Brain brain, string[] stateNames)
    {
        this.brain = brain;

        if(stateNames.Length > 0)
        {
            states = new State[stateNames.Length];

            for(int i = 0; i < stateNames.Length; i++)
            {
                if(i == 1)
                {
                    states[i] = new State_Move(this, stateNames[i]);
                }
                else if(i == 2)
                {
                    states[i] = new State_Jump(this, stateNames[i]);
                }
                else
                {
                    states[i] = new State(this, stateNames[i]);
                }
            }

            currentState = states[0];
        }
        else
        {
            SetState(new State(this, "Idle"));
        }
    }

    public void Update()
    {
        currentState.Update();
    }

    public void SetState(State newState)
    {
        if(currentState != null)
        {
            State oldState = currentState;

            oldState.Exit();
        }

        newState.Enter();
        currentState = newState;
    }

    public void SetState(int idx)
    {
        State newState = states[idx];

        SetState(newState);
    }
}
