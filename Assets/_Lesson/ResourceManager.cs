using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReflectionHelper;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    Sprite[] sprites;

    private string[] iconFolderNames = new string[] { "Amulet", "Belt", "Potion", "Weapon" };

    public Dictionary<string, Sprite[]> spriteDict = new Dictionary<string, Sprite[]>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        sprites = Resources.LoadAll<Sprite>("ItemIcons");
        InitSpriteDict();
    }

    void InitSpriteDict()
    {
        Type[] types = ReflectionHelper.ClassFinder.GetSubclassTypes<Item>();

        for(int i = 0; i < types.Length; i++)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/" + types[i].Name);

            if(sprites != null && sprites.Length > 0)
            {
                spriteDict.Add(types[i].Name, sprites);
            }
        }
    }

    public Sprite GetSprite(int itemId)
    {
        for(int i = 0; i < sprites.Length; i++)
        {
            if(sprites[i].name.StartsWith("Icon_" + itemId))
            {
                return sprites[i];
            }
        }

        return null;
    }

    public Sprite GetSprite(string spriteCategoryName, int textureId, int spriteId)
    {
        if(spriteDict.ContainsKey(spriteCategoryName))
        {
            Sprite[] sprites = spriteDict[spriteCategoryName];

            if(sprites == null || sprites.Length == 0)
            {
                return null;
            }
            else
            {
                for(int i = 0; i < sprites.Length; i++)
                {
                    if(sprites[i].name.Contains("Icon" + textureId + "_"))
                    {
                        if(sprites[i].name.EndsWith("_" + spriteId))
                        {
                            return sprites[i];
                        }
                    }
                }

                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
