using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillButtonUpdateUI : MonoBehaviour {

    [SerializeField]
    public UIButtonSkillUP[] buttons;

    void OnEnable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UpdateUI();
        }
    }
}
