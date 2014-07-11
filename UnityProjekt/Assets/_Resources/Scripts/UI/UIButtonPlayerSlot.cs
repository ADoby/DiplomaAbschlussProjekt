using UnityEngine;
using System.Collections;

public class UIButtonPlayerSlot : MonoBehaviour {

    public PlayerSlots PlayerSlots;

    // Use this for initialization
    void Start()
    {
        GetComponent<UIButton>().OnButtonClicked += OnButtonPressed;
    }

    public void OnButtonPressed(UIRect sender)
    {
        GameManager.Instance.SelectSlot((int)PlayerSlots);
    }
}
