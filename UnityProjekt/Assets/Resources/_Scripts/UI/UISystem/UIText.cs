using UnityEngine;

public class UIText : UIRect
{
    public string Text = "";

    public override void DrawMe()
    {
        GUI.Label(absoluteRect, Text);
    }
}

