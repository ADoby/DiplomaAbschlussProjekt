using UnityEngine;

[ExecuteInEditMode]
public class UIPanel : UIRect
{
    public bool ShowBox = true;
    protected string content = "";

    public override void Draw()
    {
        if (Visible)
        {
            if(ShowBox) GUI.Box(absoluteRect, content);
            DrawChildren();
        }
    }
}
