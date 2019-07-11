using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

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

    public bool canTransitToSelf;

    public State()
    {

    }

    public State(StateMachine stateMachine, string stateName)
    {
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Update()
    {
        //Debug.Log("正在运行状态：" + GetType());
    }

    public virtual void Enter()
    {
        Debug.Log("进入状态：" + GetType());
    }

    public virtual void Exit()
    {
        Debug.Log("离开状态：" + GetType());
    }

    public virtual void OnInputAxis(float h, float v)
    {

    }

    public virtual void OnInputButton(string buttonName, ButtonEventType eventType)
    {

    }

    public virtual void UpdateAnimator()
    {

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
        //SetState(new State(this, "Idle"));
    }

    public StateMachine()
    {

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
            //SetState(new State(this, "Idle"));
        }
    }

    public T[] CreateAllStates<T>() where T : State
    {
        List<Type> typeList = new List<Type>();

        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = assembly.GetTypes();

            for(int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(typeof(T)))
                {
                    typeList.Add(types[i]);
                }
            }
        }

        T[] states = new T[typeList.Count];

        for (int i = 0; i < states.Length; i++)
        {
            Debug.Log(typeList[i]);

            foreach (ConstructorInfo ci in typeList[i].GetConstructors())
            {
                ParameterInfo[] pis = ci.GetParameters();

                Debug.Log("Para Count:" + pis.Length);

                foreach(ParameterInfo pi in pis)
                {
                    Debug.Log("Parameter Name:" + pi.Name);
                }
            }

            states[i] = typeList[i].GetConstructors()[0].Invoke(new object[2] { this, "State"}) as T;
        }

        return states;
    }

    public void Update()
    {
        currentState.Update();
        currentState.UpdateAnimator();
    }

    public void SetState(State newState)
    {
        if(currentState != null)
        {
            State oldState = currentState;

            if (oldState.GetType() == newState.GetType() && oldState.canTransitToSelf == false) return;

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

    public void SetState<T>() where T : State
    {
        for(int i = 0; i < states.Length; i++)
        {
            if(states[i].GetType() == typeof(T))
            {
                State newState = states[i];
                SetState(newState);
                return;
            }
        }
    }
}
