using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public RectTransform crosshair;

    float screenRatio { get { return (float)1280 / Screen.width; } }

    void Start()
    {
        instance = this;
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
}
