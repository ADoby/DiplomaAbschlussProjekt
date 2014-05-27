using UnityEngine;

public abstract class MenuState
{
    public abstract void Update();

    public abstract void OnGUI();

    protected void SwitchToState(MenuState newMenuState)
    {
        MenuManager.Instance.SwitchMenuState(newMenuState);
    }

    protected void CloseMenu()
    {
        MenuManager.Instance.CloseMenu();
    }

    protected void ExitGame()
    {
        Application.Quit();
    }
}
