using UnityEngine;
using System.Collections;

public class ResetAnimation : MonoBehaviour {

    public Animator anim;
    public string triggerName = "Do";

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Reset()
    {
        if (anim)
        {
            anim.SetTrigger(triggerName);
        }
    }
}
