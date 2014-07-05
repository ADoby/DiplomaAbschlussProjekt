using UnityEngine;
using System.Collections;

public class UIButtonSelectClass : MonoBehaviour
{

    public PlayerClasses playerClass;

	// Use this for initialization
	void Start ()
	{
	    GetComponent<UIButton>().OnButtonClicked += OnButtonPressed;
	}

    public void OnButtonPressed(UIRect sender)
    {
        GameManager.Instance.SelectClass((int)playerClass);
    }
}
