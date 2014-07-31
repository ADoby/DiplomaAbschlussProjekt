using UnityEngine;

public delegate void InputButtonClicked(UIButton button, InputInfo info);

[System.Serializable]
public class InputButtonCallback : UIDefaultCallback
{
    [SerializeField]
    private InputButtonClicked callBackMethod;

    [SerializeField]
    private UIButton button;
    [SerializeField]
    private InputInfo info;

    public InputButtonCallback(InputButtonClicked del, UIButton button, InputInfo info)
    {
        callBackMethod = del;
        this.button = button;
        this.info = info;
    }

    public override void CallBack(UIRect sender)
    {
        callBackMethod(button, info);
    }
}

