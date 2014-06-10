using UnityEngine;

[System.Serializable]
public class UIPanel : UIRect
{
    public bool ShowBox = true;
    public string content = "";

    public override void DrawMe()
    {
        if (ShowBox)
            GUI.Box(absoluteRect, content);
    }
}
