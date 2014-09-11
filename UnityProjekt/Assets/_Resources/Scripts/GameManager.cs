using System.Linq;
using UnityEngine;

public enum PlayerClasses
{
	JACK,
	BORO,
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

[System.Serializable]
public class GameManager : MonoBehaviour {

	public static GameManager Instance;

    public static bool GamePaused = false;

    public static bool AllowMenuInput = true;

	public int CurrentSpawnedEntityCount = 0;
	public int MaxSpawnedEntityCount = 40;

	public GameObject[] levels;
	public int DefaultLevel = 0;
    [SerializeThis]
	private int currentLevel = 0;

	public static bool CanSpawnEntity
	{
		get { return (Instance.CurrentSpawnedEntityCount < Instance.MaxSpawnedEntityCount); }
	}

    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public PlayerClass[] PlayerClasses;

    public GameObject GameContainer;

    public UIRect ReadyButton;

	public void AddEntity()
	{
		CurrentSpawnedEntityCount++;
	}

	public void RemoveEntity()
	{
		CurrentSpawnedEntityCount--;
	}

	public int currentPlayerSelectingClass = 0;

	public UIButton[] slotButtons;

    [SerializeThis]
	private PlayerController[] Players = {null,null,null,null};
    [SerializeThis]
	private CameraController[] Cameras = {null,null,null,null};

	public CameraController[] GetCameras()
	{
		return Cameras;
	}
    public PlayerController[] GetPlayers()
    {
        return Players;
    }

    public float CurrentDifficulty = 0;
	public float DifficultyValue = 0f;
	public float DifficultEveryXSecond = 5f;

    public float LastDifficulty = 0f;
    public float LastDifficultyValue = 0f;
    public float LastLevelDamage = 0f;

    public float CurrentLevelDamage = 0f;

    public UIRect WinScreen;
    public UIRect DifficultyBar;
    public UIText DifficultyText;

    public UIText ProgressText;
    public UIRect ProgressBar;

    void Awake()
    {
        Instance = this;

        LevelSerializer.MaxGames = 2;
    }

	void Start()
	{
		GameEventHandler.TriggerOnPause();
        SelectSlot(0);

        ReadyButton.Enabled = false;
	}

    void OnEnable()
    {
        GameEventHandler.OnDamageDone += OnDamageDone;
        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;

        GameEventHandler.OnCreateCheckpoint += SaveCheckPoint;
        GameEventHandler.OnResetToCheckpoint += LoadLastCheckPoint;
    }

    void OnDisable()
    {
        UnListen();
    }

    void OnDestroy()
    {
        UnListen();   
    }

    void UnListen()
    {
        GameEventHandler.OnDamageDone -= OnDamageDone;
        GameEventHandler.OnPause -= OnPause;
        GameEventHandler.OnResume -= OnResume;

        GameEventHandler.OnCreateCheckpoint -= SaveCheckPoint;
        GameEventHandler.OnResetToCheckpoint -= LoadLastCheckPoint;
    }

    public void StopGame()
    {
        for (int i = 0; i < 4; i++)
        {
            RemovePlayer(Players[i]);

        }

        ResetDifficultyAndDamage();

        SelectSlot(3);
        SelectSlot(2);
        SelectSlot(1);
        SelectSlot(0);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        for (int i = 0; i < LevelSerializer.SavedGames[LevelSerializer.PlayerName].Count; i++)
        {
            LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Delete();
        }
    }

    public void ResetDifficultyAndDamage()
    {
        CurrentDifficulty = 0;
        DifficultyValue = 0;
        CurrentLevelDamage = 0;
        UpdateDifficulty();
    }

