using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ShopMenuList[] shopMenuLists;
    private int currentShopMenuIdx = 0;

    public List<Item> currentShopItems = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ShowShopMenu(currentShopMenuIdx);
            currentShopMenuIdx++;

            if(currentShopMenuIdx >= shopMenuLists.Length)
            {
                currentShopMenuIdx = 0;
            }
        }
    }

    public void ShowShopMenu(int idx)
    {
        if(shopMenuLists[idx] != null)
        {
            ShowShopMenu(shopMenuLists[idx]);
        }
    }

    public void ShowShopMenu(ShopMenuList shopMenuList)
    {
        currentShopItems.Clear();

        UIManager.instance.ClearShopItems();

        for(int i = 0; i < shopMenuList.menuItems.Length; i++)
        {
            Item item = ItemFactory.CreateItem(shopMenuList.menuItems[i]);
            currentShopItems.Add(item);

            UIManager.instance.CreateShopItemButton(item);
        }
    }
}
