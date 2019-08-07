using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RarityType { Normal, Blue, Yellow, Purple, Orange }

public class Rarity
{
    public static float normalDrop = 0.5f;
    public static float blueDrop =  0.75f;
    public static float yelloDrop = 0.9f;
    public static float purpleDrop = 0.975f;

    public static RarityType GetRandomRarity()
    {
        float rnd = Random.Range(0, 1f);

        Debug.LogWarning("Random:" + rnd);

        if(rnd < normalDrop)
        {
            return RarityType.Normal;
        }
        else if(rnd < blueDrop)
        {
            return RarityType.Blue;
        }
        else if(rnd < yelloDrop)
        {
            return RarityType.Yellow;
        }
        else if(rnd < purpleDrop)
        {
            return RarityType.Purple;
        }
        else
        {
            return RarityType.Orange;
        }
    }

    public static Spec GetFinalSpecByRarity(Spec baseSpec, RarityType rarityType)
    {
        Spec extraSpec = GetExtraSpecByRarity(baseSpec, rarityType);

        Spec finalSpec = Spec.Add(baseSpec, extraSpec);

        return finalSpec;
    }

    public static Spec GetExtraSpecByRarity(Spec baseSpec, RarityType rarityType)
    {
        Spec extraSpec = new Spec();

        for(int i = 0; i < Spec.SPEC_COUNT; i++)
        {
            extraSpec[i] = baseSpec[i] * GetRandomFactorByRarity(rarityType);
        }

        return extraSpec;
    }

    public static float GetRandomFactorByRarity(RarityType rarityType)
    {
        switch (rarityType)
        {
            case RarityType.Normal:
                return 0;
            case RarityType.Blue:
                return Random.Range(0, 0.1f);
            case RarityType.Yellow:
                return Random.Range(0.08f, 0.3f);
            case RarityType.Purple:
                return Random.Range(0.25f, 0.45f);
            case RarityType.Orange:
                return Random.Range(0.5f, 0.75f);
            default:
                return 0;
        }
    }

    public static Color GetColorByRarity(RarityType rarityType)
    {
        switch (rarityType)
        {
            case RarityType.Normal:
                return Color.black;
            case RarityType.Blue:
                return Color.blue;
            case RarityType.Yellow:
                return Color.yellow;
            case RarityType.Purple:
                return new Color(0.5f, 0, 1, 1);
            case RarityType.Orange:
                return new Color(1f, 0.5f, 0, 1);
            default:
                return Color.black;
        }
    }
}

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

    bool UnEquip(int equipmentSlotIdx);
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

public enum EquipmentType { None = -1, GunAxe, Axe, Gun, LeatherArmor, MagicArmor }

public enum WeaponActionType { NoWeapon = -1, Melee, Range, MeleeAndRange }

public class Axe : Weapon
{
    public Axe(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}

public class Gun : Weapon
{
    public Gun(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}


public class GunAxe : Weapon
{
    public GunAxe(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}

public class Weapon : Equipment
{

    public Weapon(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}

public class Equipment : Item
{
    public Equipment(ItemInfo info, IItemOwner owner) : base(info, owner)
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

public class Armor : Equipment
{
    public Armor(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}


public class Amulet : Equipment
{
    public Amulet(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}

public class Belt : Equipment
{
    public Belt(ItemInfo info, IItemOwner owner) : base(info, owner)
    {

    }
}


public class Potion : Item
{
    public Potion(ItemInfo info, IItemOwner owner) : base(info, owner)
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
    public int price;

    public RarityType rarity;

    public bool canStack;
    public bool consumeWhenUsed;
    public bool isEffectPermanent;
    public float effectDuration;
    public EquipmentType equipmentType;

    public InventoryGrid grid;
    public IItemOwner owner;
    public Sprite sprite;    

    public int equipmentSlotIdx { get { return grid.GetEquipmentSlotIdx(); } }

    public Item(ItemInfo info, IItemOwner owner)
    {
        this.itemId = info.itemId;
        this.itemName = info.itemName;

        this.canStack = info.canStack;

        if(this.canStack)
        {
            this.rarity = RarityType.Normal;
        }
        else
        {
            this.rarity = Rarity.GetRandomRarity();
        }

        this.spec = Rarity.GetFinalSpecByRarity(info.spec, this.rarity);

        float priceFactor = 1 + Rarity.GetRandomFactorByRarity(this.rarity);

        this.price = (int)(info.price * priceFactor);

        this.consumeWhenUsed = info.consumeWhenUsed;
        this.isEffectPermanent = info.isEffectPermanent;
        this.effectDuration = info.effectDuration;
        this.equipmentType = info.weaponType;

        this.owner = owner;
        sprite = ResourceManager.instance.GetSprite(info.className, info.textureId, info.spriteId);

        Debug.Log("生成了稀有度为" + this.rarity + "的" + this.itemName + ",Spec:" + this.spec.ToString());
    }

    public Item(Item origin)
    {
        this.itemId = origin.itemId;
        this.itemName = origin.itemName;

        this.canStack = origin.canStack;

        this.rarity = origin.rarity;

        this.spec = Spec.Clone(origin.spec);
        this.price = origin.price;

        this.consumeWhenUsed = origin.consumeWhenUsed;
        this.isEffectPermanent = origin.isEffectPermanent;
        this.effectDuration = origin.effectDuration;
        this.equipmentType = origin.equipmentType;

        this.owner = origin.owner;
        sprite = origin.sprite;

        Debug.Log("生成了稀有度为" + this.rarity + "的" + this.itemName + ",Spec:" + this.spec.ToString());

    }

    public static Item Clone(Item origin)
    {
        return new Item(origin);
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
