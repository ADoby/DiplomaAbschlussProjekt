using UnityEngine;

[System.Serializable]
public class MenuState
{
    public virtual void Update() { }

    public virtual void OnGUI() { }

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
