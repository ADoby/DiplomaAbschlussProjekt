using UnityEngine;

public enum SettingsMenuState
{
    DEFAULT,
    DISPLAY,
    INPUT
}

public class SettingsMenu : MenuState
{
    public SettingsMenu()
    {

    }

    public override void Update()
    {
        if (InputController.Instance.IsWaitingForInput)
            InputController.Instance.Update();
    }

    public override void OnGUI()
    {
        GUITools.BackgroundBox("Settings Menu");

        if (GUILayout.Button("Close Game"))
        {
            Application.Quit();
        }

        if (GUILayout.Button("Reset Player Health + Position"))
        {
            InputController.Close();
            GameEventHandler.TriggerReset();
        }

        if (GUILayout.Button("Reset Inputs"))
        {
            //ResetInputs();
        }

        if (InputController.Instance.IsWaitingForInput)
        {
            GUILayout.Label("Waiting for Input");
        }
        else
        {
            foreach (var item in InputController.Instance.InputInfos)
            {
                if (GUILayout.Button(item.Action + " : " + item.GetInfo()))
                {
                    InputController.Instance.RebindKey(item);
                    
                }
            }
        }

        if (GUITools.CenteredButton(0, 0, 100, 20, "Back"))
        {
            SwitchToState(new MainMenuState());
        }
    }
}
