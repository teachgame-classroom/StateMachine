using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    Sprite[] sprites;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        sprites = Resources.LoadAll<Sprite>("ItemIcons");
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
}
