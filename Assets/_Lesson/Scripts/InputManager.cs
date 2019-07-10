using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonEventType
{
    Down,
    Up,
    KeepDown
}

public class InputList
{
    public const string FIRE1 = "Fire1";
    public const string FIRE2 = "Fire2";
    public const string FIRE3 = "Fire3";
    public const string FIRE4 = "Fire4";
    public const string L1 = "L1";
    public const string L2 = "L2";
    public const string R1 = "R1";
    public const string R2 = "R2";

    public static string xAxisName { get { return "Horizontal"; } }

    public static string yAxisName { get { return "Vertical"; } }

    public static string[] buttons { get { return _buttons; } }

    private static string[] _buttons = new string[] { FIRE1, FIRE2, FIRE3, FIRE4, L1, L2, R1, R2 };

}

public class InputManager : MonoBehaviour
{

    public static InputManager instance;

    public delegate void AxisEventHandler(float h, float v);

    public event AxisEventHandler InputEvent_Axis;

    public delegate void ButtonEventHandler(string buttonName, ButtonEventType eventType);

    public event ButtonEventHandler InputEvent_Button;

    public delegate void MouseButtonEventHandler(int buttonIdx, ButtonEventType eventType, RaycastHit hit);

    public event MouseButtonEventHandler InputEvent_MouseButton;

    public delegate void KeyboardEventHandler(int keyNumber);

    public event KeyboardEventHandler InputEvent_Keyboard;


    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputAxis(InputList.xAxisName, InputList.yAxisName);
        CheckInputButton(InputList.buttons);

        CheckMouseButton();
        CheckInputKeyboard();
    }


    private void CheckInputKeyboard()
    {
        Debug.Log("检查键盘事件");
        int keyNumber = -1;

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            keyNumber = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyNumber = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyNumber = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            keyNumber = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            keyNumber = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            keyNumber = 5;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            keyNumber = 6;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            keyNumber = 7;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            keyNumber = 8;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            keyNumber = 9;
        }

        if (keyNumber >= 0 && keyNumber <= 9)
        {
            Debug.Log("发送键盘事件");
            InputEvent_Keyboard(keyNumber);
        }
    }

    private void CheckInputAxis(string xAxis, string yAxis)
    {
        float h = Input.GetAxis(xAxis);
        float v = Input.GetAxis(yAxis);

        if (InputEvent_Axis != null)
        {
            InputEvent_Axis(h, v);
        }
    }

    private void CheckMouseButton()
    {
        CheckAllMouseButtonDown();
        CheckAllMouseButtonUp();
        CheckAllMouseButtonKeepDown();
    }

    private void CheckAllMouseButtonDown()
    {
        for (int i = 0; i < 3; i++)
        {
            CheckMouseButtonDown(i);
        }
    }

    private void CheckAllMouseButtonUp()
    {
        for (int i = 0; i < 3; i++)
        {
            CheckMouseButtonUp(i);
        }
    }

    private void CheckAllMouseButtonKeepDown()
    {
        for (int i = 0; i < 3; i++)
        {
            CheckMouseButtonKeepDown(i);
        }
    }

    private void CheckMouseButtonDown(int idx)
    {
        if (Input.GetMouseButtonDown(idx))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (InputEvent_MouseButton != null)
                    InputEvent_MouseButton(idx, ButtonEventType.Down, hit);
            }
        }
    }

    private void CheckMouseButtonUp(int idx)
    {
        if (Input.GetMouseButtonUp(idx))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (InputEvent_MouseButton != null)
                    InputEvent_MouseButton(idx, ButtonEventType.Up, hit);
            }
        }
    }

    private void CheckMouseButtonKeepDown(int idx)
    {
        if (Input.GetMouseButton(idx))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (InputEvent_MouseButton != null)
                    InputEvent_MouseButton(idx, ButtonEventType.KeepDown, hit);
            }
        }
    }

    private void CheckInputButton(string[] buttonList)
    {
        CheckAllButtonDown(buttonList);
        CheckAllButtonUp(buttonList);
        CheckAllButtonKeepDown(buttonList);
    }

    private void CheckAllButtonDown(string[] buttonList)
    {
        foreach (string buttonName in buttonList)
        {
            CheckButtonDown(buttonName);
        }
    }

    private void CheckAllButtonUp(string[] buttonList)
    {
        foreach (string buttonName in buttonList)
        {
            CheckButtonUp(buttonName);
        }
    }

    private void CheckAllButtonKeepDown(string[] buttonList)
    {
        foreach (string buttonName in buttonList)
        {
            CheckButtonKeepDown(buttonName);
        }
    }

    private void CheckButtonDown(string buttonName)
    {
        if (Input.GetButtonDown(buttonName))
        {
            if (InputEvent_Button != null)
            {
                InputEvent_Button(buttonName, ButtonEventType.Down);
            }
        }
    }

    private void CheckButtonUp(string buttonName)
    {
        if (Input.GetButtonUp(buttonName))
        {
            if (InputEvent_Button != null)
            {
                InputEvent_Button(buttonName, ButtonEventType.Up);
            }
        }
    }

    private void CheckButtonKeepDown(string buttonName)
    {
        if (Input.GetButton(buttonName))
        {
            if (InputEvent_Button != null)
            {
                InputEvent_Button(buttonName, ButtonEventType.KeepDown);
            }
        }
    }
}
