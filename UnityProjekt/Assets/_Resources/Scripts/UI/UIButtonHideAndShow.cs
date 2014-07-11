using UnityEngine;
using System.Collections;

public class UIButtonHideAndShow : MonoBehaviour
{

    public UIRect[] HideRects;
    public UIRect[] ShowRects;

	// Use this for initialization
	void Start ()
	{
	    GetComponent<UIButton>().OnButtonClicked += OnButtonPressed;
	}

    public void OnButtonPressed(UIRect sender)
    {
        foreach (var hideRect in HideRects)
        {
            hideRect.Visible = false;
        }
        foreach (var showRect in ShowRects)
        {
            showRect.Visible = true;
        }
    }
}
