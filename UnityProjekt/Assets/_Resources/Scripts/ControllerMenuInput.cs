using UnityEngine;
using System.Collections;

[System.Serializable]
public class HorizontalUIButtonLayout
{
    public UIButton[] buttons;
}

[System.Serializable]
public class ControllerMenuInput : MonoBehaviour {

    public HorizontalUIButtonLayout[] lines;

    [SerializeField]
    private int currentLine = 0;
    [SerializeField]
    private int currentColumn = 0;

	// Use this for initialization
	void Start () {
        if (lines.Length == 0 || (lines.Length > 0 && lines[0].buttons.Length == 0))
            enabled = false;
	}

    void Awake(){
        GameEventHandler.StopControllerMenu += StopControllerMenu;
    }

    void OnDisable()
    {
        StopControllerMenu();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.AllowMenuInput)
            return;

        int lineBefore = currentLine;
        int columnBefore = currentColumn;
        bool input = false;
        if (InputController.GetClicked("MENUUP"))
        {
            input = true;
            if (currentLine > 0)
                currentLine--;
        }
        else if (InputController.GetClicked("MENUDOWN"))
        {
            input = true;
            if (currentLine + 1 < lines.Length)
                currentLine++;
        }
        if (InputController.GetClicked("MENULEFT"))
        {
            input = true;
            if (currentColumn > 0)
                currentColumn--;
        }
        else if (InputController.GetClicked("MENURIGHT"))
        {
            input = true;
            if (currentColumn + 1 < lines[currentLine].buttons.Length)
                currentColumn++;
        }

        if (InputController.GetClicked("MENUSELECT"))
            lines[currentLine].buttons[currentColumn].ThrowClicked();

        if (input)
        {
            if (currentLine != lineBefore)
            {
                if (currentColumn >= lines[currentLine].buttons.Length)
                {
                    currentColumn = lines[currentLine].buttons.Length - 1;
                }
            }

            lines[lineBefore].buttons[columnBefore].SetForceHover(false);
            lines[currentLine].buttons[currentColumn].SetForceHover(true);
        }
	}

    public void StopControllerMenu()
    {
        //lines[currentLine].buttons[currentColumn].SetForceHover(false);
    }
}
