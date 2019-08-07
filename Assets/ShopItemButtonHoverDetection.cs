using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemButtonHoverDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.ShowFloatingItemInfo(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.ShowFloatingItemInfo(null);
    }
}
