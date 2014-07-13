using UnityEngine;
using System.Collections;

public class RebindKeyOnButtonPress : MonoBehaviour
{
    public bool playerInput = true;

    public string playerNumber = "0";
    public string actionName = "";

    public string inputString { get; set; }

    public UpdateInputControlWaitingForInput WaitForInputGameObject;

	// Use this for initialization
	void Start ()
	{
	    UIButton button = GetComponent<UIButton>();
	    button.OnButtonClicked += OnUIButtonClicked;

        if (playerInput)
        {
            inputString = playerNumber + "_" + actionName;

            button.Text = "Player: " + playerNumber + " Action: " + actionName + "\n" + InputController.Instance.GetInfo(inputString).GetInfo();

        }
        else
        {
            inputString = actionName;

            button.Text = "Action: " + actionName + "\n" + InputController.Instance.GetInfo(inputString).GetInfo();

        }
	    
	}

    public void OnUIButtonClicked(UIRect sender)
    {
        WaitForInputGameObject.RebindKey((UIButton)sender, inputString, playerInput);
    }
}
