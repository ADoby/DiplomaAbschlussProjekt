using UnityEngine;

using System.Collections.Generic;

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

    private Dictionary<string, MenuState> initiatedMenuStates;

	void Start () {
        initiatedMenuStates = new Dictionary<string, MenuState>();
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

    public void SwitchMenuState<T>()
        where T : MenuState, new()
    {
        if (initiatedMenuStates.ContainsKey(typeof(T).ToString()))
        {
            currentMenuState = initiatedMenuStates[typeof(T).ToString()];
        }
        else
        {
            currentMenuState = new T();
            initiatedMenuStates.Add(typeof(T).ToString(), currentMenuState);
        }
    }

    public void OpenMenu()
    {
        SwitchMenuState<MainMenuState>();
    }

    public void CloseMenu()
    {
        currentMenuState = null;
    }
}
