using UnityEngine;

public class UIText : UIRect
{
    public bool ShowBackground = false;
    public string Text = "";

    public override void Draw()
    {
        if (Visible)
        {
            if (ShowBackground)
                GUI.Box(absoluteRect, "");

            GUI.Label(absoluteRect, Text);
        }

        
    }
}

