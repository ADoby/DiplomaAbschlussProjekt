using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum InputType
{
    KEYCODE,
    MOUSEBUTTON,
    UNITYBUTTON,
    AXIS,
    AXISLOW,
    AXISHIGH,
    AXISASBUTTONLOW,
    AXISASBUTTONHIGH
}

public class InputResult
{
    public bool active;
    public float value;
    public bool down;
    public bool clicked;
    public bool up;
}

public enum ActionType
{
    BUTTON,
    AXIS,
    AXISDIRECTION
}

[System.Serializable]
public class InputInfo
{
    public string Action = "Action";

    public ActionType ActionType;

    public InputType InputType;

    public KeyCode key;
    public int mouseButtonID;
    public string inputString;

    private float lastValue = 0f;
    private bool clicked = false;

    public void Update()
    {
        clicked = false;
        switch (InputType)
        {
            case InputType.KEYCODE:
                break;
            case InputType.MOUSEBUTTON:
                break;
            case InputType.UNITYBUTTON:
                break;
            case InputType.AXIS:
                break;
            case InputType.AXISASBUTTONLOW:
                float newValue1 = Input.GetAxis(inputString);
                if (lastValue == 0 && newValue1 < 0)
                {
                    clicked = true;
                }
                lastValue = newValue1;
                break;
            case InputType.AXISASBUTTONHIGH:
                float newValue2 = Input.GetAxis(inputString);
                if (lastValue == 0 && newValue2 > 0)
                {
                    clicked = true;
                }
                lastValue = newValue2;
                break;
            default:
                break;
        }
    }

    public float Value()
    {
        float value = 0;
        switch (InputType)
        {
            case InputType.KEYCODE:
                value = Input.GetKey(key) ? 1 : 0;
                break;
            case InputType.MOUSEBUTTON:
                value = Input.GetMouseButton(mouseButtonID) ? 1 : 0;
                break;
            case InputType.UNITYBUTTON:
                value = Input.GetButton(inputString) ? 1 : 0;
                break;
            case InputType.AXIS:
                value = Input.GetAxis(inputString);
                break;
            case InputType.AXISLOW:
                value = Input.GetAxis(inputString) < 0 ? Input.GetAxis(inputString) : 0;
                break;
            case InputType.AXISHIGH:
                value = Input.GetAxis(inputString) > 0 ? Input.GetAxis(inputString) : 0;
                break;
            case InputType.AXISASBUTTONLOW:
                value = Input.GetAxis(inputString) < 0 ? 1 : 0;
                break;
            case InputType.AXISASBUTTONHIGH:
                value = Input.GetAxis(inputString) > 0 ? 1 : 0;
                break;
            default:
                break;
        }
        return value;
    }

    public bool Clicked()
    {
        switch (InputType)
        {
            case InputType.KEYCODE:
                return Input.GetKeyDown(key);
            case InputType.MOUSEBUTTON:
                return Input.GetMouseButtonDown(mouseButtonID);
            case InputType.UNITYBUTTON:
                return Input.GetButtonDown(inputString);
            case InputType.AXIS:
                break; //An axis can't be clicked
            case InputType.AXISLOW:
                break;
            case InputType.AXISHIGH:
                break;
            case InputType.AXISASBUTTONLOW:
                return clicked;
            case InputType.AXISASBUTTONHIGH:
                return clicked;
            default:
                break;
        }
        return false;
    }

    public bool Down()
    {
        switch (InputType)
        {
            case InputType.KEYCODE:
                return Input.GetKey(key);
            case InputType.MOUSEBUTTON:
                return Input.GetMouseButton(mouseButtonID);
            case InputType.UNITYBUTTON:
                return Input.GetButton(inputString);
            case InputType.AXIS:
                break;
            case InputType.AXISLOW:
                break;
            case InputType.AXISHIGH:
                break;
            case InputType.AXISASBUTTONLOW:
                return Input.GetAxis(inputString) < 0;
            case InputType.AXISASBUTTONHIGH:
                return Input.GetAxis(inputString) > 0;
            default:
                break;
        }
        return false;
    }

