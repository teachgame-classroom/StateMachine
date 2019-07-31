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
    public bool consumeWhenUsed;
    public bool isEffectPermanent;
    public float effectDuration;
    public WeaponType weaponType;
    public Spec spec;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemInfo[] itemInfos;
}
