using UnityEngine;
using System.Collections;

public class LevelUpGUI : MonoBehaviour {

    private PlayerController player;

    public float width = 0.8f, height = 0.8f;

    public static LevelUpGUI Instance;

    void Awake()
    {
        LevelUpGUI.Instance = this;
    }

    public static void OpenPlayer(PlayerController newPlayer)
    {
        LevelUpGUI.Instance.OpenWithPlayer(newPlayer);
    }

    public static void Close()
    {
        LevelUpGUI.Instance.CloseI();
    }

	// Update is called once per frame
	void Update () {
        if (player && InputController.GetClicked("ESCAPE"))
        {
            CloseI();
        }
	}

    public void OpenWithPlayer(PlayerController newPlayer)
    {
        player = newPlayer;
        GameEventHandler.TriggerOnPause();
    }

    public void CloseI()
    {
        player = null;
        GameEventHandler.TriggerOnResume();
    }

    public static bool IsOpened()
    {
        return Instance.IsOpenedI();
    }

    public bool IsOpenedI()
    {
        return (player != null);
    }

    public bool WantsToCloseI()
    {
        return true;
    }
    public static bool WantsToClose()
    {
        return Instance.WantsToCloseI();
    }

    void OnGUI()
    {
        if (player)
        {
            GUIStyle area = new GUIStyle(GUI.skin.window);

            GUILayout.BeginArea(new Rect((1f - width) / 2f * Screen.width, (1f - height) / 2f * Screen.height, width * Screen.width, height * Screen.height), area);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle button = new GUIStyle(GUI.skin.button);

            if(GUILayout.Button("Close", button))
            {
                CloseI();
                return;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Skill Points: " + player.PlayerClass.skillPoints.ToString());

            //GUILayout.Box("", GUILayout.Height(1));

            for (int i = 0; i < (int)AttributeType.COUNT; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(player.PlayerClass.GetAttribute(i).Name + ": " + player.PlayerClass.GetAttributeValue(i).ToString());
                if (GUILayout.Button("Add " + player.PlayerClass.GetAttribute(i).ValuePerSkillPoint))
                {
                    player.PlayerClass.SkillUpAttribute(i);
                    if (player.PlayerClass.skillPoints == 0)
                    {
                        CloseI();
                        return;
                    }
                }
                GUILayout.EndHorizontal();
                //GUILayout.Label("Current Value: " + player.PlayerClass.GetAttributeValue(i).ToString());

                //GUILayout.Box("", GUILayout.Height(1));
            }
            GUILayout.EndArea();
        }
    }
}
