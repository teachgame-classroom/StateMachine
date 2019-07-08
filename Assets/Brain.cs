using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public CubeController cube;
    private StateMachine stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        cube = GetComponent<CubeController>();
        stateMachine = new StateMachine(this, new string[] { "Idle", "Move", "Jump" });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            stateMachine.SetState(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            stateMachine.SetState(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stateMachine.SetState(2);
        }

        stateMachine.Update();

        if(Input.GetKeyDown(KeyCode.J))
        {
            stateMachine.SetState(2);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        stateMachine.SetState(1);
    }
}
