using UnityEngine;

public class SimpleMenuManager : MonoBehaviour
{

    #region Singleton

    private static SimpleMenuManager instance;

    public static SimpleMenuManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    #endregion

    public MenuState currentMenuState;

	void Start () {
        OpenMenu();
	}
	
	void Update () {
        if(currentMenuState != null)
            currentMenuState.Update();
	}

    void OnGUI()
    {
        if (instance == null)
            instance = this;

        if (currentMenuState != null)
            currentMenuState.OnGUI();
        else
            OpenMenu();
    }

    public void SwitchMenuState(MenuState newMenuState)
    {
        currentMenuState = newMenuState;
    }

    public void OpenMenu()
    {
        currentMenuState = new MainMenuState();
    }

    public void CloseMenu()
    {
        currentMenuState = null;
    }
}
