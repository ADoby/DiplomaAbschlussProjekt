using UnityEngine;

public class UpdateInputControlWaitingForInput : MonoBehaviour
{
    public UIRect DisabledUiRectRect;

    public UIRect myUIRect;

    public UIButton inputButton;

    private string playerNumber = "";
    private string actionName = "";

    private string inputString = "";

    private bool playerInput = true;

    private bool rebindSend = false;

    void Awake()
    {
        myUIRect = GetComponent<UIRect>();
    }

    public void RebindKey(UIButton button, string action, bool playerInput = true)
    {
        rebindSend = false;
        inputButton = button;
        this.playerInput = playerInput;
        inputString = action;

        if (playerInput)
        {
            string[] args = action.Split('_');
            playerNumber = args[0];
            actionName = args[1];
        }
        else
        {
            playerNumber = "";
            actionName = action;
        }
        
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
	    if (!myUIRect.Visible || !rebindSend)
	        return;

	    if (InputController.GetClicked("ESCAPE"))
	    {
	        InputController.Instance.CancelRebind();

            ResetUI();
	    }
	    else
	    {
            if (InputController.Instance.CheckForInput())
            {
                if (playerInput)
                {
                    inputButton.Text = "Player: " + playerNumber + " Action: " + actionName + "\n" + InputController.Instance.GetInfo(playerNumber + "_" + actionName).GetInfo();
                }
                else
                {
                    inputButton.Text = "Action: " + actionName + "\n" + InputController.Instance.GetInfo(actionName).GetInfo();
                }
                ResetUI();
            }
	    }
	}

    void LateUpdate()
    {
        if (myUIRect.Visible && !rebindSend)
        {
            InputController.Instance.RebindKey(inputString);
            rebindSend = true;

            GameManager.AllowMenuInput = false;
        }
        else if (!myUIRect.Visible && rebindSend)
        {

            GameManager.AllowMenuInput = true;
        }
    }
}
