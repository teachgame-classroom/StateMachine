using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager instance;
    public ItemDatabase itemDatabase;

    void Awake()
    {
        instance = this;
    }

    public ItemInfo GetItemInfo(int itemId)
    {
        for(int i = 0; i < itemDatabase.itemInfos.Length; i++)
        {
            ItemInfo info = itemDatabase.itemInfos[i];

            if (info.itemId == itemId)
            {
                return info;
            }
        }

        return null;
    }
}