    public bool Up()
    {
        return !Down();
    }

    public virtual string GetInfo()
    {
        switch (InputType)
        {
            case InputType.KEYCODE:
                return "Key: " + key.ToString();
            case InputType.MOUSEBUTTON:
                if (mouseButtonID == 0)
                {
                    return "Left mouse button";
                }
                else if (mouseButtonID == 1)
                {
                    return "Right mouse button";
                }
                else if (mouseButtonID == 2)
                {
                    return "Midle mouse button";
                }
                break;
            case InputType.UNITYBUTTON:
                return "Button: " + inputString;
            case InputType.AXIS:
                return "Axis: " + inputString;
            case InputType.AXISLOW:
                return "Axis direction low: " + inputString;
            case InputType.AXISHIGH:
                return "Axis direction high: " + inputString;
            case InputType.AXISASBUTTONLOW:
                return "Axis as button low: " + inputString;
            case InputType.AXISASBUTTONHIGH:
                return "Axis as button high: " + inputString;
            default:
                break;
        }
        return "Default InputInfo";
    }
}

public class InputController : MonoBehaviour{

    public InputInfo[] InputInfos =
    {
        new InputInfo() {Action = "1_LEFT", ActionType = ActionType.AXISDIRECTION, key = KeyCode.A},
        new InputInfo() {Action = "1_RIGHT", ActionType = ActionType.AXISDIRECTION, key = KeyCode.D},
        new InputInfo() {Action = "1_JUMP", ActionType = ActionType.BUTTON, key = KeyCode.Space},
        new InputInfo() {Action = "1_SKILL1", ActionType = ActionType.BUTTON, key = KeyCode.Alpha1},
        new InputInfo() {Action = "1_SKILL2", ActionType = ActionType.BUTTON, key = KeyCode.Alpha2},
        new InputInfo() {Action = "1_SKILL3", ActionType = ActionType.BUTTON, key = KeyCode.Alpha3},
        new InputInfo() {Action = "1_SKILL4", ActionType = ActionType.BUTTON, key = KeyCode.Alpha4},
        new InputInfo() {Action = "ESCAPE", ActionType = ActionType.BUTTON, key = KeyCode.Escape},
        new InputInfo() {Action = "LEVELUP", ActionType = ActionType.BUTTON, key = KeyCode.F},
        new InputInfo() {Action = "1_CROUCH", ActionType = ActionType.BUTTON, key = KeyCode.S}
    };

    private InputInfo[] resetInfo;

    private Dictionary<string, InputInfo> actionToInfo = new Dictionary<string, InputInfo>();


	// Use this for initialization
	void Awake () {
        Instance = this;

        resetInfo = (InputInfo[])InputInfos.Clone();

        bool save = false;
        bool load = false;
        if (PlayerPrefs.GetInt("InputCount") == 0)
        {
            Debug.Log("No Saved Prefs, saving default");
            save = true;
            PlayerPrefs.SetInt("InputCount", InputInfos.Length);
        }
        else
        {
            load = true;
        }

        if (load)
        {
            List<InputInfo> inputInfoList = new List<InputInfo>();
            int count = PlayerPrefs.GetInt("InputCount");
            for (int i = 0; i < count; i++)
            {
                InputInfo input = new InputInfo();
                input.Action = PlayerPrefs.GetString("Action" + i.ToString());
                input.ActionType = (ActionType)PlayerPrefs.GetInt("ActionActionType" + i.ToString());
                input.InputType = (InputType)PlayerPrefs.GetInt("ActionInputType" + i.ToString());
                input.key = (KeyCode)PlayerPrefs.GetInt("ActionKey" + i.ToString());
                input.mouseButtonID = PlayerPrefs.GetInt("ActionMouseButton" + i.ToString());
                input.inputString = PlayerPrefs.GetString("ActionInputString" + i.ToString());
                inputInfoList.Add(input);
            }
            InputInfos = inputInfoList.ToArray();
        }

        int actionID = 0;
        foreach (var item in InputInfos)
        {
            actionToInfo.Add(item.Action, item);

            if (save)
            {
                PlayerPrefs.SetString("Action" + actionID.ToString(), item.Action);
                PlayerPrefs.SetInt("ActionActionType" + actionID.ToString(), (int)item.ActionType);
                PlayerPrefs.SetInt("ActionInputType" + actionID.ToString(), (int)item.InputType);
                PlayerPrefs.SetInt("ActionKey" + actionID.ToString(), (int)item.key);
                PlayerPrefs.SetInt("ActionMouseButton" + actionID.ToString(), (int)item.mouseButtonID);
                PlayerPrefs.SetString("ActionInputString" + actionID.ToString(), item.inputString);
            }
            actionID++;
        }
	}

