using UnityEngine;

[ExecuteInEditMode]
public class UIMasterPanel : UIRect
{
    public bool ShowBackground = false;

    void Start()
    {
        UpdateHierarchy();
    }

    void OnGUI()
    {
        UpdateUI();
        Draw();
    }

    public override void Draw()
    {
        if (ShowBackground) GUI.Box(absoluteRect, "");

        if (Visible)
        {
            DrawChildren();
        }
    }
}

