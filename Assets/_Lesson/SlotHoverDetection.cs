using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHoverDetection : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
{
    public int slotIdx;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag:" + slotIdx);
        UIManager.instance.OnSlotBeginDrag(slotIdx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag:" + slotIdx);
        UIManager.instance.OnSlotDrag(slotIdx);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop:" + slotIdx);
        UIManager.instance.OnSlotDrop(slotIdx);
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
            UIManager.instance.OnEmptyDrop(slotIdx);
        }
        Debug.Log("End Drag:" + slotIdx);
    }

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
