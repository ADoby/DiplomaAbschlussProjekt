using UnityEngine;

public class UIText : UIRect
{
    public string Text;

    public override void Draw()
    {
        GUI.Label(rect, Text);
    }
}

