using UnityEngine;

[System.Serializable]
public class UIButton : UIRect
{

    [Multiline(3)]
    public string Text = "";

    public UIDefaultCallback Callback;

    public float fontSize = 1.0f;

    public GUIStyle ButtonStyle;

    public delegate void ButtonEvent(UIButton sender);

    public ButtonEvent OnButtonClicked;
    public static ButtonEvent OnAnyButtonClicked;

    public override void DrawMe()
    {
        if (ButtonStyle == null)
            ButtonStyle = new GUIStyle(GUI.skin.button);

        ButtonStyle.fontSize = (int)(absoluteRect.height / (GUITools.MaxFontSize - fontSize));

        
        if (GUI.Button(absoluteRect, Text, ButtonStyle))
        {
            if (Callback != null)
                Callback.CallBack(this);

            if (OnButtonClicked != null)
                OnButtonClicked(this);

            if (OnAnyButtonClicked != null)
                OnAnyButtonClicked(this);
        }
    }
}
