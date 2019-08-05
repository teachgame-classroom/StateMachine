using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class ThirdPersonCharacterController : MonoBehaviour, ICameraFollowable, IItemOwner
{
    public WeaponActionType weaponActionType;
    private EquipmentType[] weaponTypes = new EquipmentType[2];

    public Transform[] outfits;
    private int currentOutfitIdx = 0;

    private int outfitIdx = 0;

    public const int LEFT = 0;
    public const int RIGHT = 1;

    public bool isPlayer;
    protected CharacterStateMachine stateMachine;
    protected ThirdPersonCharacter character;

    public Spec baseSpec;
    private Spec equipmentTotalSpec;

    public Spec extraSpec = new Spec();
    public Spec finalSpec;

    public const int EQUIPMENT_SLOT_COUNT = 12;

    Equipment[] equipments = new Equipment[EQUIPMENT_SLOT_COUNT];

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

    public Transform[] currentWeapons = new Transform[2];

    public Transform[] weapons = new Transform[6];

    public Transform[] weaponLocation_L = new Transform[2];
    public Transform[] weaponLocation_R = new Transform[2];

    public Transform[] weaponShotPos = new Transform[2];

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
                    weapons[(int)EquipmentType.GunAxe * 2] = slotTrans;
                    break;
                case SlotType.RightGunAxe:
                    weapons[(int)EquipmentType.GunAxe * 2 + 1] = slotTrans;
                    break;
                case SlotType.LeftAxe:
                    weapons[(int)EquipmentType.Axe * 2] = slotTrans;
                    break;
                case SlotType.RightAxe:
                    weapons[(int)EquipmentType.Axe * 2 + 1] = slotTrans;
                    break;
                case SlotType.LeftGun:
                    weapons[(int)EquipmentType.Gun * 2] = slotTrans;
                    break;
                case SlotType.RightGun:
                    weapons[(int)EquipmentType.Gun * 2 + 1] = slotTrans;
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
            inventory.EquipmentSpecChangeEvent += OnEquipmentChange;
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
                EquipWeapon(EquipmentType.GunAxe, LEFT);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(EquipmentType.Axe, RIGHT);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeapon(EquipmentType.Gun, LEFT);
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
        if (currentWeapons[LEFT]) currentWeapons[LEFT].SetParent(weaponLocation_L[weaponPosIdx]);
        if (currentWeapons[RIGHT]) currentWeapons[RIGHT].SetParent(weaponLocation_R[weaponPosIdx]);

        foreach (Transform trans in currentWeapons)
        {
            if(trans)
            {
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
            }
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

    public void CaculateFinalSpec()
    {
        Spec base_equipment = Spec.Add(baseSpec, equipmentTotalSpec);
        finalSpec = Spec.Add(base_equipment, extraSpec);
        Debug.Log("Final Spec:" + finalSpec.ToString());
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        for (int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            UIManager.instance.SetStats((StatsType)i, finalSpec[i]);
        }
    }


    public bool Equip(Equipment equipment)
    {
        int[] equipmentSlot = inventory.GetEquipmentSlot(equipment);

        Weapon weapon = equipment as Weapon;

        if(weapon != null)
        {
            int hand = inventory.GetWeaponHand(weapon.equipmentSlotIdx);
            EquipWeapon(weapon.equipmentType,hand);
        }

        Armor armor = equipment as Armor;

        if(armor != null)
        {
            int outfitIdx = GetOutfitIndex(armor.equipmentType);

            if(outfitIdx >= 0)
            {
                SwitchOutfit(outfitIdx);
            }
        }
        return true;
    }

    public bool UnEquip(int equipmentSlotIdx)
    {
        int hand = inventory.GetWeaponHand(equipmentSlotIdx);

        if(hand != -1)
        {
            EquipWeapon(EquipmentType.None, hand);
        }

        return true;
    }


    void OnEquipmentChange(Spec equipmentTotalSpec)
    {
        this.equipmentTotalSpec = equipmentTotalSpec;
        CaculateFinalSpec();
    }

    public void EquipWeapon(EquipmentType weaponType, int hand)
    {
        int weaponStartIdx = (int)weaponType * 2;

        if (hand == LEFT || hand == RIGHT)
        {
            if (currentWeapons[hand]) currentWeapons[hand].gameObject.SetActive(false);

            if(weaponType == EquipmentType.None)
            {
                currentWeapons[hand] = null;
                return;
            }

            currentWeapons[hand] = weapons[weaponStartIdx + hand];
            currentWeapons[hand].gameObject.SetActive(true);

            foreach (SlotMarker marker in currentWeapons[hand].GetComponentsInChildren<SlotMarker>())
            {
                if (hand == LEFT && marker.slotType == SlotType.LeftShotPos)
                {
                    weaponShotPos[hand] = marker.transform;
                }

                if (hand == RIGHT && marker.slotType == SlotType.RightShotPos)
                {
                    weaponShotPos[hand] = marker.transform;
                }
            }

            weaponTypes[hand] = weaponType;
            weaponActionType = UpdateWeaponActionType(weaponTypes);
            Debug.Log("WeaponActionType:" + weaponActionType);
        }
    }

    WeaponActionType UpdateWeaponActionType(EquipmentType[] equipmentTypes)
    {
        bool hasAxe = false;
        bool hasWeapon = false;

        foreach(EquipmentType type in equipmentTypes)
        {
            if(type != EquipmentType.None)
            {
                hasWeapon = true;
            }

            if(type == EquipmentType.Axe)
            {
                hasAxe = true;
            }
        }

        if(hasWeapon)
        {
            if(hasAxe)
            {
                return WeaponActionType.Melee;
            }
            else
            {
                return WeaponActionType.MeleeAndRange;
            }
        }
        else
        {
            return WeaponActionType.NoWeapon;
        }
    }

    int GetOutfitIndex(EquipmentType equipment)
    {
        if(equipment == EquipmentType.LeatherArmor)
        {
            return 1;
        }

        if(equipment == EquipmentType.MagicArmor)
        {
            return 0;
        }

        return -1;
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
            this.extraSpec = Spec.Add(this.extraSpec, extraSpec);

            Debug.Log("得到临时属性加成：" + this.extraSpec.ToString() + "，持续时间：" + duration);
            CaculateFinalSpec();
            Debug.Log("最终属性临时变为：" + this.finalSpec.ToString());
            StartCoroutine(CleanExtraSpec(duration, extraSpec));
        }
        else
        {
            this.baseSpec = Spec.Add(this.baseSpec, extraSpec);
            Debug.Log("得到永久属性加成：" + extraSpec.ToString() + "，基础属性变为：" + this.baseSpec);
        }
    }

    IEnumerator CleanExtraSpec(float delay, Spec extraSpec)
    {
        yield return new WaitForSeconds(delay);
        this.extraSpec = Spec.Sub(this.extraSpec, extraSpec);
        CaculateFinalSpec();
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
