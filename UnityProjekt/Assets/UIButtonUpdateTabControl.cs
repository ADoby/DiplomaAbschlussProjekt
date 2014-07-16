using UnityEngine;
using System.Collections;

public class UIButtonUpdateTabControl : MonoBehaviour {

    public Tab_Control control;

    public int tabIndex = 0;

	// Use this for initialization
	void Start () {
        GetComponent<UIButton>().OnButtonClicked += OnButtonClicked;
	}

    public void OnButtonClicked(UIRect rect)
    {
        control.SetTab(tabIndex);
    }
}
