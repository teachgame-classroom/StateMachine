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

    protected Transform[] weaponShotPos = new Transform[2];

    public AudioClip fireClip;

    public LineRenderer[] gunTrailEffects;

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
                case SlotType.LeftShotPos:
                    weaponShotPos[LEFT] = slotTrans;
                    break;
                case SlotType.RightShotPos:
                    weaponShotPos[RIGHT] = slotTrans;
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

    public void WpnPullTrigerRight(int idx)
    {
        if(idx == 0)
        {
            stateMachine.OnAnimationEvent("WpnPullTrigerRight");
        }
    }

    public void WpnPullTrigerLeft(int idx)
    {
        if (idx == 0)
        {
            stateMachine.OnAnimationEvent("WpnPullTrigerLeft");
        }
    }

    Vector3[] hitPos = new Vector3[2];

    public void Shoot(int weaponIdx)
    {
        for(int i = 0; i < hitPos.Length; i++)
        {
            hitPos[i] = Vector3.zero;
        }

        RaycastHit hit;
        Transform weaponTrans = weaponShotPos[weaponIdx];
        Vector3 endPoint = weaponTrans.position + transform.forward * 100;

        if (Physics.Raycast(weaponTrans.position, transform.forward, out hit, 100))
        {
            ThirdPersonCharacter target = hit.transform.GetComponent<ThirdPersonCharacter>();

            hitPos[weaponIdx] = hit.point;

            gunTrailEffects[weaponIdx].SetPosition(1, hit.point);

            endPoint = hit.point;

            if (target)
            {
                target.Hurt(20);
            }
        }

        StartCoroutine(GunTrail(weaponIdx, weaponTrans.position, endPoint));

        //Debug.DrawLine(weaponTrans.position, weaponTrans.position + transform.forward * 100);

        AudioSource.PlayClipAtPoint(fireClip, transform.position);
    }

    IEnumerator GunTrail(int idx, Vector3 startPos, Vector3 endPos)
    {
        gunTrailEffects[idx].SetPosition(0, startPos);
        gunTrailEffects[idx].SetPosition(1, startPos);

        gunTrailEffects[idx].gameObject.SetActive(true);

        Vector3 trailEndPoint = startPos;

        float t = 0;

        while(trailEndPoint != endPos)
        {
            trailEndPoint = Vector3.MoveTowards(trailEndPoint, endPos, 500 * Time.deltaTime);
            gunTrailEffects[idx].SetPosition(1, trailEndPoint);

            t += Time.deltaTime * 5;
            float alpha = Mathf.Lerp(1, 0, t);

            gunTrailEffects[idx].material.SetColor("_TintColor", new Color(1, 1, 1, alpha));
            yield return null;
        }

        gunTrailEffects[idx].gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < hitPos.Length; i++)
        {
            if(hitPos[i] != Vector3.zero)
            {
                Gizmos.DrawCube(hitPos[i], Vector3.one * 0.1f);
            }
        }
    }
}
