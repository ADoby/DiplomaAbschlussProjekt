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
        masterPanel = new UIMasterPanel();
        masterPanel.Position = new UIPosition() { Value = new Vector2(0, 0), normalized = true };
        masterPanel.Size = new UISize() { Value = new Vector2(1, 1), normalized = true };
        masterPanel.ShowBox = false;

        waitingForInputPanel = new UIPanel();
        waitingForInputPanel.Position = new UIPosition() { Value = new Vector2(0, 0), normalized = true };
        waitingForInputPanel.Size = new UISize() { Value = new Vector2(1, 1), normalized = true };
        waitingForInputPanel.content = "Waiting for Input";
        waitingForInputPanel.ShowBox = false;

        inputPanel = new UIPanel();
        inputPanel.Position = new UIPosition() { Value = new Vector2(0, 0), normalized = true };
        inputPanel.Size = new UISize() { Value = new Vector2(1, 1), normalized = true };
        inputPanel.content = "Input Manager";

        masterPanel.AddChild(inputPanel);
        masterPanel.AddChild(waitingForInputPanel);

        UIButton backButton = new UIButton();
        backButton.Position = new UIPosition() { Value = new Vector2(5, 5), normalized = false };
        backButton.Size = new UISize() { Value = new Vector2(120, 25), normalized = false };

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

            UIPanel InputInfoHolderPanel = new UIPanel();
            InputInfoHolderPanel.Position = new UIPosition() { Value = new Vector2(0, positionY), normalized = false };
            InputInfoHolderPanel.Size = new UISize() { Value = new Vector2(250, PanelHeight), normalized = false };

            InputInfoHolderPanel.HorizontalAnchor = HorizontalAnchorPoint.CENTER;
            InputInfoHolderPanel.HorizontalAlignment = HorizontalAnchorPoint.CENTER;

            InputInfoHolderPanel.content = item.Action.Replace("1_", "Player 1 ") + " : ";

            inputPanel.AddChild(InputInfoHolderPanel);

            //Label
            UIText inputLabel = new UIText();
            inputLabel.Position = new UIPosition() { Value = new Vector2(0, 0), normalized = false };
            inputLabel.Size = new UISize() { Value = new Vector2(100, 25), normalized = false };

            inputLabel.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputLabel.HorizontalAlignment = HorizontalAnchorPoint.LEFT;
            inputLabel.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            //inputLabel.Text = item.Action.Replace("1_", "Player 1 ") + " : ";

            InputInfoHolderPanel.AddChild(inputLabel);


            inputButton = new UIButton();
            inputButton.Position = new UIPosition() { Value = new Vector2(80, 0), normalized = false };
            inputButton.Size = new UISize() { Value = new Vector2(120, 25), normalized = false };

            inputButton.HorizontalAnchor = HorizontalAnchorPoint.LEFT;
            inputButton.VerticalAnchor = VerticalAnchorPoint.BOTTOM;
            inputButton.VerticalAlignment = VerticalAnchorPoint.BOTTOM;

            inputButton.Text = item.GetInfo();

            inputButton.buttonCallback = new InputButtonCallback(OnInputButtonClicked, inputButton, item);

            InputInfoHolderPanel.AddChild(inputButton);


            UIButton resetButton = new UIButton();
            resetButton.Position = new UIPosition() { Value = new Vector2(0, 0), normalized = false };
            resetButton.Size = new UISize() { Value = new Vector2(25, 25), normalized = false };

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
