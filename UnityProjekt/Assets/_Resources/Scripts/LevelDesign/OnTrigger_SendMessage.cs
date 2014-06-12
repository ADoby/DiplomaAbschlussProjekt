using UnityEngine;
using System.Collections;

public class OnTrigger_SendMessage : MonoBehaviour
{
    public GameObject go;
    public string SendMessageString = "Message";
    public LayerMask triggeredMask;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && Tools.IsInLayerMask(other.gameObject, triggeredMask))
        {
            go.SendMessage(SendMessageString, SendMessageOptions.DontRequireReceiver);
        }
    }
}