    [ContextMenu("ResetInputs")]
    public void ResetInputs()
    {
        InputInfos = (InputInfo[])resetInfo.Clone();

        PlayerPrefs.SetInt("InputCount", InputInfos.Length);

        actionToInfo.Clear();
        int actionID = 0;
        foreach (var item in resetInfo)
        {

            PlayerPrefs.SetString("Action" + actionID.ToString(), item.Action);
            PlayerPrefs.SetInt("ActionActionType" + actionID.ToString(), (int)item.ActionType);
            PlayerPrefs.SetInt("ActionInputType" + actionID.ToString(), (int)item.InputType);
            PlayerPrefs.SetInt("ActionKey" + actionID.ToString(), (int)item.key);
            PlayerPrefs.SetInt("ActionMouseButton" + actionID.ToString(), (int)item.mouseButtonID);
            PlayerPrefs.SetString("ActionInputString" + actionID.ToString(), item.inputString);

            actionToInfo.Add(item.Action, item);

            actionID++;
        }
    }

    public string[] unityButtonInputs;
    public string[] unityAxisInputs = 
    {
        "Mouse ScrollWheel",
        "L_XAxis_1",
        "L_YAxis_1",
        "R_XAxis_1",
        "R_YAxis_1",
        "TriggersL_1",
        "TriggersR_1"
    };

    private bool waitForInput = false;
    public bool IsWaitingForInput
    {
        get { return waitForInput; }
    }

    private InputInfo clickedInfo = null;

    private void ChangeInfoMouse(int mouseButton)
    {
        clickedInfo.mouseButtonID = mouseButton;
        clickedInfo.InputType = InputType.MOUSEBUTTON;
        waitForInput = false;
        UpdatePlayerPrefForAction(clickedInfo);
    }
    private void ChangeInfoKey(KeyCode key)
    {
        clickedInfo.key = key;
        clickedInfo.InputType = InputType.KEYCODE;
        waitForInput = false;
        UpdatePlayerPrefForAction(clickedInfo);
    }
    private void ChangeInfoButton(string button)
    {
        clickedInfo.inputString = button;
        clickedInfo.InputType = InputType.UNITYBUTTON;
        waitForInput = false;
        UpdatePlayerPrefForAction(clickedInfo);
    }
    private void ChangeInfoAxis(string axis)
    {
        if (clickedInfo.ActionType == ActionType.BUTTON)
        {
            if (Input.GetAxis(axis) < 0)
            {
                clickedInfo.InputType = InputType.AXISASBUTTONLOW;
            }
            else
            {
                clickedInfo.InputType = InputType.AXISASBUTTONHIGH;
            }
        }
        else if (clickedInfo.ActionType == ActionType.AXISDIRECTION)
        {
            if (Input.GetAxis(axis) < 0)
            {
                clickedInfo.InputType = InputType.AXISLOW;
            }
            else
            {
                clickedInfo.InputType = InputType.AXISHIGH;
            }
        }
        else
        {
            clickedInfo.InputType = InputType.AXIS;
        }
        clickedInfo.inputString = axis;
        waitForInput = false;
        UpdatePlayerPrefForAction(clickedInfo);
    }

