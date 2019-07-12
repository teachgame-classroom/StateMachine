using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class ThirdPersonCharacterController : MonoBehaviour
{
    public const int LEFT = 0;
    public const int RIGHT = 1;

    public bool isPlayer;
    protected CharacterStateMachine stateMachine;
    protected ThirdPersonCharacter character;

    protected Transform[] weapons = new Transform[2];

    protected Transform[] weaponLocation_L = new Transform[2];
    protected Transform[] weaponLocation_R = new Transform[2];

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<ThirdPersonCharacter>();
        stateMachine = new CharacterStateMachine(character);


        SlotMarker[] slots = GetComponentsInChildren<SlotMarker>();

        foreach(SlotMarker slot in slots)
        {
            Transform slotTrans = slot.transform;

            switch(slot.slotType)
            {
                case SlotType.LeftBack:
                    weaponLocation_L[0] = slotTrans;
                    break;
                case SlotType.RightBack:
                    weaponLocation_R[0] = slotTrans;
                    break;
                case SlotType.LeftHand:
                    weaponLocation_L[1] = slotTrans;
                    break;
                case SlotType.RightHand:
                    weaponLocation_R[1] = slotTrans;
                    break;
                case SlotType.LeftWeapon:
                    weapons[LEFT] = slotTrans;
                    break;
                case SlotType.RightWeapon:
                    weapons[RIGHT] = slotTrans;
                    break;
                default:
                    break;
            }
        }

        if (isPlayer)
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

    public void ToggleWeaponCollider(int weaponIdx, bool setActive)
    {
        weapons[weaponIdx].GetComponent<Collider>().enabled = setActive;
    }

    public void OnAnimationEvent(string eventName)
    {
        stateMachine.OnAnimationEvent(eventName);
    }

    public void AttachWeapon(int weaponPosIdx)
    {
        weapons[LEFT].SetParent(weaponLocation_L[weaponPosIdx]);
        weapons[RIGHT].SetParent(weaponLocation_R[weaponPosIdx]);

        foreach(Transform trans in weapons)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
    }
}
