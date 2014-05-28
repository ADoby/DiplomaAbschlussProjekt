using UnityEngine;

public class UIButton : UIRect
{
    public string Text = "";

    public GameObject MessageReciever;
    public string MethodName = "";

    public override void Draw()
    {
        if (Visible)
        {
            if (GUI.Button(absoluteRect, Text) && MessageReciever != null && MethodName.Equals("") == false)
            {
                MessageReciever.SendMessage(MethodName);
            }
        }


    }
}
