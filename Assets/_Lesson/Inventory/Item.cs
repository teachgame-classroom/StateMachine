using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IItemOwner
{
    bool UseItem(Item item);
    bool Equip(Equipment equipment);
}

public class ItemFactory
{
    public static Item CreateItem(int itemId)
    {
        string itemName = "";

        switch (itemId)
        {
            case 4:
                itemName = "Health";
                break;
            case 5:
                itemName = "Mana";
                break;
            case 6:
                itemName = "Fury";
                break;
        }

        return new Item(itemId, itemName, true);
    }

    public static Weapon CreateWeapon(int itemId)
    {
        string itemName = "";
        WeaponType weaponType = WeaponType.GunAxe;

        switch (itemId)
        {
            case 1:
                itemName = "GunAxe";
                weaponType = WeaponType.GunAxe;
                break;
            case 2:
                itemName = "Gun";
                weaponType = WeaponType.Gun;
                break;
            case 3:
                itemName = "Axe";
                weaponType = WeaponType.Axe;
                break;
        }

        return new Weapon(itemId, itemName, weaponType);
    }
}

public enum WeaponType { GunAxe, Axe, Gun }

public class Weapon : Equipment
{
    public WeaponType weaponType;

    public Weapon(int itemId, string itemName, WeaponType weaponType) : this(itemId, itemName, weaponType, null)
    {
    }

    public Weapon(int itemId, string itemName, WeaponType weaponType, IItemOwner owner) : base(itemId, itemName, owner)
    {
        this.weaponType = weaponType;
    }
}

public class Equipment : Item
{
    public Equipment(int itemId, string itemName) : this(itemId, itemName, null)
    {

    }

    public Equipment(int itemId, string itemName, IItemOwner owner) : base(itemId, itemName, owner)
    {

    }

    public override void OnClick()
    {
        Equip();
    }

    public virtual void Equip()
    {
        if(owner != null)
        {
            owner.Equip(this);
        }            
    }
}

public class Item
{
    public InventoryGrid grid;
    public bool consumeWhenUsed;
    public IItemOwner owner;
    public int itemId;
    public string itemName;
    public Sprite sprite;

    public Item(int itemId, string itemName, bool consumeWhenUsed) : this(itemId, itemName, consumeWhenUsed, null)
    {
    }

    public Item(int itemId, string itemName, IItemOwner owner) : this(itemId, itemName, false, owner)
    {
    }

    public Item(int itemId, string itemName, bool consumeWhenUsed, IItemOwner owner)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.consumeWhenUsed = consumeWhenUsed;
        this.owner = owner;
        sprite = ResourceManager.instance.GetSprite(itemId);
//        sprite = Resources.Load<Sprite>("ItemIcons/" + "Icon_" + this.itemId);
    }

    public virtual void OnClick()
    {
        TryUse();
    }

    public virtual bool TryUse()
    {
        Debug.Log(itemName + ":" + grid.itemCount);

        if (consumeWhenUsed)
        {
            if(grid.itemCount > 0)
            {
                grid.itemCount -= 1;
                Use();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Use();
            return true;
        }
    }

    public virtual void Use()
    {
        Debug.Log("使用了" + itemName);
    }
}