    public void ResetLevel()
    {
        if (LevelSerializer.SavedGames[LevelSerializer.PlayerName].Count == 2)
        {
            LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][1].Data, false, true);
        }
        else
        {
            LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data, false, true);
        }
        ResetDifficultyAndDamage();
    }

	public void LoadLevel(int id)
	{
		for (int i = 0; i < levels.Length; i++)
		{
			levels[i].SetActive(false);
		}
		levels[id].SetActive(true);

		currentLevel = id;
		UpdateProgressUI(true);
	}

	public Vector3 LevelSpawnPoint
	{
		get
		{
			return levels[currentLevel].GetComponent<LevelController>().LevelSpawnPoint.position;
		}
	}

	public void RemovePlayer(PlayerController player)
	{
		if (player == null)
			return;

		int index = 0;
		for (int i = 0; i < Players.Length; i++)
		{
			if (Players[i] == player)
			{
				index = i;
				break;
			}
		}

		Destroy(Cameras[index].gameObject);
		Cameras[index] = null;

		GameEventHandler.TriggerPlayerLeft(player, player.Name);

		Players[index] = null;
        EntitySpawnManager.RemoveHitAble(player.GetComponent<HitAble>());
		Destroy(player.gameObject);
	}

	public PlayerController MainPlayer
	{
		get
		{
			for (int i = 0; i < Players.Length; i++)
			{
				if (Players[i] != null)
				{
					return Players[i];
				}
			}
			return null;
		}
	}

	private float NeededLevelDamage
	{
		get
		{
			return levels[currentLevel].GetComponent<LevelController>().NeededLevelDamage;
		}
	}
	

	public void OnDamageDone(PlayerController player, Damage damage)
	{
		CurrentLevelDamage += damage.amount;
        if (CurrentLevelDamage > NeededLevelDamage)
        {
            WinScreen.gameObject.SetActive(true);
            GameEventHandler.TriggerOnPause();
        }
	}

	public void UpdateProgressUI(bool instant = false)
	{
        float progress = instant ? CurrentLevelDamage / NeededLevelDamage : Mathf.Lerp(ProgressBar.RelativeSize.y, CurrentLevelDamage / NeededLevelDamage, Time.deltaTime);


		ProgressText.Text = "Progress: " + progress.ToString("##0.##%");

		ProgressBar.RelativeSize.y = progress;
	}

    void UpdateDifficulty()
    {
        DifficultyValue += Time.deltaTime;
        DifficultyBar.RelativeSize.x = DifficultyValue / DifficultEveryXSecond;
        if (DifficultyValue >= DifficultEveryXSecond)
        {
            CurrentDifficulty++;
            DifficultyValue = 0;

        } 
        DifficultyText.Text = "Difficulty: " + CurrentDifficulty.ToString("####0");
    }

	void Update()
	{
        if (GamePaused)
            return;

        if (InputController.GetClicked("RESETLEVEL"))
        {
            ResetLevel();
        }

        UpdateDifficulty();

		UpdateProgressUI();
	}

	public int PlayerCount
	{
		get { return Players.Count(o => o != null); }
	}

	public void StartGame()
	{

        ResetDifficultyAndDamage();

		foreach (var playerController in Players)
		{
			if (playerController)
			{
				playerController.gameObject.SetActive(true);
				playerController.Init();
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
						cameraController.GetComponent<PlayerUI>().panel.RelativePosition.y = 0.5f;
					}
				}

				cameraController.gameObject.SetActive(true);

				camIndex++;
			}
		}

		GameContainer.SetActive(true);

		GameEventHandler.TriggerOnResume();

        LoadLevel(DefaultLevel);

        LevelSerializer.SaveGame("LevelStart");
	}

    public void SaveCheckPoint()
    {

        if (LevelSerializer.SavedGames[LevelSerializer.PlayerName].Count == 2)
            LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Delete();

        LastDifficulty = CurrentDifficulty;
        LastDifficultyValue = DifficultyValue;
        LastLevelDamage = CurrentLevelDamage;
        LevelSerializer.SaveGame("CheckPoint");

    }

    public void LoadLastCheckPoint()
    {
        LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data, false, true);
        CurrentDifficulty = LastDifficulty;
        DifficultyValue = LastDifficultyValue;
        CurrentLevelDamage = LastLevelDamage;
    }

    public void BackButtonClicked(UIRect sender)
    {
        RemoveAllPlayers();
        SelectSlot(0);
    }

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            RemovePlayer(Players[i]);

            slotButtons[i].Text = "Slot " + (i + 1);
            slotButtons[i].ButtonStyle.normal.textColor = Color.white;
        }
    }

	public void SelectSlot(int id)
	{
        if (id < 0 || id >= MaxPlayers)
            return;

        if (Players[currentPlayerSelectingClass] == null)
        {
            slotButtons[currentPlayerSelectingClass].Text = "Slot " + (currentPlayerSelectingClass + 1);
            slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.white;
        }
        else
        {
            slotButtons[currentPlayerSelectingClass].Text = "Selected: " + Players[currentPlayerSelectingClass].PlayerClass.name.Replace("(Clone)", "");
            slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.green;
        }

        if(id == currentPlayerSelectingClass)
            RemovePlayer(Players[id]);

        currentPlayerSelectingClass = id;

        if (Players[currentPlayerSelectingClass] != null)
        {
            slotButtons[currentPlayerSelectingClass].Text = "Player " + (currentPlayerSelectingClass + 1) + " Select\nClick again to delete";
        }
        else
        {
            slotButtons[currentPlayerSelectingClass].Text = "Player " + (currentPlayerSelectingClass + 1) + " Select";
        }

        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.red;

        UpdateReadyButton();
        UpdateSelectedClassUI();
	}

    public void UpdateSelectedClassUI()
    {
        //TODO: PlayerClass UI mit Informationen über die selektierte Klasse
    }

    private void UpdateReadyButton()
    {
        if (PlayerCount > 0)
        {
            ReadyButton.Enabled = true;
        }
        else
        {
            ReadyButton.Enabled = false;
        }
    }

    public int MaxPlayers = 2;

    private void SelectNextPlayerSlot()
    {
        currentPlayerSelectingClass = -1;
        for (int i = 0; i < MaxPlayers; i++)
        {
            if (Players[i] == null)
            {
                currentPlayerSelectingClass = i;
                break;
            }
        }
        SelectSlot(currentPlayerSelectingClass);
    }

    public void SelectClass(int id)
    {
        RemovePlayer(Players[currentPlayerSelectingClass]);

        GameObject newPlayer = (GameObject)Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        EntitySpawnManager.AddHitAble(newPlayer.GetComponent<HitAble>());

        newPlayer.SetActive(false);
        Players[currentPlayerSelectingClass] = newPlayer.GetComponent<PlayerController>();

        GameObject newCam = (GameObject)Object.Instantiate(cameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
        newCam.SetActive(false);

        newCam.GetComponent<PlayerUI>().playerControl = Players[currentPlayerSelectingClass];

        Cameras[currentPlayerSelectingClass] = newCam.GetComponent<CameraController>();
        Cameras[currentPlayerSelectingClass].player = newPlayer.transform;
        Cameras[currentPlayerSelectingClass].playerControl = newPlayer.GetComponent<PlayerController>();

        Players[currentPlayerSelectingClass].PlayerId = currentPlayerSelectingClass;

        PlayerClass playerClass = (PlayerClass)Object.Instantiate(PlayerClasses[id]);
        playerClass.transform.parent = newPlayer.transform;
        Players[currentPlayerSelectingClass].PlayerClass = playerClass;

        slotButtons[currentPlayerSelectingClass].Text = "Selected: " + Players[currentPlayerSelectingClass].PlayerClass.name.Replace("(Clone)", "") + "\nClick to delete";
        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.green;

        UpdateReadyButton();
        UpdateSelectedClassUI();

        //SelectNextPlayerSlot();
    }

	public static Vector3 GetLevelSpawnPoint()
	{
		return Instance.LevelSpawnPoint;
	}

	public void OnPause()
	{
		GamePaused = true;
	}

	public void OnResume()
	{
		GamePaused = false;
	}

	public void AddPlayer(PlayerController playerController)
	{
		
		if (Players.Contains(playerController))
		{
			Debug.Log("Containing");
			return;
		}

		for (int i = 0; i < Players.Length; i++)
		{
			if (Players[i] == null)
			{
				Debug.Log("Player Added");
				Players[i] = playerController;
				return;
			}
		}
		Debug.Log("Not addded");
	}
}
