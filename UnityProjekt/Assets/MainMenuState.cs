using UnityEngine;

public class MainMenuState : MenuState
{
    private const float ButtonWidth = 100f, ButtonHeight = 20f;

    public override void Update()
    {
        
    }

    public override void OnGUI()
    {
        GUITools.BackgroundBox("Main Menu");

        if (GUITools.CenteredButton(0, -30, ButtonWidth, ButtonHeight, "Start Game"))
        {
            SwitchToState(new StartGameMenuState());
        }

        if (GUITools.CenteredButton(0, 0, ButtonWidth, ButtonHeight, "Settings"))
        {
            SwitchToState(new SettingsMenuState());
        }

        if (GUITools.CenteredButton(0, 30, ButtonWidth, ButtonHeight, "Exit Game"))
        {
            ExitGame();
        }
    }
}

