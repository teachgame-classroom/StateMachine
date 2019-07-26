using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Reflection;
using ReflectionHelper;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager instance;
    public ItemDatabase itemDatabase;

    public Type[] itemCtorTypes = new Type[] { typeof(ItemInfo), typeof(IItemOwner) };

    public delegate object itemConstructor(object[] paras);
    public Dictionary<string, itemConstructor> ctorDict = new Dictionary<string, itemConstructor>();

    void Awake()
    {
        instance = this;
        InitCtorDict();
    }

    void InitCtorDict()
    {
        Type[] itemTypes = ClassFinder.GetSubclassTypes<Item>();

        for(int i = 0; i < itemTypes.Length; i++)
        {
            ConstructorInfo info = ClassFinder.GetConstructor(itemTypes[i], itemCtorTypes);
            ctorDict.Add(itemTypes[i].Name, info.Invoke);
        }
    }

    public itemConstructor GetConstructor(string itemClassName)
    {
        if(ctorDict.ContainsKey(itemClassName))
        {
            return ctorDict[itemClassName];
        }
        else
        {
            throw new Exception("找不到" + itemClassName + "的构造函数");
        }
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
