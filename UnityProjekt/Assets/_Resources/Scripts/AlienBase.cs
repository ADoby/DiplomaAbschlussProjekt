using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AlienBase : MonoBehaviour
{

    [SerializeField] 
    public List<AlienBaseState> StateOrder;

    private int currentState = 0;

    public float currentTime = 0;

	// Update is called once per frame
	void Update () {

        if (StateOrder[currentState].Update())
        {
	        NextState();
        }

        currentTime = StateOrder[currentState].currentTime;
        foreach (Transform child in transform)
        {
            child.gameObject.SendMessage("UpdateTimer", currentTime, SendMessageOptions.DontRequireReceiver);
        }
        
	}

    private void NextState()
    {
        if(currentState < StateOrder.Count - 1)
            currentState++;
    }

}
