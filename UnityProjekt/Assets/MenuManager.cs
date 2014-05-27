using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{

    #region Singleton

    private static MenuManager instance;

    public static MenuManager Instance
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

    private MenuState currentMenuState;

	void Start () {
        currentMenuState = new MainMenuState();
	}
	
	void Update () {
        if(currentMenuState != null)
            currentMenuState.Update();
	}

    void OnGUI()
    {
        if (currentMenuState != null)
            currentMenuState.OnGUI();
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
