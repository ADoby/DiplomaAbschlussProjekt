using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Pixelate")]
public class PixelateEffect : MonoBehaviour {

    public Vector2 resolution = new Vector2(800, 600);

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture small = RenderTexture.GetTemporary((int)resolution.x, (int)resolution.y);
        small.filterMode = FilterMode.Point;

        source.filterMode = FilterMode.Point;

        Graphics.Blit(source, small);
        Graphics.Blit(small, destination);
        RenderTexture.ReleaseTemporary(small);
    }
}
