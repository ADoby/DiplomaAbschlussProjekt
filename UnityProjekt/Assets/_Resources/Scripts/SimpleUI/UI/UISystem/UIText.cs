using UnityEngine;

public class UIText : UIRect
{
    [Multiline(5)]
    public string Text = "";

    public float fontSize = 1.0f;

    public GUIStyle FontStyle;

    public override void DrawMe()
    {
        if (FontStyle == null)
            FontStyle = new GUIStyle(GUI.skin.label);

        FontStyle.fontSize = (int)(absoluteRect.height / (GUITools.MaxFontSize - fontSize));

        GUI.Label(absoluteRect, Text, FontStyle);
    }
}

