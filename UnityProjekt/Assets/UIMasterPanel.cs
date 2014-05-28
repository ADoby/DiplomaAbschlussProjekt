using UnityEngine;

[ExecuteInEditMode]
public class UIMasterPanel : UIRect
{
    void OnGUI()
    {
        UpdateChildren();
        Draw();
    }

    public override void Draw()
    {
        DrawChildren();
    }
}

