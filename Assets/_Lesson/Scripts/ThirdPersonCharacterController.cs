using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class ThirdPersonCharacterController : MonoBehaviour, ICameraFollowable, IItemOwner
{
    public Transform[] outfits;
    private int currentOutfitIdx = 0;

    private int outfitIdx = 0;

    public const int LEFT = 0;
    public const int RIGHT = 1;

    public bool isPlayer;
    protected CharacterStateMachine stateMachine;
    protected ThirdPersonCharacter character;

    public Spec baseSpec;
    public List<Spec> extraSpec = new List<Spec>();
    public Spec finalSpec;

    List<Equipment> equipments = new List<Equipment>();

    public int currentHp { get; private set; }

    public GameObject hurtEffect;
    public bool isAlive { get { return currentHp > 0; } }


    public bool hasTarget { get { return targetIdx >= 0 && targetList[targetIdx] != null; } }


    public ThirdPersonCharacterController target
    {
        get
        {
            if (hasTarget)
            {
                return targetList[targetIdx];
            }
            else
            {
                return null;
            }
        }
    }

    protected List<ThirdPersonCharacterController> targetList = new List<ThirdPersonCharacterController>();
    protected int targetIdx = -1;

    public Vector3 targetDirection
    {
        get
        {
            if (hasTarget)
            {
                return (target.transform.position - transform.position).normalized;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    public CameraFollow cameraFollow
    {
        get;
        set;
    }

    protected Transform[] currentWeapons = new Transform[2];

    protected Transform[] weapons = new Transform[6];

    protected Transform[] weaponLocation_L = new Transform[2];
    protected Transform[] weaponLocation_R = new Transform[2];

    protected Transform[] weaponShotPos = new Transform[2];

    public AudioClip fireClip;

    public LineRenderer[] gunTrailEffects;

    public const int BACKPACK_SIZE = 40;
    public const int EQUIPMENT_SIZE = 12;
    public Inventory inventory;

    public Item[] testItems = new Item[8];
    private int testItemIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = baseSpec.hp;

        inventory = new Inventory(BACKPACK_SIZE, EQUIPMENT_SIZE, this);

        character = GetComponent<ThirdPersonCharacter>();
        stateMachine = new CharacterStateMachine(character);


        SlotMarker[] slots = GetComponentsInChildren<SlotMarker>(true);

        foreach (SlotMarker slot in slots)
        {
            Transform slotTrans = slot.transform;

            switch (slot.slotType)
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
                case SlotType.LeftGunAxe:
                    weapons[(int)WeaponType.GunAxe * 2] = slotTrans;
                    break;
                case SlotType.RightGunAxe:
                    weapons[(int)WeaponType.GunAxe * 2 + 1] = slotTrans;
                    break;
                case SlotType.LeftAxe:
                    weapons[(int)WeaponType.Axe * 2] = slotTrans;
                    break;
                case SlotType.RightAxe:
                    weapons[(int)WeaponType.Axe * 2 + 1] = slotTrans;
                    break;
                case SlotType.LeftGun:
                    weapons[(int)WeaponType.Gun * 2] = slotTrans;
                    break;
                case SlotType.RightGun:
                    weapons[(int)WeaponType.Gun * 2 + 1] = slotTrans;
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

            ThirdPersonCharacterController[] controllers = FindObjectsOfType<ThirdPersonCharacterController>();

            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].gameObject.name != "Player")
                {
                    targetList.Add(controllers[i]);
                }
            }

            UIManager.instance.InventorySlotClickEvent += inventory.OnInventorySlotClick;
            UIManager.instance.InventorySlotHoverEvent += inventory.OnInventorySlotHover;
            UIManager.instance.InventoryBeginDragEvent += inventory.OnInventoryBeginDrag;
            UIManager.instance.InventoryDropEvent += inventory.OnInventoryDrop;
            UIManager.instance.InventoryDropEmptyEvent += inventory.OnInventoryEmptyDrop;
            inventory.InventoryChangeEvent += UIManager.instance.OnInventoryChange;
            inventory.InventoryHasItemEvent += UIManager.instance.OnHasItemNotify;

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

        if(isPlayer)
        {
            transform.position = outfits[currentOutfitIdx].position;
            transform.rotation = outfits[currentOutfitIdx].rotation;

            for(int i = 0; i < outfits.Length; i++)
            {
                outfits[i].localPosition = Vector3.zero;
                outfits[i].localRotation = Quaternion.identity;
            }

            UpdateCrossHair();

            if(Input.GetKeyDown(KeyCode.T))
            {
                outfitIdx++;
                if(outfitIdx > 1)
                {
                    outfitIdx = 0;
                }

                SwitchOutfit(outfitIdx);
            }

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeapon(WeaponType.GunAxe);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(WeaponType.Axe);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeapon(WeaponType.Gun);
            }
        }

        //SwitchTarget();
    }

    void TestPutInItem(int idx)
    {
        inventory.PutInItem(testItems[idx]);
    }

    void OnInputAxis(float h, float v)
    {
        stateMachine.OnInputAxis(h, v);
    }

    public void ToggleWeaponCollider(int weaponIdx, bool setActive)
    {
        currentWeapons[weaponIdx].GetComponent<Collider>().enabled = setActive;
    }

    public void OnAnimationEvent(string eventName)
    {
        stateMachine.OnAnimationEvent(eventName);
    }

    public void AttachWeapon(int weaponPosIdx)
    {
        currentWeapons[LEFT].SetParent(weaponLocation_L[weaponPosIdx]);
        currentWeapons[RIGHT].SetParent(weaponLocation_R[weaponPosIdx]);

        foreach (Transform trans in currentWeapons)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
    }

    public void WpnPullTrigerRight(int idx)
    {
        if (idx == 0)
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

    public bool IsAimAtTarget()
    {
        float dot = 0;

        if (target)
        {
            dot = Vector3.Dot(transform.forward, targetDirection);
        }
        else
        {
            Vector3 cameraDirectionXZ = cameraFollow.transform.forward;
            cameraDirectionXZ.Set(cameraDirectionXZ.x, 0, cameraDirectionXZ.z);

            cameraDirectionXZ.Normalize();

            dot = Vector3.Dot(transform.forward, cameraDirectionXZ);
        }

        if (dot > 0.999f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SwitchTarget()
    {
        int idx = NextIndex();

        while (targetList[idx] == null || targetList[idx].isAlive == false)
        {
            idx = NextIndex();
        }

        targetIdx = idx;

        cameraFollow.SetLockOnTarget(target.transform);
    }

    public void CancelLockOn()
    {
        targetIdx = -1;
        cameraFollow.SetLockOnTarget(null);
    }

    private void UpdateCrossHair()
    {
        if(target)
        {
            UIManager.instance.ShowCrosshair(target.transform.position + Vector3.up);
        }
        else
        {
            UIManager.instance.CenterCrosshair();
        }
    }

    private int NextIndex()
    {
        int idx = targetIdx + 1;

        if (idx >= targetList.Count)
        {
            idx = 0;
        }

        return idx;
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
        Vector3 shotDirection = transform.forward;

        if (IsAimAtTarget())
        {
            if (target)
            {
                Vector3 aimPos = target.transform.position + Vector3.up;
                Vector3 offset = Random.insideUnitSphere * 0.2f;
                Vector3 finalAimPos = aimPos + offset;
                shotDirection = (finalAimPos - weaponTrans.position).normalized;
            }
            else
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
                {
                    Vector3 aimPos = hit.point;
                    Debug.DrawLine(weaponTrans.position, aimPos, Color.yellow, 10f);
                    shotDirection = (aimPos - weaponTrans.position).normalized;
                }
                else
                {
                    Vector3 aimPos = Camera.main.transform.position + Camera.main.transform.forward * 100;
                    shotDirection = (aimPos - weaponTrans.position).normalized;
                }
            }
        }

        if (Physics.Raycast(weaponTrans.position, shotDirection, out hit, 100))
        {
            ThirdPersonCharacterController target = hit.transform.GetComponent<ThirdPersonCharacterController>();

            hitPos[weaponIdx] = hit.point;

            gunTrailEffects[weaponIdx].SetPosition(1, hit.point);

            endPoint = hit.point;

            if (target && !target.isPlayer)
            {
                target.Hurt(finalSpec.atk);
            }
        }

        StartCoroutine(GunTrail(weaponIdx, weaponTrans.position, endPoint));

        //Debug.DrawLine(weaponTrans.position, weaponTrans.position + transform.forward * 100);

        AudioSource.PlayClipAtPoint(fireClip, transform.position);
    }

    public void RotateTowardTarget()
    {
        if(target)
        {
            Quaternion targetRot = Quaternion.FromToRotation(Vector3.forward, targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720 * Time.deltaTime);
        }
    }

    public void RotateTowardAimDirection()
    {
        Vector3 aimDirection = cameraFollow.transform.forward;

        aimDirection.Scale(new Vector3(1,0,1));
        aimDirection.Normalize();

        Quaternion targetRot = Quaternion.FromToRotation(Vector3.forward, aimDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720 * Time.deltaTime);
    }

    public void SetMoveInputAxis(MoveInputAxis moveInputAxis)
    {
        stateMachine.moveInputAxis = moveInputAxis;
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

    public bool UseItem(Item item)
    {
        Potion potion = item as Potion;

        if(potion != null)
        {
            ChangeSpec(potion.spec, potion.isEffectPermanent, potion.effectDuration);
        }

        return true;
    }

    public void CaculatFinalSpec()
    {
        Spec spec = baseSpec;

        for(int i = 0; i < equipments.Count; i++)
        {
            spec = Spec.Add(spec, equipments[i].spec);
        }

        finalSpec = spec;
        Debug.Log("Final Spec:" + finalSpec.ToString());
    }

    public bool Equip(Equipment equipment)
    {
        Weapon weapon = equipment as Weapon;

        if(weapon != null)
        {
            for(int i = 0; i < equipments.Count; i++)
            {
                Weapon equipedWeapon = equipments[i] as Weapon;
                if (equipedWeapon != null)
                {
                    equipments.RemoveAt(i);
                }
            }
            EquipWeapon(weapon.weaponType);
        }

        equipments.Add(equipment);

        CaculatFinalSpec();


        return true;
    }

    public void EquipWeapon(WeaponType weaponType)
    {
        if (currentWeapons[0]) currentWeapons[0].gameObject.SetActive(false);
        if (currentWeapons[1]) currentWeapons[1].gameObject.SetActive(false);

        int weaponStartIdx = (int)weaponType * 2;
        currentWeapons[0] = weapons[weaponStartIdx];
        currentWeapons[1] = weapons[weaponStartIdx + 1];

        currentWeapons[0].gameObject.SetActive(true);
        currentWeapons[1].gameObject.SetActive(true);

        foreach(SlotMarker marker in currentWeapons[0].GetComponentsInChildren<SlotMarker>())
        {
            if(marker.slotType == SlotType.LeftShotPos)
            {
                weaponShotPos[0] = marker.transform;
            }
        }

        foreach (SlotMarker marker in currentWeapons[1].GetComponentsInChildren<SlotMarker>())
        {
            if (marker.slotType == SlotType.RightShotPos)
            {
                weaponShotPos[1] = marker.transform;
            }
        }
    }

    public void Hurt(int damage)
    {
        if (isAlive)
        {
            currentHp -= damage;
            GameObject effect = Instantiate(hurtEffect, transform.position + Vector3.up, transform.rotation);
            Destroy(effect, 2f);
            Debug.Log(gameObject.name + "受到了" + damage + "点伤害");
        }
    }

    public void ChangeSpec(Spec extraSpec, bool isPermernate, float duration = 0)
    {
        if(isPermernate == false)
        {
            int idx = this.extraSpec.Count;
            this.extraSpec.Add(Spec.Clone(extraSpec));

            Debug.Log("得到临时属性加成：" + this.extraSpec.ToString() + "，持续时间：" + duration);
            finalSpec = Spec.Add(finalSpec, this.extraSpec[idx]);
            Debug.Log("最终属性临时变为：" + this.finalSpec.ToString());
            StartCoroutine(CleanExtraSpec(duration, idx));
        }
        else
        {
            this.baseSpec = Spec.Add(this.baseSpec, extraSpec);
            Debug.Log("得到永久属性加成：" + extraSpec.ToString() + "，基础属性变为：" + this.baseSpec);
        }
    }

    IEnumerator CleanExtraSpec(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);
        finalSpec = Spec.Sub(finalSpec, this.extraSpec[idx]);
        this.extraSpec[idx].ClearSpec();
        Debug.Log("临时加成结束，最终属性恢复为：" + this.finalSpec.ToString());
    }

    public void SwitchOutfit(int idx)
    {
        for (int i = 0; i < outfits.Length; i++)
        {
            if(i != idx)
            {
                outfits[i].gameObject.SetActive(false);
            }
            else
            {
                outfits[i].gameObject.SetActive(true);
                character.m_Animator = outfits[i].GetComponent<Animator>();
                currentOutfitIdx = i;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(isPlayer)
        {
            Pickup pickup = collider.GetComponent<Pickup>();

            if (pickup)
            {
                inventory.PutInItem(pickup.itemId, pickup.itemCount);
                Destroy(collider.gameObject);
            }
        }
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
