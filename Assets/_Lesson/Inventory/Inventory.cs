using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid
{
    public Item item;
    public int itemCount;
}

public class Inventory
{
    public IItemOwner owner;

    public int gridOnDrag = -1;
    public int gridOnDrop = -1;

    public delegate void InventoryChangeDelegate(InventorySlotType slotType, int gridIdx, int itemCount, Sprite sprite);
    public event InventoryChangeDelegate InventoryChangeEvent;

    public delegate void InventorySlotDelegate(InventorySlotType slotType, int gridIdx);
    public event InventorySlotDelegate InventoryHasItemEvent;

    private InventoryGrid[] grids;

    private int backpackSize;
    private int equipmentSize;

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
                InventoryChangeEvent(inventorySlotType, slotIndexOfType, grids[i].itemCount, grids[i].item.sprite);
            }
            else
            {
                InventoryChangeEvent(inventorySlotType, slotIndexOfType, 0, null);
            }
        }
    }

    public bool PutInItem(int itemId, int itemCount = 1)
    {
        Item item = ItemFactory.CreateItem(itemId);
        item.owner = this.owner;
        return PutInItem(item, itemCount);
    }

    public bool PutInItem(Item item, int itemCount = 1)
    {
        int putInGridIdx = -1;
        int emptyGridIdx = -1;
        bool result = false;

        for (int i = 0; i < grids.Length; i++)
        {
            // 查找背包里面存放有同类物品的格子，增加其存放数量
            if (grids[i].item != null && grids[i].item.itemName == item.itemName)
            {
                grids[i].itemCount += itemCount;
                Debug.Log("将" + itemCount + "个" + item.itemName + "放入了第" + i + "格，总数增加为" + grids[i].itemCount + "个");
                putInGridIdx = i;
                result = true;
            }

            // 查找第一个没有存放物品的格子，当背包中还没有存放同类物品时，就把物品放到第一个空格子
            if(emptyGridIdx == -1)
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

                InventoryChangeEvent(inventorySlotType, slotIndexOfType, grids[putInGridIdx].itemCount, grids[putInGridIdx].item.sprite);
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
                Weapon weapon = equipment as Weapon;

                if(slotIdx != WeaponLeftSlot_InternalIdx && slotIdx != WeaponRightSlot_InternalIdx)
                {
                    if(weapon == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if(weapon == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
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
        Sprite sprite = null;

        if (grids[slotIdx].item != null)
        {
            sprite = grids[slotIdx].item.sprite;
        }

        int slotIndexOfType;
        InventorySlotType inventorySlotType = GetSlotType(slotIdx, out slotIndexOfType);
        InventoryChangeEvent(inventorySlotType, slotIndexOfType, itemCount, sprite);
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
        }
    }

    public bool HasItem(int slotIdx)
    {
        return grids[slotIdx].itemCount > 0 && grids[slotIdx].item != null;
    }
}
