using UnityEngine;
using System.Collections;

public class ImitateMainPlayer : MonoBehaviour
{

    public static Transform mainPlayer;

    void Awake()
    {
        mainPlayer = transform;
    }

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
