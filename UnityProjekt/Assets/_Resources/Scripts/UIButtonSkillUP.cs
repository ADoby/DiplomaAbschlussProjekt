using UnityEngine;
using System.Collections;

public class UIButtonSkillUP : MonoBehaviour {

    public PlayerUI playerUI;

    public AttributeType attributeType;

    public string format;

    [Multiline(2)]
    public string text = "#SkillName#: <color=#AA0000>#value# (#fullValue#)</color>\n<color=#00CC66>Increase by <color=#AA0000>#valuePerPoint#</color></color>";

    public UIText uiText;

	// Use this for initialization
	void Start () {
        GetComponent<UIButton>().OnButtonClicked += OnButtonClicked;
	}

    [ContextMenu("ResetText")]
    public void ResetText()
    {
        text = "#SkillName#: <color=#ff0000>#value# (#fullValue#)</color>\n<color=#00CC66>Increase by <color=#ff0000>#valuePerPoint#</color></color> (#currentSkillUps#/#maxSkillUps#)";
    }

    public void UpdateUI()
    {
        if (!playerUI.playerControl)
            return;

        uiText.Text = text.Replace("#value#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).Value.ToString(format)).
            Replace("#fullValue#", playerUI.playerControl.PlayerClass.GetAttributeValue(attributeType).ToString(format)).
            Replace("#valuePerPoint#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).ValuePerSkillPoint.ToString(format)).
            Replace("#SkillName#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).Name).
            Replace("#currentSkillUps#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).currentSkillUps.ToString("##0")).
            Replace("#maxSkillUps#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).MaxSkillUps.ToString("##0"));

        playerUI.UpdateAfterSkillPointUP();
    }

    public UIRect uperPanel;

    public void OnButtonClicked(UIRect rect)
    {
        if (uperPanel.Visible)
        {
            playerUI.playerControl.PlayerClass.SkillUpAttribute((int)attributeType);
            UpdateUI();
        }
    }
}
