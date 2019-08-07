using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid
{
    public int slotIdx;
    public Item item;
    public int itemCount;

    public Inventory inventory;

    public int GetEquipmentSlotIdx()
    {
        int equipmentSlotIdx;

        InventorySlotType inventorySlotType = inventory.GetSlotType(slotIdx, out equipmentSlotIdx);

        if(inventorySlotType != InventorySlotType.Equipment)
        {
            return -1;
        }
        else
        {
            return equipmentSlotIdx;
        }
    }
}

public class Inventory
{
    public IItemOwner owner;

    public int gridOnDrag = -1;
    public int gridOnDrop = -1;

    public delegate void InventoryChangeDelegate(InventorySlotType slotType, int gridIdx, int itemCount, Sprite sprite, RarityType rarityType);
    public event InventoryChangeDelegate InventoryChangeEvent;

    public delegate void InventorySlotDelegate(InventorySlotType slotType, int gridIdx);
    public event InventorySlotDelegate InventoryHasItemEvent;

    public delegate void EquipmentSpecChangeDelegate(Spec equipmentTotalSpec);
    public event EquipmentSpecChangeDelegate EquipmentSpecChangeEvent;

    public delegate void MoneyChangeDelegate(int money);
    public event MoneyChangeDelegate MoneyChangeEvent;

    public delegate void SendItemDelegate(Item item);
    public event SendItemDelegate SendItemEvent;

    private InventoryGrid[] grids;

    private int backpackSize;
    private int equipmentSize;

    public int money { get; private set; }

    private int headSlot = 0;
    private int neckSlot = 1;
    private int shoulderSlot = 2;
    private int armorSlot = 3;
    private int bracerSlot = 4;

    private int glovesSlot = 5;
    private int beltSlot = 6;
    private int pantSlot = 7;
    private int bootSlot = 8;
    private int trinketSlot = 9;

    private int weaponLeftSlot = 10;
    private int weaponRightSlot = 11;

    public int WeaponLeftSlot_InternalIdx { get { return backpackSize + weaponLeftSlot; } }
    public int WeaponRightSlot_InternalIdx { get { return backpackSize + weaponRightSlot; } }

    public Inventory(int capacity, IItemOwner owner) : this(capacity)
    {
        this.owner = owner;
    }

    public Inventory(int capacity)
    {
        grids = new InventoryGrid[capacity];

        for(int i = 0; i < capacity; i++)
        {
            grids[i] = new InventoryGrid();
            grids[i].inventory = this;
            grids[i].slotIdx = i;
        }
    }

    public Inventory(int backpackSize, int equipmentSize, IItemOwner owner) : this(backpackSize, equipmentSize)
    {
        this.owner = owner;
    }

    public Inventory(int backpackSize, int equipmentSize) : this( backpackSize + equipmentSize )
    {
        this.backpackSize = backpackSize;
        this.equipmentSize = equipmentSize;
    }

    public void SetMoney(int amount)
    {
        money = Mathf.Max(0, amount);
        if(MoneyChangeEvent != null)
        {
            MoneyChangeEvent(money);
        }
        Debug.Log("兜里现在有" + money + "块钱");
    }

    public int AddMoney(int amount)
    {
        if(amount > 0)
        {
            SetMoney(money + amount);
        }

        return money;
    }

