using UnityEngine;
using System.Collections.Generic;

public enum SettingsMenuState
{
    DEFAULT,
    DISPLAY,
    INPUT
}

[System.Serializable]
public class SettingsMenu : MenuState
{
    public UIMasterPanel masterPanel;

    public UIPanel inputPanel;
    public UIPanel waitingForInputPanel;

    public SettingsMenu()
    {
        masterPanel = new GameObject().AddComponent<UIMasterPanel>();
        masterPanel.SetRelativePosition(0, 0);
        masterPanel.SetRelativeSize(1, 1);
        masterPanel.ShowBox = false;

        waitingForInputPanel = new GameObject().AddComponent<UIPanel>();
        waitingForInputPanel.SetRelativePosition(0, 0);
        waitingForInputPanel.SetRelativeSize(1, 1);
        waitingForInputPanel.content = "Waiting for Input";
        waitingForInputPanel.ShowBox = false;

        inputPanel = new GameObject().AddComponent<UIPanel>();
        inputPanel.SetRelativePosition(0, 0);
        inputPanel.SetRelativeSize(1, 1);
        inputPanel.content = "Input Manager";

        masterPanel.AddChild(inputPanel);
        masterPanel.AddChild(waitingForInputPanel);

        UIButton backButton = new GameObject().AddComponent<UIButton>();
        backButton.SetAbsolutePosition(5, 5);
        backButton.SetAbsoluteSize(120, 25);

        backButton.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
        backButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
        backButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

        backButton.Text = "Back";

        backButton.OnButtonClicked += OnBackButtonClicked;

        masterPanel.AddChild(backButton);

        float positionY = 30;

        UIButton inputButton;
        foreach (var item in InputController.Instance.InputInfos)
        {
            float PanelHeight = 50;

            UIPanel InputInfoHolderPanel = new GameObject().AddComponent<UIPanel>();
            InputInfoHolderPanel.SetAbsolutePosition(0, positionY);
            InputInfoHolderPanel.SetAbsoluteSize(250, PanelHeight);

            InputInfoHolderPanel.HorizontalAnchor = HorizontalAnchorPoint.CENTER;
            InputInfoHolderPanel.HorizontalAlignment = HorizontalAnchorPoint.CENTER;

            InputInfoHolderPanel.content = item.Action.Replace("1_", "Player 1 ") + " : ";

            inputPanel.AddChild(InputInfoHolderPanel);

            //Label
            UIText inputLabel = new GameObject().AddComponent<UIText>();
            inputLabel.SetAbsoluteSize(100, 25);

            inputLabel.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputLabel.HorizontalAlignment = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            //inputLabel.Text = item.Action.Replace("1_", "Player 1 ") + " : ";

            InputInfoHolderPanel.AddChild(inputLabel);


            inputButton = new GameObject().AddComponent<UIButton>();
            inputButton.SetAbsolutePosition(80, 0);
            inputButton.SetAbsoluteSize(120, 25);

            inputButton.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            inputButton.Text = item.GetInfo();

            inputButton.Callback = new InputButtonCallback(OnInputButtonClicked, inputButton, item);

            InputInfoHolderPanel.AddChild(inputButton);


            UIButton resetButton = new GameObject().AddComponent<UIButton>();
            resetButton.SetAbsoluteSize(25, 25);

            resetButton.HorizontalAnchor = HorizontalAnchorPoint.RIGHT;
            resetButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            resetButton.HorizontalAlignment = HorizontalAnchorPoint.RIGHT;
            resetButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            resetButton.Text = "X";

            resetButton.Callback = new InputButtonCallback(OnDeleteInputButtonClicked, inputButton, item);

            InputInfoHolderPanel.AddChild(resetButton);

            positionY += PanelHeight;
        }
    }

    public void OnBackButtonClicked(UIButton button)
    {
        SwitchToState<MainMenuState>();
    }

    public void OnDeleteInputButtonClicked(UIButton button, InputInfo info)
    {
        InputController.Instance.DeleteKeyBind(info);
        button.Text = info.GetInfo();
    }

    private UIButton lastClickedInputButton = null;
    private InputInfo lastClickedInputInfo = null;
    public void OnInputButtonClicked(UIButton button, InputInfo info)
    {
        InputController.Instance.RebindKey(info);

        lastClickedInputButton = button;
        lastClickedInputInfo = info;

        inputPanel.SetActive(false, true);
    }

    public override void Update()
    {
        if (InputController.Instance.IsWaitingForInput)
        {
            if (InputController.Instance.CheckForInput())
            {
                if (lastClickedInputButton != null && lastClickedInputInfo != null)
                {
                    lastClickedInputButton.Text = lastClickedInputInfo.GetInfo();
                    inputPanel.SetActive(true, true);
                }
            }
        }
    }

    public override void OnGUI()
    {
        waitingForInputPanel.Visible = InputController.Instance.IsWaitingForInput;

        masterPanel.UpdateUI();
        
        if (GUILayout.Button("Reset Inputs"))
        {
            InputController.Instance.ResetInputs();
        }
    }
}
