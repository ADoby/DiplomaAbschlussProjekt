using UnityEngine;
using System.Collections;

public class GrowingPart : MonoBehaviour
{

    public float minTime = 0.0f, maxTime = 1.0f;

    public Vector3 scaleStart;
    public Vector3 scaleEnd;

    [ContextMenu("Current scale as EndScale")]
    public void GetTransformEndScale()
    {
        scaleEnd = transform.localScale;
    }

    [ContextMenu("Current scale as EndScale - All Children")]
    public void GetTransformEndScaleChildren()
    {
        GetTransformEndScale();
        foreach (Transform child in transform)
        {
            child.gameObject.SendMessage("GetTransformEndScaleChildren", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void UpdateTimer(float time)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SendMessage("UpdateTimer", time, SendMessageOptions.DontRequireReceiver);
        }

        time = Mathf.Clamp(time, minTime, maxTime);

        float prozent = (time-minTime)/(maxTime-minTime);

        Vector3 wantedScale = scaleStart + (scaleEnd - scaleStart)*prozent;
        transform.localScale = wantedScale;

        
    }

	// Use this for initialization
	void Start ()
	{
	    GetTransformEndScale();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
