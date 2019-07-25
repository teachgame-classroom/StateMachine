using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spec
{
    public const int SPEC_COUNT = 9;

    public int atk;
    public int def;
    public int dex;

    public int hp;
    public int mana;
    public int fury;

    public float recoverHp;
    public float recoverMana;
    public float recoverFury;

    public float this[int idx]
    {
        get
        {
            switch (idx)
            {
                case 0:
                    return atk;
                case 1:
                    return def;
                case 2:
                    return dex;
                case 3:
                    return hp;
                case 4:
                    return mana;
                case 5:
                    return fury;
                case 6:
                    return recoverHp;
                case 7:
                    return recoverMana;
                case 8:
                    return recoverFury;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        set
        {
            switch (idx)
            {
                case 0:
                    atk = (int)value;
                    return;
                case 1:
                    def = (int)value;
                    return;
                case 2:
                    dex = (int)value;
                    return;
                case 3:
                    hp = (int)value;
                    return;
                case 4:
                    mana = (int)value;
                    return;
                case 5:
                    fury = (int)value;
                    return;
                case 6:
                    recoverHp = value;
                    return;
                case 7:
                    recoverMana = value;
                    return;
                case 8:
                    recoverFury = value;
                    return;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

        }
    }

    public override string ToString()
    {
        string result = string.Format
            ("Atk:{0}, Def:{1}, Dex:{2}, Hp:{3}, Mana:{4}, Fury:{5}, recoverHp:{6}, recoverMana:{7}, recoverFury:{8}",
            atk, def, dex, hp, mana, fury, recoverHp, recoverMana, recoverFury);
        return result;
    }

    public static Spec Add(Spec specA, Spec specB)
    {
        Spec newSpec = new Spec();

        for(int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            newSpec[i] = specA[i] + specB[i];
        }

        return newSpec;
    }

    public static Spec Sub(Spec specA, Spec specB)
    {
        Spec newSpec = new Spec();

        for (int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            newSpec[i] = specA[i] - specB[i];
        }

        return newSpec;
    }

    public static Spec Mul(Spec specA, float multiplier)
    {
        Spec newSpec = new Spec();

        for (int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            newSpec[i] = specA[i] * multiplier;
        }

        return newSpec;
    }


}

public interface IItemOwner
{
    bool UseItem(Item item);
    bool Equip(Equipment equipment);
}

public class ItemFactory
{
    public static Item CreateItem(int itemId)
    {
        ItemInfo itemInfo = ItemDatabaseManager.instance.GetItemInfo(itemId);

        if(itemInfo != null)
        {
            return new Item(itemId, itemInfo.itemName, itemInfo.spec, true);
        }

        return null;
    }

    public static Weapon CreateWeapon(int itemId)
    {
        ItemInfo itemInfo = ItemDatabaseManager.instance.GetItemInfo(itemId);

        if (itemInfo != null)
        {
            return new Weapon(itemId, itemInfo.itemName, itemInfo.spec, itemInfo.weaponType);
        }

        return null;

    }
}

public enum WeaponType { GunAxe, Axe, Gun }

public class Weapon : Equipment
{
    public WeaponType weaponType;

    public Weapon(int itemId, string itemName, Spec spec, WeaponType weaponType) : this(itemId, itemName, spec, weaponType, null)
    {
    }

    public Weapon(int itemId, string itemName, Spec spec, WeaponType weaponType, IItemOwner owner) : base(itemId, itemName, spec, owner)
    {
        this.weaponType = weaponType;
    }
}

public class Equipment : Item
{
    public Equipment(int itemId, string itemName, Spec spec) : this(itemId, itemName, spec, null)
    {

    }

    public Equipment(int itemId, string itemName, Spec spec, IItemOwner owner) : base(itemId, itemName, spec, owner)
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

    public Spec spec;

    public Item(int itemId, string itemName, Spec spec, bool consumeWhenUsed) : this(itemId, itemName, spec, consumeWhenUsed, null)
    {
    }

    public Item(int itemId, string itemName, Spec spec, IItemOwner owner) : this(itemId, itemName, spec, false, owner)
    {
    }

    public Item(int itemId, string itemName, Spec spec, bool consumeWhenUsed, IItemOwner owner)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.spec = spec;
        this.consumeWhenUsed = consumeWhenUsed;
        this.owner = owner;
        sprite = ResourceManager.instance.GetSprite(itemId);
//        sprite = Resources.Load<Sprite>("ItemIcons/" + "Icon_" + this.itemId);
    }

    public override string ToString()
    {
        string result = string.Format("itemId:{0}, itemName:{1}, spec:{2}", itemId.ToString(), itemName, spec.ToString());
        return result;
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
