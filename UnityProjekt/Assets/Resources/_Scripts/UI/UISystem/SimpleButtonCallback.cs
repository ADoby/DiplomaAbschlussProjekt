
public delegate void SimpleButtonClicked(UIButton button);


public class SimpleButtonCallback : UIButtonCallback
{
    private SimpleButtonClicked callBackMethod;

    private UIButton button;

    public SimpleButtonCallback(SimpleButtonClicked del, UIButton button)
    {
        callBackMethod = del;
        this.button = button;
    }

    public override void CallBack()
    {
        callBackMethod(button);
    }
}

