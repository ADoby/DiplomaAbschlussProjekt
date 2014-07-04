using UnityEngine;
using System.Collections;

public class UITexture : UIRect
{

    public Texture Texture;

    public ScaleMode ScaleMode;

    public override void DrawMe()
    {
        GUI.DrawTexture(absoluteRect, Texture, ScaleMode);
    }
}