    public bool SpendMoney(int amount)
    {
        if(HasEnoughMoney(amount))
        {
            SetMoney(money - amount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasEnoughMoney(int amount)
    {
        return money >= amount;
    }

    public int GetInternalSlotIndex(InventorySlotType slotType, int slotIdx)
    {
        if(slotType == InventorySlotType.Backpack)
        {
            return slotIdx;
        }
        else
        {
            return slotIdx + this.backpackSize;
        }
    }

    public InventorySlotType GetSlotType(int internalSlotIdx, out int slotIndexOfType)
    {
        if(internalSlotIdx < backpackSize)
        {
            slotIndexOfType = internalSlotIdx;
            return InventorySlotType.Backpack;
        }
        else
        {
            slotIndexOfType = internalSlotIdx - backpackSize;
            return InventorySlotType.Equipment;
        }
    }

    public void Refresh()
    {
        for(int i = 0; i < grids.Length; i++)
        {
            int slotIndexOfType;

            InventorySlotType inventorySlotType = GetSlotType(i, out slotIndexOfType);

            if(grids[i].item != null)
            {
                InventoryChangeEvent(inventorySlotType, slotIndexOfType, grids[i].itemCount, grids[i].item.sprite, grids[i].item.rarity);
            }
            else
            {
                InventoryChangeEvent(inventorySlotType, slotIndexOfType, 0, null, RarityType.Normal);
            }
        }

        if(MoneyChangeEvent != null)
        {
            MoneyChangeEvent(money);
        }
    }

    public int GetWeaponHand(int weaponSlotIdx)
    {
        if(weaponSlotIdx == weaponLeftSlot)
        {
            return 0;
        }
        else if (weaponSlotIdx == weaponRightSlot)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public bool TryBuyItem(Item item)
    {
        if(SpendMoney(item.price))
        {
            Debug.Log("用" + item.price + "购买了" + item.itemName);
            return PutInItem(item);
        }
        else
        {
            Debug.Log("拿不出" + item.price + "来买" + item.itemName);
            return false;
        }
    }

    public bool PutInItem(int itemId, int itemCount = 1)
    {
        Item item = ItemFactory.CreateItem(itemId);
        return PutInItem(item, itemCount);
    }

    public bool PutInItem(Item item, int itemCount = 1)
    {
        item.owner = this.owner;

        int putInGridIdx = -1;
        int emptyGridIdx = -1;
        bool result = false;

        for (int i = 0; i < grids.Length; i++)
        {
            // 如果物品可以重复堆放
            if(item.canStack)
            {
                // 查找背包里面存放有同类物品的格子，增加其存放数量
                if (grids[i].item != null && grids[i].item.itemName == item.itemName)
                {
                    grids[i].itemCount += itemCount;
                    Debug.Log("将" + itemCount + "个" + item.itemName + "放入了第" + i + "格，总数增加为" + grids[i].itemCount + "个");
                    putInGridIdx = i;
                    result = true;
                }
            }
            else
            {
                itemCount = 1;
            }

            // 查找第一个没有存放物品的格子，当背包中还没有存放同类物品时，就把物品放到第一个空格子
            if (emptyGridIdx == -1)
            {
                if (grids[i].itemCount == 0 || grids[i].item == null)
                {
                    emptyGridIdx = i;
                }
            }
        }

        if (result == false && emptyGridIdx != -1)
        {
            grids[emptyGridIdx].item = item;
            grids[emptyGridIdx].itemCount = itemCount;
            Debug.Log("将" + itemCount + "个" + item.itemName + "放入了空白的第" + emptyGridIdx + "格，总数是" + grids[emptyGridIdx].itemCount + "个");
            putInGridIdx = emptyGridIdx;
            result = true;
        }

        if(result)
        {
            grids[putInGridIdx].item.grid = grids[putInGridIdx];

            if (InventoryChangeEvent != null)
            {
                int slotIndexOfType;
                InventorySlotType inventorySlotType = GetSlotType(putInGridIdx, out slotIndexOfType);

                InventoryChangeEvent(inventorySlotType, slotIndexOfType, grids[putInGridIdx].itemCount, grids[putInGridIdx].item.sprite, grids[putInGridIdx].item.rarity);
            }
        }
        else
        {
            Debug.Log("背包已满，不能放入");
        }

        return result;
    }

    public void OnInventoryBeginDrag(InventorySlotType slotType, int slotIdx)
    {
        slotIdx = GetInternalSlotIndex(slotType, slotIdx);

        if(HasItem(slotIdx))
        {
            Debug.Log("要拖动的格子上有物品");
            gridOnDrag = slotIdx;
            if(InventoryHasItemEvent != null)
            {
                int slotIndexOfType;

                InventorySlotType inventorySlotType = GetSlotType(slotIdx, out slotIndexOfType);

                InventoryHasItemEvent(inventorySlotType, slotIndexOfType);
            }
        }
    }

    public void OnInventoryDrop(InventorySlotType slotType, int slotIdx)
    {
        slotIdx = GetInternalSlotIndex(slotType, slotIdx);

        if (gridOnDrag == -1 || slotIdx == -1)
        {
            return;
        }

        gridOnDrop = slotIdx;

        if(CanSwitch(gridOnDrag, gridOnDrop))
        {
            SwitchItem(gridOnDrag, gridOnDrop);

            TryEquip(gridOnDrag);
            TryEquip(gridOnDrop);
        }
        else
        {
            Refresh();
        }

        gridOnDrag = -1;
        gridOnDrop = -1;
    }

    private bool TryEquip(int slotIdx)
    {
        if (IsEquipmentSlot(slotIdx))
        {
            Item item = grids[slotIdx].item;

            if (item != null)
            {
                Equipment equipment = item as Equipment;

                if (equipment != null)
                {
                    equipment.Equip();
                    return true;
                }
            }
            else
            {
                int equipmentSlot = slotIdx - backpackSize;
                owner.UnEquip(equipmentSlot);
            }
        }

        return false;
    }

    public void OnInventoryEmptyDrop(InventorySlotType slotType, int slotIdx)
    {
        slotIdx = GetInternalSlotIndex(slotType, slotIdx);

        gridOnDrag = -1;
        gridOnDrop = -1;
        Refresh();
    }

    public bool IsEquipmentSlot(int idx)
    {
        return idx >= backpackSize;
    }

    public bool CanSwitch(int idx_a, int idx_b)
    {
        bool SlotA_Match_ItemB = IsSlotMatchItem(idx_a, grids[idx_b].item);
        bool SlotB_Match_ItemA = IsSlotMatchItem(idx_b, grids[idx_a].item);
        return SlotA_Match_ItemB && SlotB_Match_ItemA;
    }

    public bool IsSlotMatchItem(int slotIdx, Item item)
    {
        if (item == null) return true;

        int slotIndexOfType;

        InventorySlotType slotType = GetSlotType(slotIdx, out slotIndexOfType);

        if(slotType == InventorySlotType.Backpack)
        {
            return true;
        }
        else
        {
            Equipment equipment = item as Equipment;

            if(equipment != null)
            {
                int[] slots = GetEquipmentSlot(equipment);

                if(slots == null || slots.Length == 0)
                {
                    return false;
                }

                for(int i = 0; i < slots.Length; i++)
                {
                    if(slots[i] == slotIndexOfType)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }

    public Spec CaculateTotalEquipmentSpec()
    {
        Spec totalSpec = new Spec();

        for(int i = backpackSize; i < backpackSize + equipmentSize; i++)
        {
            if(grids[i].item != null)
            {
                Equipment equipment = grids[i].item as Equipment;

                if(equipment != null)
                {
                    totalSpec = Spec.Add(totalSpec, equipment.spec);
                }
            }
        }

        return totalSpec;
    }

    public int[] GetEquipmentSlot(Equipment equipment)
    {
        Weapon weapon = equipment as Weapon;
        if(weapon != null)
        {
            return new int[] { weaponLeftSlot, weaponRightSlot };
        }

        Amulet amulet = equipment as Amulet;
        if (amulet != null)
        {
            return new int[] { neckSlot };
        }

        Belt Belt = equipment as Belt;
        if (Belt != null)
        {
            return new int[] { beltSlot };
        }

        Armor armor = equipment as Armor;
        if(armor != null)
        {
            return new int[] { armorSlot };
        }

        return null;
    }

    public bool IsSlotMatchEquipmentType(int slotIdx, int equipmentSlotType)
    {
        if(IsEquipmentSlot(slotIdx))
        {
            int equipmentSlotIdx;
            InventorySlotType slotType = GetSlotType(slotIdx, out equipmentSlotIdx);

            return equipmentSlotIdx == equipmentSlotType;
        }
        else
        {
            return false;
        }
    }

    public void SwitchItem(int idx_a, int idx_b)
    {
        InventoryGrid grid_a = grids[idx_a];
        InventoryGrid grid_b = grids[idx_b];

        Item tempItem = grid_a.item;
        int tempItemCount = grid_a.itemCount;

        grid_a.item = grid_b.item;
        grid_a.itemCount = grid_b.itemCount;

        if(grid_a.item != null)
        {
            grid_a.item.grid = grid_a;
        }

        grid_b.item = tempItem;
        grid_b.itemCount = tempItemCount;

        if(grid_b.item != null)
        {
            grid_b.item.grid = grid_b;
        }

        if(IsEquipmentSlot(idx_a) || IsEquipmentSlot(idx_b))
        {
            Spec spec = CaculateTotalEquipmentSpec();
            EquipmentSpecChangeEvent(spec);
        }

        Refresh();
    }

    public void OnInventorySlotClick(InventorySlotType slotType, int slotIdx)
    {
        slotIdx = GetInternalSlotIndex(slotType, slotIdx);

        Debug.Log("背包收到了" + slotIdx + "号格子的点击事件");
        if(HasItem(slotIdx))
        {
            grids[slotIdx].item.OnClick();
            Debug.Log(grids[slotIdx].itemCount);
        }

        int itemCount = grids[slotIdx].itemCount;
        RarityType rarityType = grids[slotIdx].item.rarity;
        Sprite sprite = null;

        if (grids[slotIdx].item != null)
        {
            sprite = grids[slotIdx].item.sprite;
        }

        int slotIndexOfType;
        InventorySlotType inventorySlotType = GetSlotType(slotIdx, out slotIndexOfType);
        InventoryChangeEvent(inventorySlotType, slotIndexOfType, itemCount, sprite, rarityType);
    }

    public void OnInventorySlotHover(InventorySlotType slotType, int slotIdx)
    {
        slotIdx = GetInternalSlotIndex(slotType, slotIdx);

        if (slotIdx >= 0 && slotIdx < grids.Length)
        {
            Item item = grids[slotIdx].item;

            if(item != null)
            {
                Debug.Log(item.ToString());
            }

            if (SendItemEvent != null)
            {
                SendItemEvent(item);
            }
        }
    }

    public bool HasItem(int slotIdx)
    {
        return grids[slotIdx].itemCount > 0 && grids[slotIdx].item != null;
    }
}
