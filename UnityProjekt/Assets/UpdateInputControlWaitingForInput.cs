using UnityEngine;
using System.Collections;

public class UpdateInputControlWaitingForInput : MonoBehaviour
{
    public UIRect DisabledUiRectRect;

    public UIRect myUIRect;

    public UIButton inputButton;

    private string playerNumber = "";
    private string actionName = "";

    void Awake()
    {
        myUIRect = GetComponent<UIRect>();
    }

    public void RebindKey(UIButton button, string action)
    {
        inputButton = button;
        InputController.Instance.RebindKey(action);

        string[] args = action.Split('_');
        playerNumber = args[0];
        actionName = args[1];
        
        DisabledUiRectRect.Enabled = false;
        myUIRect.Visible = true;
    }

	// Update is called once per frame
	void Update ()
	{
	    if (!myUIRect.Visible)
	        return;

	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        InputController.Instance.CancelRebind();

            myUIRect.Visible = false;
	    }
	    else
	    {
            if (InputController.Instance.CheckForInput())
            {
                //Finished
                if (DisabledUiRectRect)
                    DisabledUiRectRect.Enabled = true;


                inputButton.Text = "Player: " + playerNumber + " Action: " + actionName + "\n" + InputController.Instance.GetInfo(playerNumber + "_" + actionName).GetInfo();

                myUIRect.Visible = false;
            }
	    }
	    
	}
}
