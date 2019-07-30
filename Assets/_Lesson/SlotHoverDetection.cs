using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InventorySlotType { Backpack, Equipment }

public class SlotHoverDetection : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
{
    public InventorySlotType slotType;
    public int slotIdx;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag:" + slotIdx);
        UIManager.instance.OnSlotBeginDrag(slotType, slotIdx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag:" + slotIdx);
        UIManager.instance.OnSlotDrag(slotType, slotIdx);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop:" + slotIdx);
        UIManager.instance.OnSlotDrop(slotType, slotIdx);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool hoverOnSlot = false;

        foreach(GameObject go in eventData.hovered)
        {
            if(go.GetComponent<SlotHoverDetection>())
            {
                hoverOnSlot = true;
                break;
            }
        }

        if(!hoverOnSlot)
        {
            UIManager.instance.OnEmptyDrop(slotType, slotIdx);
        }
        Debug.Log("End Drag:" + slotIdx);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.OnSlotClicked(slotType, slotIdx);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetItemHover(slotType, slotIdx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetItemHover(slotType, -1);
    }
}
