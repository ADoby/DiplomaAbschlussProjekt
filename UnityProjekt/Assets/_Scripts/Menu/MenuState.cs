using UnityEngine;

[System.Serializable]
public class MenuState
{
    public virtual void Update() { }

    public virtual void OnGUI() { }

    protected void SwitchToState<T>()
        where T : MenuState, new()
    {
        SimpleMenuManager.Instance.SwitchMenuState<T>();
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
