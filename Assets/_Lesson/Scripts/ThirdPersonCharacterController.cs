using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class ThirdPersonCharacterController : MonoBehaviour
{
    public bool isPlayer;
    protected CharacterStateMachine stateMachine;
    protected ThirdPersonCharacter character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<ThirdPersonCharacter>();
        stateMachine = new CharacterStateMachine(character);
        if(isPlayer)
        {
            InputManager.instance.InputEvent_Axis += OnInputAxis;
            InputManager.instance.InputEvent_Button += OnInputButton;
        }
    }

    private void OnInputButton(string buttonName, ButtonEventType eventType)
    {
        stateMachine.OnInputButton(buttonName, eventType);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    void OnInputAxis(float h, float v)
    {
        stateMachine.OnInputAxis(h, v);
    }
}
