using UnityEngine;
using System.Collections;

public class UIButtonActivateAndDeactivate : MonoBehaviour
{

    public GameObject[] DeactivateObjects;
    public GameObject[] ActivateObjects;

	// Use this for initialization
	void Start ()
	{
	    UIButton myButton = GetComponent<UIButton>();
	    if (myButton)
	    {
	        myButton.OnButtonClicked += OnButtonClicked;
	    }
	}

    public void OnButtonClicked(UIRect rect)
    {
        foreach (var deactivateObject in DeactivateObjects)
        {
            deactivateObject.SetActive(false);
        }
        foreach (var activateObject in ActivateObjects)
        {
            activateObject.SetActive(true);
        }
    }

}
