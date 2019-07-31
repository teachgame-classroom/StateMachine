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

    public static Spec Clone(Spec origin)
    {
        Spec newSpec = new Spec();

        for(int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            newSpec[i] = origin[i];
        }

        return newSpec;
    }

    public void ClearSpec()
    {
        for(int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            this[i] = 0;
        }
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

        if (itemInfo != null)
        {
            ItemDatabaseManager.itemConstructor ctor = ItemDatabaseManager.instance.GetConstructor(itemInfo.className);
            return ctor(new object[] { itemInfo, null }) as Item;
            //return new Item(itemInfo, null);
        }

        return null;
    }
}

public enum WeaponType { GunAxe, Axe, Gun }

public class Axe : Weapon
{
    public Axe(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Axe(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}

public class Gun : Weapon
{
    public Gun(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Gun(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}


public class GunAxe : Weapon
{
    public GunAxe(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public GunAxe(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}

public class Weapon : Equipment
{

    public Weapon(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Weapon(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}

public class Equipment : Item
{
    public Equipment(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Equipment(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
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

public class Amulet : Equipment
{
    public Amulet(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Amulet(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}

public class Belt : Equipment
{
    public Belt(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Belt(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }
}


public class Potion : Item
{
    public Potion(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }

    public Potion(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
        : base(itemId, itemName, className, textureId, spriteId, spec, consumeWhenUsed, isEffectPermernate, effectDuration, weaponType, owner)
    {

    }


    public override void Use()
    {
        base.Use();
        owner.UseItem(this);
    }
}

public class Item
{
    public int itemId;
    public string itemName;
    public Spec spec;

    public bool consumeWhenUsed;
    public bool isEffectPermanent;
    public float effectDuration;
    public WeaponType weaponType;

    public InventoryGrid grid;
    public IItemOwner owner;
    public Sprite sprite;

    public Item(int itemId, string itemName, string className, int textureId, int spriteId, Spec spec, bool consumeWhenUsed, bool isEffectPermernate, float effectDuration, WeaponType weaponType, IItemOwner owner)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.spec = Spec.Clone(spec);
        this.consumeWhenUsed = consumeWhenUsed;
        this.isEffectPermanent = isEffectPermernate;
        this.effectDuration = effectDuration;
        this.weaponType = weaponType;

        this.owner = owner;
        sprite = ResourceManager.instance.GetSprite(className, textureId, spriteId);
    }


    public Item(ItemInfo info, IItemOwner owner) : this
        (info.itemId, info.itemName, info.className, info.textureId, info.spriteId, info.spec, info.consumeWhenUsed, info.isEffectPermanent, info.effectDuration, info.weaponType, owner)
    {

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
