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
    public delegate void InventoryChangeDelegate(int gridIdx, int itemCount, Sprite sprite);
    public event InventoryChangeDelegate InventoryChangeEvent;

    private InventoryGrid[] grids;

    public Inventory(int capacity)
    {
        grids = new InventoryGrid[capacity];

        for(int i = 0; i < capacity; i++)
        {
            grids[i] = new InventoryGrid();
        }
    }

    public void Refresh()
    {
        for(int i = 0; i < grids.Length; i++)
        {
            if(grids[i].item != null)
            {
                InventoryChangeEvent(i, grids[i].itemCount, grids[i].item.sprite);
            }
            else
            {
                InventoryChangeEvent(i, 0, null);
            }
        }
    }

    public bool PutInItem(int itemId, int itemCount = 1)
    {
        Item item = ItemFactory.CreateItem(itemId);

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
            if(InventoryChangeEvent != null)
            {
                InventoryChangeEvent(putInGridIdx, grids[putInGridIdx].itemCount, grids[putInGridIdx].item.sprite);
            }
        }
        else
        {
            Debug.Log("背包已满，不能放入");
        }

        return result;
    }
}
