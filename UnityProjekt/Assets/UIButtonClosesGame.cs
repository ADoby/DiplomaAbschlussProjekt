using UnityEngine;
using System.Collections;

public class UIButtonClosesGame : MonoBehaviour
{

    void Start()
    {
        UIButton button = GetComponent<UIButton>();
        if(button)
            button.OnButtonClicked += OnButtonClicked;
    }

    public void OnButtonClicked(UIButton button)
    {
        Application.Quit();
    }
}
