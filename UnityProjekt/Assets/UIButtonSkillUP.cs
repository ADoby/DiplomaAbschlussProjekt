using UnityEngine;
using System.Collections;

public class UIButtonSkillUP : MonoBehaviour {

    public PlayerUI playerUI;

    public AttributeType attributeType;

    public string format;

    [Multiline(2)]
    public string text = "SkillName: <color=#ff0000>#value# (#fullValue#)</color>\n<color=#00CC66>Increase by <color=#ff0000>#valuePerPoint#</color></color>";

    public UIText uiText;

	// Use this for initialization
	void Start () {
        GetComponent<UIButton>().OnButtonClicked += OnButtonClicked;
	}

    public void UpdateUI()
    {
        uiText.Text = text.Replace("#value#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).Value.ToString(format)).
            Replace("#fullValue#", playerUI.playerControl.PlayerClass.GetAttributeValue(attributeType).ToString(format)).
            Replace("#valuePerPoint#", playerUI.playerControl.PlayerClass.GetAttribute(attributeType).ValuePerSkillPoint.ToString(format));

        playerUI.UpdateUI();
    }

    public void OnButtonClicked(UIRect rect)
    {
        playerUI.playerControl.PlayerClass.SkillUpAttribute((int)attributeType);
        UpdateUI();
    }
}
