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
    private RectTransform backpackPanel;
    private RectTransform equipmentPanel;

    private RectTransform[] backpackSlots;
    private RectTransform[] equipmentSlots;

    private RectTransform[] statInfos = new RectTransform[9];

    public delegate void InventorySlotDelegate(InventorySlotType slotType, int slotIdx);
    public event InventorySlotDelegate InventorySlotClickEvent;
    public event InventorySlotDelegate InventorySlotHoverEvent;

    public delegate void InventoryDragDropDelegate(InventorySlotType slotType, int gridIdx);
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

        backpackPanel = characterPanel.Find("Content/Extension (Bags)") as RectTransform;

        Transform backpackRoot = backpackPanel.Find("Slots Grid");

        backpackSlots = new RectTransform[backpackRoot.childCount];

        for(int i = 0; i < backpackRoot.childCount; i++)
        {
            backpackSlots[i] = backpackRoot.GetChild(i) as RectTransform;
            backpackSlots[i].GetComponent<SlotHoverDetection>().slotIdx = i;
        }

        equipmentPanel = characterPanel.Find("Content/Equip Slots") as RectTransform;

        SlotHoverDetection[] hoverDetections = equipmentPanel.GetComponentsInChildren<SlotHoverDetection>();
        List<RectTransform> equipmentRectList = new List<RectTransform>();
        for(int i = 0; i < hoverDetections.Length; i++)
        {
            hoverDetections[i].slotIdx = i;
            equipmentRectList.Add(hoverDetections[i].transform as RectTransform);
        }

        equipmentSlots = equipmentRectList.ToArray();

        Debug.Log(backpackSlots.Length);

        StatsMarker[] statsMarkers = characterPanel.GetComponentsInChildren<StatsMarker>();

        for(int i = 0; i < statsMarkers.Length; i++)
        {
            int idx = (int)statsMarkers[i].statsType;
            statInfos[idx] = statsMarkers[i].transform as RectTransform;
        }

        SetItemHover(InventorySlotType.Backpack,-1);
        SetItemHover(InventorySlotType.Equipment, -1);

        dragIcon = transform.Find("DragIcon") as RectTransform;
    }

    public void OnInventoryChange(InventorySlotType inventorySlotType, int slotIdx, int itemCount, Sprite sprite, RarityType rarityType)
    {
        RectTransform[] slots = GetSlotByInventoryType(inventorySlotType);

        if(itemCount > 0)
        {
            SetItemSlotSprite(slots, slotIdx, sprite, rarityType);
        }
        else
        {
            SetItemSlotSprite(slots, slotIdx, null, rarityType);
        }

        if (inventorySlotType == InventorySlotType.Backpack)
        {
            SetItemSlotCountText(slots, slotIdx, itemCount);
        }
    }

    void Update()
    {
    }

    public void SetStats(StatsType statsType, float stat)
    {
        RectTransform rect = statInfos[(int)statsType];

        rect.Find("Value Text").GetComponent<Text>().text = stat.ToString();
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

    public void SetItemSlotSprite(RectTransform[] slots, int idx, Sprite sprite, RarityType rarityType)
    {
        Debug.Log(slots[idx].gameObject.name);
        Image iconImage = slots[idx].Find("Icon").GetComponent<Image>();
        iconImage.sprite = sprite;

        Image bgImage = slots[idx].Find("BG").GetComponent<Image>();

        if(iconImage.sprite)
        {
            iconImage.gameObject.SetActive(true);
            bgImage.color = Rarity.GetColorByRarity(rarityType);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
            bgImage.color = Rarity.GetColorByRarity(RarityType.Normal);
        }
    }

    public void SetItemSlotCountText(RectTransform[] slots, int idx, int number)
    {
        Text text = slots[idx].Find("Number").GetComponent<Text>();

        if (number > 0)
        {
            text.text = number.ToString();
        }
        else
        {
            text.text = "";
        }
    }


    public void SetItemHover(InventorySlotType slotType, int idx)
    {
        RectTransform[] slots = GetSlotByInventoryType(slotType);

        SetItemHover(slots, idx);

        if (InventorySlotHoverEvent != null)
        {
            InventorySlotHoverEvent(slotType, idx);
        }
    }

    private RectTransform[] GetSlotByInventoryType(InventorySlotType slotType)
    {
        RectTransform[] slots;

        if (slotType == InventorySlotType.Backpack)
        {
            slots = backpackSlots;
        }
        else
        {
            slots = equipmentSlots;
        }

        return slots;
    }

    public void SetItemHover(RectTransform[] slots, int idx)
    {
        if (idx < 0)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Find("Hover").gameObject.SetActive(false);
            }
        }
        else
        {
            slots[idx].Find("Hover").gameObject.SetActive(true);
        }
    }


    public void OnSlotBeginDrag(InventorySlotType slotType, int slotIdx)
    {
        if(InventoryBeginDragEvent != null)
        {
            InventoryBeginDragEvent(slotType, slotIdx);
        }
    }

    public void OnHasItemNotify(InventorySlotType slotType, int slotIdx)
    {
        RectTransform[] slots = GetSlotByInventoryType(slotType);

        Debug.Log("可以拖动");
        dragIcon.gameObject.SetActive(true);

        dragIcon.GetComponent<Image>().sprite = slots[slotIdx].Find("Icon").GetComponent<Image>().sprite;

        Transform numberTrans = slots[slotIdx].Find("Number");

        if(numberTrans)
        {
            string text = numberTrans.GetComponent<Text>().text;
            dragIcon.GetComponentInChildren<Text>().text = text;
            numberTrans.gameObject.SetActive(false);
        }

        dragIcon.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);
        slots[slotIdx].Find("Icon").gameObject.SetActive(false);
    }

    public void OnSlotDrag(InventorySlotType slotType, int slotIdx)
    {
        if(dragIcon.gameObject.activeSelf)
        {
            dragIcon.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);
        }
    }


    public void OnSlotDrop(InventorySlotType slotType, int slotIdx)
    {
        if(InventoryDropEvent != null)
        {
            InventoryDropEvent(slotType, slotIdx);
        }

        //itemSlots[slotIdx].Find("Icon").gameObject.SetActive(true);
        //itemSlots[slotIdx].Find("Number").gameObject.SetActive(true);

        //itemSlots[slotIdx].Find("Icon").GetComponent<Image>().sprite = dragIcon.GetComponent<Image>().sprite;
        //itemSlots[slotIdx].Find("Number").GetComponent<Text>().text = dragIcon.GetComponentInChildren<Text>().text;

        dragIcon.gameObject.SetActive(false);
    }

    public void OnEmptyDrop(InventorySlotType slotType, int slotIdx)
    {
        if(InventoryDropEmptyEvent != null)
        {
            InventoryDropEmptyEvent(slotType, slotIdx);
        }

        dragIcon.gameObject.SetActive(false);
    }

    public void OnSlotClicked(InventorySlotType slotType, int slotIdx)
    {
        Debug.Log("点击了" + slotIdx + "号格子");
        if(InventorySlotClickEvent != null)
        {
            InventorySlotClickEvent(slotType, slotIdx);
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
