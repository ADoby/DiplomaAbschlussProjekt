using UnityEngine;
using System.Collections;

public class RTtest : MonoBehaviour
{
    public float refresh = 1f;

    // Update is called once per frame
    void Start()
    {
        InvokeRepeating("Simulate", 0f, refresh);
    }

    void SimulateRenderTexture()
    {
        renderer.material.mainTexture = RenderTextureFree.Capture();
        renderer.enabled = true;
    }

    void Simulate()
    {
        renderer.enabled = false;
        Invoke("SimulateRenderTexture", 0f);
    }
}