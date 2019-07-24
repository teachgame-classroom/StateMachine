using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHoverDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int slotIdx;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.OnSlotClicked(slotIdx);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetItemHover(slotIdx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetItemHover(-1);
    }
}
