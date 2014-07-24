using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

    public Transform player;
    public PlayerController playerControl;

    [Range(0f, 10f)]
    public float distance = 1.0f;

    [Range(0.1f, 10.0f)]
    public float damping = 3.0f;

    public Vector3 positionDiff = Vector3.zero;
    public Vector3 currentExtraDiff = Vector3.zero;

    public float size = 10f;

    public float changeDamping = 2.0f;

    private float cameraWantedSize = 10;
    private Vector3 wantedPositionDiff = Vector3.zero;

    public float crouchTimer = 0f;
    public float crouchMinTime = 1.0f;

	// Use this for initialization
	void Start () {
        if (!player)
            enabled = false;

        cameraWantedSize = (Screen.height / size) / 2f;
        wantedPositionDiff = positionDiff;

	}

	// Update is called once per frame
	void Update () {
        cameraWantedSize = (Screen.height / size) / 2f;

        if (playerControl.Crouching)
        {
            if(crouchTimer < crouchMinTime)
                crouchTimer += Time.deltaTime;
            if (crouchTimer > crouchMinTime)
            {
                currentExtraDiff += Vector3.down * 3f;

                crouchTimer = crouchMinTime;
            }
        }
        else 
        {
            if (crouchTimer == crouchMinTime)
            {
                currentExtraDiff -= Vector3.down * 3f;
            }
            crouchTimer = 0f;
        }


        Vector3 currentPosition = transform.position;
        Vector3 playerPosition = player.position;
        playerPosition.z = currentPosition.z;

        positionDiff = wantedPositionDiff + currentExtraDiff;
        currentPosition = playerPosition + positionDiff;

        //positionDiff = Vector3.Lerp(positionDiff, wantedPositionDiff + currentExtraDiff, Time.deltaTime * changeDamping);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraWantedSize, Time.deltaTime * changeDamping);



        if (Vector3.Distance(transform.position, playerPosition + positionDiff) > distance)
        {
            
            //wantedPosition = playerPosition + (wantedPosition - playerPosition).normalized * distance + positionDiff;
        }

        transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * damping);

    }
}
