using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform dragIcon;

    public RectTransform crosshair;
    public RectTransform characterPanel;
    private RectTransform inventoryPanel;
    private RectTransform[] itemSlots;

    private int hoverSlotIdx = -1;

    public delegate void InventorySlotDelegate(int slotIdx);
    public event InventorySlotDelegate InventorySlotClickEvent;
    public event InventorySlotDelegate InventorySlotHoverEvent;

    public delegate void InventoryDragDropDelegate(int gridIdx);
    public event InventoryDragDropDelegate InventoryBeginDragEvent;
    public event InventoryDragDropDelegate InventoryDropEvent;
    public event InventoryDragDropDelegate InventoryDropEmptyEvent;

    float screenRatio { get { return (float)1280 / Screen.width; } }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.Find("Player");

        TogglePanel(characterPanel, false);

        inventoryPanel = characterPanel.Find("Content/Extension (Bags)") as RectTransform;

        Transform slotsRoot = inventoryPanel.Find("Slots Grid");

        itemSlots = new RectTransform[slotsRoot.childCount];

        for(int i = 0; i < slotsRoot.childCount; i++)
        {
            itemSlots[i] = slotsRoot.GetChild(i) as RectTransform;
            itemSlots[i].GetComponent<SlotHoverDetection>().slotIdx = i;
        }

        Debug.Log(itemSlots.Length);

        SetItemHover(-1);

        dragIcon = transform.Find("DragIcon") as RectTransform;
    }

    public void OnInventoryChange(int gridIdx, int itemCount, Sprite sprite)
    {
        if(itemCount > 0)
        {
            SetItemSlotSprite(gridIdx, sprite);
        }
        else
        {
            SetItemSlotSprite(gridIdx, null);
        }

        SetItemSlotCountText(gridIdx, itemCount);
    }

    void Update()
    {
    }

    public void ShowCrosshair(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        crosshair.anchoredPosition = screenPos * screenRatio;
        ToggleCrosshair(true);
    }

    public void CenterCrosshair()
    {
        crosshair.anchoredPosition = (Vector3.right * Screen.width + Vector3.up * Screen.height ) / 2 * screenRatio;
        ToggleCrosshair(true);
    }

    public void ToggleCrosshair(bool setActive)
    {
        crosshair.gameObject.SetActive(setActive);
    }

    public Vector3 ScreenToCanvasPoint(Vector3 screenPos, float screenRatio)
    {
        return screenPos * screenRatio;
    }

    public void TogglePanel(RectTransform panel, bool setActive)
    {
        panel.gameObject.SetActive(setActive);
    }

    public bool ToggleCharacterPanel()
    {
        TogglePanel(characterPanel, !characterPanel.gameObject.activeSelf);
        return characterPanel.gameObject.activeSelf;
    }

    public void SetItemSlotSprite(int idx, Sprite sprite)
    {
        Image iconImage = itemSlots[idx].Find("Icon").GetComponent<Image>();
        iconImage.sprite = sprite;

        if(iconImage.sprite)
        {
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    public void SetItemSlotCountText(int idx, int number)
    {
        Text text = itemSlots[idx].Find("Number").GetComponent<Text>();

        if (number > 0)
        {
            text.text = number.ToString();
        }
        else
        {
            text.text = "";
        }
    }


    public void SetItemHover(int idx)
    {
        if(idx < 0)
        {
            for(int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Find("Hover").gameObject.SetActive(false);
            }
        }
        else
        {
            if(hoverSlotIdx >= 0)
            {
                itemSlots[hoverSlotIdx].Find("Hover").gameObject.SetActive(false);
            }

            hoverSlotIdx = idx;
            itemSlots[hoverSlotIdx].Find("Hover").gameObject.SetActive(true);
        }

        if(InventorySlotHoverEvent != null)
        {
            InventorySlotHoverEvent(idx);
        }
    }

    public void OnSlotBeginDrag(int slotIdx)
    {
        if(InventoryBeginDragEvent != null)
        {
            InventoryBeginDragEvent(slotIdx);
        }
    }

    public void OnHasItemNotify(int slotIdx)
    {
        Debug.Log("可以拖动");
        dragIcon.gameObject.SetActive(true);

        dragIcon.GetComponent<Image>().sprite = itemSlots[slotIdx].Find("Icon").GetComponent<Image>().sprite;
        dragIcon.GetComponentInChildren<Text>().text = itemSlots[slotIdx].Find("Number").GetComponent<Text>().text;

        dragIcon.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);

        itemSlots[slotIdx].Find("Icon").gameObject.SetActive(false);
        itemSlots[slotIdx].Find("Number").gameObject.SetActive(false);
    }

    public void OnSlotDrag(int slotIdx)
    {
        if(dragIcon.gameObject.activeSelf)
        {
            dragIcon.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);
        }
    }


    public void OnSlotDrop(int slotIdx)
    {
        if(InventoryDropEvent != null)
        {
            InventoryDropEvent(slotIdx);
        }

        //itemSlots[slotIdx].Find("Icon").gameObject.SetActive(true);
        //itemSlots[slotIdx].Find("Number").gameObject.SetActive(true);

        //itemSlots[slotIdx].Find("Icon").GetComponent<Image>().sprite = dragIcon.GetComponent<Image>().sprite;
        //itemSlots[slotIdx].Find("Number").GetComponent<Text>().text = dragIcon.GetComponentInChildren<Text>().text;

        dragIcon.gameObject.SetActive(false);
    }

    public void OnEmptyDrop(int slotIdx)
    {
        if(InventoryDropEmptyEvent != null)
        {
            InventoryDropEmptyEvent(slotIdx);
        }

        dragIcon.gameObject.SetActive(false);
    }

    public void OnSlotClicked(int slotIdx)
    {
        Debug.Log("点击了" + slotIdx + "号格子");
        if(InventorySlotClickEvent != null)
        {
            InventorySlotClickEvent(slotIdx);
        }
    }

    [MenuItem("Tools/Rename Selected...")]
    public static void RenameSelectedObject()
    {
        GameObject selected = Selection.activeGameObject;

        Text[] texts = selected.transform.GetComponentsInChildren<Text>();

        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.name = "Number";
            texts[i].text = "";
        }
    }
}
