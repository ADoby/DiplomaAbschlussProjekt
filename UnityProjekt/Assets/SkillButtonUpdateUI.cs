using UnityEngine;
using System.Collections;

public class SkillButtonUpdateUI : MonoBehaviour {

    public UIButtonSkillUP[] buttons;

    void OnEnable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UpdateUI();
        }
    }
}
