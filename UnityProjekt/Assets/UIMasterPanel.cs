using UnityEngine;

[ExecuteInEditMode]
public class UIMasterPanel : UIRect
{
    void OnGUI()
    {
        UpdateUI();
        Draw();
    }

    public override void Draw()
    {
        DrawChildren();
    }
}

