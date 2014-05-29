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
        masterPanel.SetPosition(0, 0, true);
        masterPanel.SetSize(1, 1, true);
        masterPanel.ShowBox = false;

        waitingForInputPanel = new GameObject().AddComponent<UIPanel>();
        waitingForInputPanel.SetPosition(0, 0, true);
        waitingForInputPanel.SetSize(1, 1, true);
        waitingForInputPanel.content = "Waiting for Input";
        waitingForInputPanel.ShowBox = false;

        inputPanel = new GameObject().AddComponent<UIPanel>();
        inputPanel.SetPosition(0, 0, true);
        inputPanel.SetSize(1, 1, true);
        inputPanel.content = "Input Manager";

        masterPanel.AddChild(inputPanel);
        masterPanel.AddChild(waitingForInputPanel);

        UIButton backButton = new GameObject().AddComponent<UIButton>();
        backButton.SetPosition(5, 5, false);
        backButton.SetSize(120, 25, false);

        backButton.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
        backButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
        backButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

        backButton.Text = "Back";

        backButton.buttonCallback = new SimpleButtonCallback(OnBackButtonClicked, backButton);

        masterPanel.AddChild(backButton);

        float positionY = 30;

        UIButton inputButton;
        foreach (var item in InputController.Instance.InputInfos)
        {
            float PanelHeight = 50;

            UIPanel InputInfoHolderPanel = new GameObject().AddComponent<UIPanel>();
            InputInfoHolderPanel.SetPosition(0, positionY, false);
            InputInfoHolderPanel.SetSize(250, PanelHeight, false);

            InputInfoHolderPanel.HorizontalAnchor = HorizontalAnchorPoint.CENTER;
            InputInfoHolderPanel.HorizontalAlignment = HorizontalAnchorPoint.CENTER;

            InputInfoHolderPanel.content = item.Action.Replace("1_", "Player 1 ") + " : ";

            inputPanel.AddChild(InputInfoHolderPanel);

            //Label
            UIText inputLabel = new GameObject().AddComponent<UIText>();
            inputLabel.SetSize(100, 25, false);

            inputLabel.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputLabel.HorizontalAlignment = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            //inputLabel.Text = item.Action.Replace("1_", "Player 1 ") + " : ";

            InputInfoHolderPanel.AddChild(inputLabel);


            inputButton = new GameObject().AddComponent<UIButton>();
            inputButton.SetPosition(80, 0, false);
            inputButton.SetSize(120, 25, false);

            inputButton.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            inputButton.Text = item.GetInfo();

            inputButton.buttonCallback = new InputButtonCallback(OnInputButtonClicked, inputButton, item);

            InputInfoHolderPanel.AddChild(inputButton);


            UIButton resetButton = new GameObject().AddComponent<UIButton>();
            resetButton.SetSize(25, 25, false);

            resetButton.HorizontalAnchor = HorizontalAnchorPoint.RIGHT;
            resetButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            resetButton.HorizontalAlignment = HorizontalAnchorPoint.RIGHT;
            resetButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            resetButton.Text = "X";

            resetButton.buttonCallback = new InputButtonCallback(OnDeleteInputButtonClicked, inputButton, item);

            InputInfoHolderPanel.AddChild(resetButton);

            positionY += PanelHeight;
        }
    }

    public void OnBackButtonClicked(UIButton button)
    {
        SwitchToState(new MainMenuState());
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
