
public delegate void InputButtonClicked(UIButton button, InputInfo info);

public class InputButtonCallback : UIButtonCallback
{
    private InputButtonClicked callBackMethod;

    private UIButton button;
    private InputInfo info;

    public InputButtonCallback(InputButtonClicked del, UIButton button, InputInfo info)
    {
        callBackMethod = del;
        this.button = button;
        this.info = info;
    }

    public override void CallBack()
    {
        callBackMethod(button, info);
    }
}

