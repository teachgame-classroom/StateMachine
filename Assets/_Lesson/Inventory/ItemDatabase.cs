using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public int itemId;
    public string itemName;
    public WeaponType weaponType;
    public Spec spec;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemInfo[] itemInfos;
}
