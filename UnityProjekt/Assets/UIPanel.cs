using UnityEngine;

[ExecuteInEditMode]
public class UIPanel : UIRect
{
    protected string content = "";

    public override void Draw()
    {
        GUI.Box(rect, content);
        DrawChildren();
    }
}
