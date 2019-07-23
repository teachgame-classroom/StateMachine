using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    public static Item CreateItem(int itemId)
    {
        string itemName = "";

        switch(itemId)
        {
            case 1:
                itemName = "GunAxe";
                break;
            case 2:
                itemName = "Gun";
                break;
            case 3:
                itemName = "Arrow";
                break;
            case 4:
                itemName = "Axe";
                break;
            case 5:
                itemName = "Book";
                break;
            case 6:
                itemName = "Shield";
                break;
            case 7:
                itemName = "MagicSword";
                break;
            case 8:
                itemName = "Sword";
                break;
        }

        return new Item(itemId, itemName);
    }
}

public class Item
{
    public int itemId;
    public string itemName;
    public Sprite sprite;

    public Item(int itemId, string itemName)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        sprite = Resources.Load<Sprite>("ItemIcons/" + "Icon_" + this.itemId);
    }
}
