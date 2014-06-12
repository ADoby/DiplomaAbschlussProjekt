using UnityEngine;
using System.Collections;
using System;

public enum Axis
{
    XY,
    COUNT
}

public class CameraController : MonoBehaviour {

    public Transform player;

    [Range(0f, 10f)]
    public float distance = 1.0f;

    [Range(0.1f, 10.0f)]
    public float damping = 3.0f;

    public Vector3 positionDiff = new Vector3(0, 0, 0);
    public Vector3 positionDiffCave = new Vector3(0, 0, 0);
    public Vector3 positionDiffOutside = new Vector3(0, 0, 0);

    public float size = 10f;
    public float sizeOutside = 10f;
    public float sizeCave = 5f;

    public Axis axis;

    public float changeDamping = 2.0f;

    private float cameraWantedSize = 10;
    private Vector3 wantedPositionDiff = new Vector3(0, 0, 0);

    private Vector3 lastPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
        if (!player)
            enabled = false;

        cameraWantedSize = size;
        wantedPositionDiff = positionDiff;

        lastPosition = transform.position;
	}

    public void PlayerInSide()
    {
        cameraWantedSize = sizeCave;
        wantedPositionDiff = positionDiffCave;
    }

    public void PlayerOutSide()
    {
        cameraWantedSize = sizeOutside;
        wantedPositionDiff = positionDiffOutside;
    }

	// Update is called once per frame
	void Update () {
        Vector3 wantedPosition = transform.position;
        Vector3 playerPosition = player.position;
        playerPosition.z = wantedPosition.z;

        positionDiff = Vector3.Lerp(positionDiff, wantedPositionDiff, Time.deltaTime * changeDamping);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraWantedSize, Time.deltaTime * changeDamping);

        switch (axis)
        {
            case Axis.XY:
                wantedPosition += positionDiff;
                //Check Distance
                if (Vector3.Distance(wantedPosition, playerPosition) > distance)
                {
                    wantedPosition = playerPosition + (wantedPosition - playerPosition).normalized * distance + positionDiff;
                }
                break;
            case Axis.COUNT:
            default:
                throw new ArgumentOutOfRangeException();
        }

        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

        GameEventHandler.TriggerCameraMoved(transform.position - lastPosition);
        lastPosition = transform.position;
    }
}
