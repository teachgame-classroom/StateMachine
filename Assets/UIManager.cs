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

    public Sprite[] testSprites;
    public RectTransform crosshair;
    public RectTransform characterPanel;
    private RectTransform inventoryPanel;
    private RectTransform[] itemSlots;

    private int hoverSlotIdx = -1;

    float screenRatio { get { return (float)1280 / Screen.width; } }

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<ThirdPersonCharacterController>().inventory.InventoryChangeEvent += OnInventoryChange;

        instance = this;
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

        for(int i = 0; i < testSprites.Length; i++)
        {
            SetItemSlotSprite(i, testSprites[i]);
        }

        SetItemHover(-1);
    }

    private void OnInventoryChange(int gridIdx, int itemCount, Sprite sprite)
    {
        SetItemSlotSprite(gridIdx, sprite);
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
