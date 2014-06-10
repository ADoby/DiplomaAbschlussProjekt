public class StartGameMenuState : MenuState
{
    public override void Update()
    {

    }

    public override void OnGUI()
    {
        GUITools.BackgroundBox("Start Game Menu");

        if (GUITools.CenteredButton(0, -30, 100, 20, "Singleplayer"))
        {
            GameManager.Instance.StartSingleplayerGame();
            CloseMenu();
        }

        if (GUITools.CenteredButton(0, 0, 100, 20, "Multiplayer Test"))
        {
            GameManager.Instance.StartMultiplayerGame();
            CloseMenu();
        }

        if (GUITools.CenteredButton(0, 30, 100, 20, "Back"))
        {
            SwitchToState<MainMenuState>();
        }
    }
}

