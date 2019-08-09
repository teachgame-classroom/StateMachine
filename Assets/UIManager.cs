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
    public RectTransform floatingPanel;

    public GameObject shopItemButtonPrefab;
    public GameObject questItemButtonPrefab;

    public QuestList questList;

    public RectTransform crosshair;
    public RectTransform characterPanel;
    private RectTransform backpackPanel;
    private RectTransform equipmentPanel;
    private RectTransform shopPanel;
    private RectTransform shopContentRoot;

    private RectTransform questPanel;
    private RectTransform questContentRoot;
    private List<RectTransform> questButtons = new List<RectTransform>();

    private RectTransform[] backpackSlots;
    private RectTransform[] equipmentSlots;

    private RectTransform[] statInfos = new RectTransform[9];

    private Text moneyText;

    public delegate void InventorySlotDelegate(InventorySlotType slotType, int slotIdx);
    public event InventorySlotDelegate InventorySlotClickEvent;
    public event InventorySlotDelegate InventorySlotHoverEvent;

    public delegate void InventoryDragDropDelegate(InventorySlotType slotType, int gridIdx);
    public event InventoryDragDropDelegate InventoryBeginDragEvent;
    public event InventoryDragDropDelegate InventoryDropEvent;
    public event InventoryDragDropDelegate InventoryDropEmptyEvent;

    public delegate void BuyItemDelegate(Item item);
    public event BuyItemDelegate BuyItemEvent;

    public delegate void IntDelegate(int i);
    public event IntDelegate QuestButtonClickEvent;

    float screenRatio { get { return (float)1280 / Screen.width; } }

    void Awake()
    {
        instance = this;

        GameObject player = GameObject.Find("Player");

        TogglePanel(characterPanel, false);

        backpackPanel = characterPanel.Find("Content/Extension (Bags)") as RectTransform;

        Transform backpackRoot = backpackPanel.Find("Slots Grid");

        backpackSlots = new RectTransform[backpackRoot.childCount];

        for (int i = 0; i < backpackRoot.childCount; i++)
        {
            backpackSlots[i] = backpackRoot.GetChild(i) as RectTransform;
            backpackSlots[i].GetComponent<SlotHoverDetection>().slotIdx = i;
        }

        moneyText = backpackPanel.Find("Footer/Gold").GetComponent<Text>();

        equipmentPanel = characterPanel.Find("Content/Equip Slots") as RectTransform;

        SlotHoverDetection[] hoverDetections = equipmentPanel.GetComponentsInChildren<SlotHoverDetection>();
        List<RectTransform> equipmentRectList = new List<RectTransform>();
        for (int i = 0; i < hoverDetections.Length; i++)
        {
            hoverDetections[i].slotIdx = i;
            equipmentRectList.Add(hoverDetections[i].transform as RectTransform);
        }

        equipmentSlots = equipmentRectList.ToArray();

        Debug.Log(backpackSlots.Length);

        StatsMarker[] statsMarkers = characterPanel.GetComponentsInChildren<StatsMarker>();

        for (int i = 0; i < statsMarkers.Length; i++)
        {
            int idx = (int)statsMarkers[i].statsType;
            statInfos[idx] = statsMarkers[i].transform as RectTransform;
        }

        SetItemHover(InventorySlotType.Backpack, -1);
        SetItemHover(InventorySlotType.Equipment, -1);

        dragIcon = transform.Find("DragIcon") as RectTransform;

        shopPanel = transform.Find("ShopMenu") as RectTransform;
        shopContentRoot = shopPanel.GetComponentInChildren<ScrollRect>().content;

        questPanel = transform.Find("QuestMenu") as RectTransform;
        questContentRoot = questPanel.GetComponentInChildren<ScrollRect>().content;

        //for(int i = 0; i < questList.quests.Length; i++)
        //{
        //    CreateQuestButton(questList.quests[i]);
        //}
    }

    public void OnInventoryChange(InventorySlotType inventorySlotType, int slotIdx, int itemId, int itemCount, Sprite sprite, RarityType rarityType)
    {
        RectTransform[] slots = GetSlotByInventoryType(inventorySlotType);

        if (itemCount > 0)
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
        UpdateFloatingPanelPos();
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
        crosshair.anchoredPosition = (Vector3.right * Screen.width + Vector3.up * Screen.height) / 2 * screenRatio;
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

    public Vector3 CanvasToScreenPoint(Vector3 canvasPos, float screenRatio)
    {
        return canvasPos / screenRatio;
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

        if (iconImage.sprite)
        {
            iconImage.gameObject.SetActive(true);
            bgImage.color = Rarity.GetColorByRarity(rarityType, true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
            bgImage.color = Rarity.GetColorByRarity(RarityType.Normal, true);
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

    public void ClearPanelContents(RectTransform rootPanel)
    {
        for (int i = 0; i < rootPanel.childCount; i++)
        {
            Destroy(rootPanel.GetChild(i).gameObject);
        }
    }

    public GameObject CreatePanelContent(RectTransform rootPanel, GameObject contentPrefab)
    {
        GameObject contentInstance = GameObject.Instantiate(contentPrefab, rootPanel);
        return contentInstance;
    }

    public void ClearShopItems()
    {
        ClearPanelContents(shopContentRoot);
    }

    public void CreateShopItemButton(Item item)
    {
        CreateShopItemButton(shopItemButtonPrefab, item);
    }

    public void CreateShopItemButton(GameObject buttonPrefab, Item item)
    {
        GameObject buttonInstance = CreatePanelContent(shopContentRoot, buttonPrefab);
        SetupItemButton(buttonInstance, item);
    }

    public void SetupItemButton(GameObject buttonInstance, Item item)
    {
        RarityType rarityType = item.rarity;
        Color color = Rarity.GetColorByRarity(rarityType);

        Text nameText = buttonInstance.transform.Find("Name").GetComponent<Text>();
        nameText.text = item.itemNameCN;

        Shadow nameShadow = nameText.GetComponent<Shadow>();

        Image icon = buttonInstance.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = item.sprite;

        Text priceText = buttonInstance.transform.Find("Price").GetComponent<Text>();
        priceText.text = item.price.ToString();

        nameText.color = color;
        nameShadow.effectColor = color / 2;

        Button button = buttonInstance.GetComponent<Button>();
        button.onClick.AddListener(() => { if (BuyItemEvent != null) { BuyItemEvent(item); } });

        ShopItemButtonHoverDetection hover = buttonInstance.GetComponent<ShopItemButtonHoverDetection>();
        hover.item = item;
    }

    public void OnQuestsEvent(Quest[] quests)
    {
        RefreshQuestPanel(quests);
    }

    public void ToggleShopPanel()
    {
        bool newActiveState = !shopPanel.gameObject.activeSelf;
        shopPanel.gameObject.SetActive(newActiveState);
    }

    public void ToggleQuestPanel()
    {
        bool newActiveState = !questPanel.gameObject.activeSelf;
        questPanel.gameObject.SetActive(newActiveState);
    }

    public void RefreshQuestPanel(Quest[] quests)
    {
        ClearQuest();

        for(int i = 0; i < quests.Length; i++)
        {
            CreateQuestButton(quests[i]);
        }
    }

    public void ClearQuest()
    {
        ClearPanelContents(questContentRoot);
    }

    public void CreateQuestButton(Quest quest)
    {
        CreateQuestButton(questItemButtonPrefab, quest);
    }

    public void CreateQuestButton(GameObject questButtonPrefab, Quest quest)
    {
        GameObject buttonInstance = CreatePanelContent(questContentRoot, questButtonPrefab);
        questButtons.Add(buttonInstance.transform as RectTransform);
        SetupQuestButton(buttonInstance, quest);
    }

    public void SetupQuestButton(GameObject buttonInstance, Quest quest)
    {
        int questId = quest.questId;
        buttonInstance.transform.Find("Name").GetComponent<Text>().text = quest.questName;
        buttonInstance.GetComponent<Button>().onClick.AddListener(() => { OnQuestButtonClick(questId); });
    }

    public void OnQuestButtonClick(int idx)
    {
        if(QuestButtonClickEvent != null)
        {
            QuestButtonClickEvent(idx);
        }
    }

    public void OnQuestStatusChange(int questId, string status, Color color)
    {
        Text statusText = questButtons[questId].Find("Status").GetComponent<Text>();
        statusText.text = status;
        statusText.color = color;

        statusText.GetComponent<Shadow>().effectColor = color / 2;
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

            ToggleFloatingPanel(false);
        }
        else
        {
            slots[idx].Find("Hover").gameObject.SetActive(true);
        }
    }


    public void OnSlotBeginDrag(InventorySlotType slotType, int slotIdx)
    {
        if (InventoryBeginDragEvent != null)
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

        if (numberTrans)
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
        if (dragIcon.gameObject.activeSelf)
        {
            dragIcon.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);
        }
    }

    public void OnSlotDrop(InventorySlotType slotType, int slotIdx)
    {
        if (InventoryDropEvent != null)
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
        if (InventoryDropEmptyEvent != null)
        {
            InventoryDropEmptyEvent(slotType, slotIdx);
        }

        dragIcon.gameObject.SetActive(false);
    }

    public void OnSlotClicked(InventorySlotType slotType, int slotIdx)
    {
        Debug.Log("点击了" + slotIdx + "号格子");
        if (InventorySlotClickEvent != null)
        {
            InventorySlotClickEvent(slotType, slotIdx);
        }
    }

    public void OnReceiveItemEvent(Item item)
    {
        ShowFloatingItemInfo(item);
    }

    public void ShowFloatingItemInfo(Item item)
    {
        if (item != null)
        {
            ToggleFloatingPanel(true);
            UpdateFloatingPanelInfo(item);
        }
        else
        {
            ToggleFloatingPanel(false);
        }
    }

    public void ToggleFloatingPanel(bool setActive)
    {
        floatingPanel.gameObject.SetActive(setActive);
    }
       
    public void UpdateFloatingPanelInfo(Item item)
    {
        Text nameText = floatingPanel.Find("NameText").GetComponent<Text>();
        Text atkText = floatingPanel.Find("Atk/ValueText").GetComponent<Text>();
        Text defText = floatingPanel.Find("Def/ValueText").GetComponent<Text>();
        Text dexText = floatingPanel.Find("Dex/ValueText").GetComponent<Text>();
        Text hpText = floatingPanel.Find("Health/ValueText").GetComponent<Text>();
        Text manaText = floatingPanel.Find("Mana/ValueText").GetComponent<Text>();
        Text furyText = floatingPanel.Find("Fury/ValueText").GetComponent<Text>();
        Text hpregenText = floatingPanel.Find("RegenHealth/ValueText").GetComponent<Text>();

        nameText.text = item.itemNameCN;
        nameText.color = Rarity.GetColorByRarity(item.rarity);

        atkText.text = item.spec.atk.ToString();
        defText.text = item.spec.def.ToString();
        dexText.text = item.spec.dex.ToString();
        hpText.text = item.spec.hp.ToString();
        manaText.text = item.spec.mana.ToString();
        furyText.text = item.spec.fury.ToString();
        hpregenText.text = item.spec.recoverHp.ToString();
    }

    public void UpdateFloatingPanelPos()
    {
        if(floatingPanel.gameObject.activeInHierarchy)
        {
            float viewportHeight = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;

            if (viewportHeight > 0.4f)
            {
                floatingPanel.pivot = new Vector2(0.5f, 1.1f);
            }
            else
            {
                floatingPanel.pivot = new Vector2(0.5f, -0.1f);
            }

            floatingPanel.anchoredPosition = ScreenToCanvasPoint(Input.mousePosition, screenRatio);
        }
    }

    public void OnMoneyChanged(int money)
    {
        moneyText.text = money.ToString();
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
