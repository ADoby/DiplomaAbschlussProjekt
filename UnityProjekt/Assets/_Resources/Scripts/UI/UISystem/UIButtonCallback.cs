using UnityEngine;

[System.Serializable]
public class UIButtonCallback
{
    public GameObject MessageReciever;
    public string MethodName = "";

    public virtual void CallBack()
    {
        MessageReciever.SendMessage(MethodName, SendMessageOptions.DontRequireReceiver);
    }
}
