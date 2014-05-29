using UnityEngine;

[System.Serializable]
public class UIButton : UIRect
{
    public string Text = "";

    public UIButtonCallback buttonCallback;

    public override void DrawMe()
    {
        if (GUI.Button(absoluteRect, Text) && buttonCallback != null)
        {
            buttonCallback.CallBack();
        }
    }
}
