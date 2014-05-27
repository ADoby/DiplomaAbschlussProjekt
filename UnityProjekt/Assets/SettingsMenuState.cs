public class SettingsMenuState : MenuState
{
    public override void Update()
    {

    }

    public override void OnGUI()
    {
        GUITools.BackgroundBox("Settings Menu");

        if (GUITools.CenteredButton(0, 0, 100, 20, "Back"))
        {
            SwitchToState(new MainMenuState());
        }
    }
}
