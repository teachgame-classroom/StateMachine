using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public string itemName;
    public int itemId;
    public string className;
    public int textureId;
    public int spriteId;
    public bool canStack;
    public bool consumeWhenUsed;
    public bool isEffectPermanent;
    public float effectDuration;
    public EquipmentType weaponType;
    public Spec spec;
    public int price;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemInfo[] itemInfos;
}
