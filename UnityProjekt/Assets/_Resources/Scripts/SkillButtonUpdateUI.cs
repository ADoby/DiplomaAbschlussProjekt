using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillButtonUpdateUI : MonoBehaviour {

    public PlayerUI playerUI;

    [SerializeField]
    public UIButtonSkillUP[] buttons;

    public int UpdateEveryFrames = 10;
    public int UpdateEveryCounter = 0;

    void Update()
    {
        if (playerUI.showMenu)
        {
            UpdateEveryCounter++;
            if (UpdateEveryCounter == UpdateEveryFrames)
            {
                UpdateEveryCounter = 0;
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].UpdateUI();
                }
            }
        }
    }
}
