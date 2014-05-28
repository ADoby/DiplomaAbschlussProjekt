using UnityEngine;

public abstract class MenuState
{
    public abstract void Update();

    public abstract void OnGUI();

    protected void SwitchToState(MenuState newMenuState)
    {
        SimpleMenuManager.Instance.SwitchMenuState(newMenuState);
    }

    protected void CloseMenu()
    {
        SimpleMenuManager.Instance.CloseMenu();
    }

    protected void ExitGame()
    {
        Application.Quit();
    }
}