    public void UpdatePlayerPrefForAction(InputInfo info)
    {
        for (int i = 0; i < InputInfos.Length; i++)
        {
            if (InputInfos[i] == info)
            {
                PlayerPrefs.SetInt("ActionInputType" + i.ToString(), (int)info.InputType);
                PlayerPrefs.SetInt("ActionKey" + i.ToString(), (int)info.key);
                PlayerPrefs.SetInt("ActionMouseButton" + i.ToString(), (int)info.mouseButtonID);
                PlayerPrefs.SetString("ActionInputString" + i.ToString(), info.inputString);
            }
        }
    }

    public InputInfo GetInfo(string action)
    {
        foreach (var inputInfo in InputInfos)
        {
            if (inputInfo.Action == action)
            {
                return inputInfo;
            }
        }
        return null;
    }

    public bool CheckForInput()
    {
        if (clickedInfo == null || !waitForInput)
            return true;

        for (int i = 0; i < 430; i++) // 430 is number of keys currently. Unity 4.3.4
        {
            //Ignore default joystick Buttons, which lead to every joystick
            if (i >= 330 && i <= 349)
                continue;

            if (Input.GetKeyDown((KeyCode)i))
            {
                ChangeInfoKey((KeyCode)i);
                return true;
            }
        }
        foreach (string item in unityButtonInputs)
        {
            if (Input.GetButtonDown(item))
            {
                ChangeInfoButton(item);
                return true;
            }
        }
        foreach (string item in unityAxisInputs)
        {
            if (Input.GetAxis(item) != 0)
            {
                ChangeInfoAxis(item);
                return true;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                ChangeInfoMouse(i);
                return true;
            }
        }

        return false;
    }

	// Update is called once per frame
	public void Update () {
        foreach (var item in InputInfos)
        {
            item.Update(); //Some Inputs need this to check for "clicked" state
        }
	}

    public static InputController Instance;

    public static float GetValue(string ActionName)
    {
        return Instance.GetValueI(ActionName);
    }
    public static bool GetDown(string action)
    {
        return Instance.GetDownI(action);
    }
    public static bool GetUp(string action)
    {
        return Instance.GetUpI(action);
    }
    public static bool GetClicked(string action)
    {
        return Instance.GetClickedI(action);
    }

    public float GetValueI(string ActionName)
    {
        if (!actionToInfo.ContainsKey(ActionName))
        {
            throw new ArgumentOutOfRangeException("Action " + ActionName + " not found!");
        }
        return actionToInfo[ActionName].Value();
    }
    public bool GetDownI(string ActionName)
    {
        if (!actionToInfo.ContainsKey(ActionName))
        {
            throw new ArgumentOutOfRangeException("Action " + ActionName + " not found!");
        }
        return actionToInfo[ActionName].Down();
    }
    public bool GetUpI(string ActionName)
    {
        if (!actionToInfo.ContainsKey(ActionName))
        {
            throw new ArgumentOutOfRangeException("Action " + ActionName + " not found!");
        }
        return actionToInfo[ActionName].Up();
    }
    public bool GetClickedI(string ActionName)
    {
        if (!actionToInfo.ContainsKey(ActionName))
        {
            throw new ArgumentOutOfRangeException("Action " + ActionName + " not found!");
        }
        return actionToInfo[ActionName].Clicked();
    }

    public void RebindKey(InputInfo item)
    {
        waitForInput = true;
        clickedInfo = item;
    }

    public void RebindKey(string inputString)
    {
        foreach (var inputInfo in InputInfos)
        {
            if (inputInfo.Action == inputString)
            {
                waitForInput = true;
                clickedInfo = inputInfo;
            }
        }
    }

    public void CancelRebind()
    {
        waitForInput = false;
        clickedInfo = null;
    }

    public void DeleteKeyBind(InputInfo item)
    {
        clickedInfo = item;
        ChangeInfoButton("None");
    }
}
