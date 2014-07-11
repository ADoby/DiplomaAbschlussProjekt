using UnityEngine;
using System.Collections;

public class HealthToBaseTime : MonoBehaviour
{

    public EnemieBase enemieBase;
    public AlienBase alienBase;

	// Use this for initialization
	void Start ()
	{
	    alienBase.UpdateTime = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
        alienBase.SetCurrentTime(alienBase.minNeededTime + enemieBase.ProzentHealth() * alienBase.maxNeededTime);
	}
}
