using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public enum PlayerClasses
{
    BORO,
    JACK,
    COUNT
}

public enum PlayerSlots
{
    ONE,
    TWO,
    THREE,
    FOUR,
    COUNT
}

public class GameManager : MonoBehaviour {

    #region Singleton

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if(!instance)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;

        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    #endregion

    public int currentPlayerSelectingClass = 0;

    public UIButton[] slotButtons;

    public Transform SpawnPosition;

    public List<PlayerController> Players = new List<PlayerController>();
    public List<CameraController> Cameras = new List<CameraController>();

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            Players.Add(null);
            Cameras.Add(null);
        }
    }

    public void RemovePlayer(PlayerController player)
    {
        if (player == null)
            return;

        int index = Players.IndexOf(player);

        Destroy(Cameras[index].gameObject);
        Cameras[index] = null;

        GameEventHandler.TriggerPlayerLeft(player, player.Name);

        Players[index] = null;
        Destroy(player.gameObject);
    }

    public PlayerController MainPlayer
    {
        get
        {
            return Players[0];
        }
    }

    void Update()
    {
        if (currentPlayerSelectingClass != -1)
        {
            slotButtons[currentPlayerSelectingClass].Text = "Player " + (currentPlayerSelectingClass + 1) + " Select";
            slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.red;
        }
    }

    public int PlayerCount
    {
        get { return Players.Count(o => o != null); }
    }

    public void StartGame()
    {
        foreach (var playerController in Players)
        {
            if (playerController)
            {
                playerController.gameObject.SetActive(true);
                playerController.OnReset();
            }
        }

        int camIndex = 0;

        foreach (var cameraController in Cameras)
        {
            if (cameraController)
            {
                if (PlayerCount == 2)
                {
                    if (camIndex == 0)
                    {
                        cameraController.camera.rect = new Rect(0, 0, 1f, 0.5f);
                    }
                    if (camIndex == 1)
                    {
                        cameraController.camera.rect = new Rect(0, 0.5f, 1f, 0.5f);
                    }
                }

                cameraController.gameObject.SetActive(true);

                camIndex++;
            }
        }

        GameContainer.SetActive(true);
    }

    public void SelectSlot(int id)
    {
        slotButtons[currentPlayerSelectingClass].Text = "Slot " + (currentPlayerSelectingClass + 1);
        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.white;

        RemovePlayer(Players[id]);

        currentPlayerSelectingClass = id;
    }

    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public PlayerClass[] PlayerClasses;

    public GameObject GameContainer;

    public void SelectClass(int id)
    {
        GameObject newPlayer = (GameObject)Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        newPlayer.SetActive(false);
        Players[currentPlayerSelectingClass] = newPlayer.GetComponent<PlayerController>();

        GameObject newCam = (GameObject) Object.Instantiate(cameraPrefab, new Vector3(0,0,-10), Quaternion.identity);
        newCam.SetActive(false);

        Cameras[currentPlayerSelectingClass] = newCam.GetComponent<CameraController>();
        Cameras[currentPlayerSelectingClass].player = newPlayer.transform;

        Players[currentPlayerSelectingClass].PlayerId = currentPlayerSelectingClass;


        PlayerClass playerClass = (PlayerClass) Object.Instantiate(PlayerClasses[id]);
        playerClass.transform.parent = newPlayer.transform;
        Players[currentPlayerSelectingClass].PlayerClass = playerClass;

        slotButtons[currentPlayerSelectingClass].Text = "Selected: " + Players[currentPlayerSelectingClass].PlayerClass.name.Replace("(Clone)", "");
        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.green;


        currentPlayerSelectingClass = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Players[i] == null)
            {
                currentPlayerSelectingClass = i;
                break;
            }
        }
    }

    public bool GamePaused = false;

    public static Vector3 GetSpawnPosition()
    {
        return Instance.SpawnPosition.position;
    }

    public void OnPause()
    {
        GamePaused = true;
    }

    public void OnResume()
    {
        GamePaused = false;
    }
}
