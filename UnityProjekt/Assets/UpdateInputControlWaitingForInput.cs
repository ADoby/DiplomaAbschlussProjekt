using UnityEngine;

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

    public void ResetUI()
    {
        if (DisabledUiRectRect)
            DisabledUiRectRect.Enabled = true;

        myUIRect.Visible = false;
    }

	// Update is called once per frame
	void Update ()
	{
	    if (!myUIRect.Visible)
	        return;

	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        InputController.Instance.CancelRebind();

            ResetUI();
	    }
	    else
	    {
            if (InputController.Instance.CheckForInput())
            {
                inputButton.Text = "Player: " + playerNumber + " Action: " + actionName + "\n" + InputController.Instance.GetInfo(playerNumber + "_" + actionName).GetInfo();

                ResetUI();
            }
	    }
	    
	}
}
